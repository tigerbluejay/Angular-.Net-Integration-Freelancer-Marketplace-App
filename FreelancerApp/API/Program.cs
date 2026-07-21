using API.Data;
using API.Entities;
using API.Extensions;
using API.Filters;
using API.Middleware;
using API.Sandbox;
using API.Sandbox.Infrastructure;
using API.Services;
using API.SignalR;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Bogus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text.Json.Serialization;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;


var builder = WebApplication.CreateBuilder(args);

// SERVICES
// =========================================================
// SERILOG CONFIGURATION
// =========================================================

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Information()
	.WriteTo.Console()
	.WriteTo.File(
		"logs/app-.txt",
		rollingInterval: RollingInterval.Day)
	.WriteTo.Seq("http://localhost:5341")
	.Enrich.FromLogContext()
	.CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

//builder.Services.AddControllers();
// =========================================================
// CONTROLLERS + ENUM SERIALIZATION
// =========================================================

builder.Services.AddControllers()
.AddJsonOptions(options =>
{
	options.JsonSerializerOptions.Converters
	.Add(new JsonStringEnumConverter());
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
	options.SerializerOptions.WriteIndented = true;
});

// =========================================================
// API VERSIONING
// =========================================================

builder.Services
	.AddApiVersioning(options =>
	{
		options.DefaultApiVersion = new ApiVersion(1, 0);

		options.AssumeDefaultVersionWhenUnspecified = true;

		options.ReportApiVersions = true;
	})
	.AddApiExplorer(options =>
	{
		options.GroupNameFormat = "'v'VVV";

		options.SubstituteApiVersionInUrl = true;
	});


// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
// =========================================================
// SWAGGER
// =========================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
options.SwaggerDoc("v1", new OpenApiInfo
{
	Title = "Freelancer Marketplace API",
	Version = "v1",
	Description = "Swagger sandbox for bug fixing and consulting workflows"
});
options.SwaggerDoc(
	"GeneralBugFixingSandbox",
	new OpenApiInfo
	{
		Title = "General Bug Fixing Sandbox",
		Version = "v1",
		Description =
		"Sandbox endpoints for debugging, Swagger testing, validation, auth, uploads, and API experimentation"
	});
	options.DocInclusionPredicate((docName, apiDesc) =>
	{
		var groupName = apiDesc.GroupName;

		// Main versioned API document
		if (docName == "v1" && groupName == "v1")
			return true;

		// Custom sandbox document
		if (docName == "GeneralBugFixingSandbox" &&
			apiDesc.ActionDescriptor.DisplayName?
				.Contains("GeneralBugFixingSandboxController") == true)
		{
			return true;
		}

		return false;
	});

	// =====================================================
	// XML COMMENTS
	// =====================================================

	var xmlFilename =
	$"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

options.IncludeXmlComments(
	Path.Combine(AppContext.BaseDirectory, xmlFilename));

// =====================================================
// JWT AUTH
// =====================================================

options.AddSecurityDefinition("Bearer",
    new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token"
    });

options.AddSecurityRequirement(
    new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

// =====================================================
// EXAMPLES
// =====================================================

options.ExampleFilters();

// =====================================================
// OPERATION FILTER
// =====================================================

options.OperationFilter<CorrelationIdOperationFilter>();

});


// =========================================================
// SWAGGER EXAMPLES
// =========================================================

builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

// =========================================================
// JWT AUTHENTICATION
// =========================================================

// JWT already configured inside AddIdentityServices()

builder.Services.AddAuthorization();

// =========================================================
// FORWARDED HEADERS
// Useful behind reverse proxies / nginx / cloud hosting
// =========================================================

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
	options.ForwardedHeaders =
		ForwardedHeaders.XForwardedFor |
		ForwardedHeaders.XForwardedProto;
});



// =========================================================
// REDIS
// =========================================================
builder.Services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = "localhost:6379";
});
// =========================================================
// CONFIGURE TRACING
// =========================================================

builder.Services.AddOpenTelemetry()
	.WithTracing(tracing =>
	{
		tracing
			.SetResourceBuilder(
				ResourceBuilder.CreateDefault()
					.AddService(
						serviceName: "PerformanceSandbox.Api",
						serviceVersion: "1.0.0")
					.AddAttributes(new[]
						{
						new KeyValuePair<string, object>("deployment.environment", "Development")
						}))
			.AddAspNetCoreInstrumentation()
			.AddOtlpExporter(options =>
			{
				options.Endpoint = new Uri("http://localhost:4319");
			});
	});

// =========================================================
// BACKGROUND SERVICE
// =========================================================

builder.Services.AddHostedService<CleanupService>();
var app = builder.Build();

Log.Information(
	"ENVIRONMENT: {Environment}",
	app.Environment.EnvironmentName);

Log.Information(
	"DB CONNECTION: {Connection}",
	builder.Configuration.GetConnectionString("DefaultConnection"));

// DATABASE MIGRATION AND SEEDING
if (!app.Environment.IsEnvironment("Testing"))
{
	using (var scope = app.Services.CreateScope())
	{
		var services = scope.ServiceProvider;

		try
		{
			var context =
				services.GetRequiredService<DataContext>();

			var userManager =
				services.GetRequiredService<UserManager<AppUser>>();

			var roleManager =
				services.GetRequiredService<RoleManager<AppRole>>();

			// =====================================================
			// APPLY MIGRATIONS
			// =====================================================

			await context.Database.MigrateAsync();

			// =====================================================
			// DEVELOPMENT ENVIRONMENT
			// =====================================================

			if (app.Environment.IsDevelopment())
			{
				Log.Information(
					"Running DEVELOPMENT seed pipeline");

				await context.Database.ExecuteSqlRawAsync(
					"DELETE FROM [Connections]");

				await Seed.SeedUsers(
					userManager,
					roleManager,
					context);
			}

			// =====================================================
			// SANDBOX ENVIRONMENT
			// =====================================================

			else if (app.Environment.IsEnvironment("Sandbox"))
			{
				Log.Information(
					"Running SANDBOX seed pipeline");

				Log.Information(
					"Resetting sandbox database");

				// seeders can sometimes use random data generators,
				// so we set a fixed seed for consistency across runs
				Randomizer.Seed = new Random(12345);

				await SandboxDatabaseReset.ResetAsync(context);

				Log.Information(
					"Sandbox database reset complete");

				await SandboxSeeder.SeedAsync(context);
			}
		}
		catch (Exception ex)
		{
			var logger =
				services.GetRequiredService<ILogger<Program>>();

			logger.LogError(
				ex,
				"An error occurred during database initialization");
		}
	}
}

// MIDDLEWARE

app.UseMiddleware<RequestTimingMiddleware>();
// =========================================================
// CORRELATION ID MIDDLEWARE
// =========================================================

app.Use(async (context, next) =>
{
	var correlationId = Guid.NewGuid().ToString();

	context.Items["CorrelationId"] = correlationId;

	using (LogContext.PushProperty(
		"CorrelationId",
		correlationId))
	{
		await next();
	}
});



// =========================================================
// SERILOG REQUEST LOGGING
// =========================================================

app.UseSerilogRequestLogging(options =>
{
	options.MessageTemplate =
		"HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

	options.GetLevel = (httpContext, elapsed, ex) =>
	{
		if (ex != null ||
			httpContext.Response.StatusCode >= 500)
		{
			return LogEventLevel.Error;
		}

		if (elapsed > 1000)
		{
			return LogEventLevel.Warning;
		}

		return LogEventLevel.Information;
	};
});

// =========================================================
// MIDDLEWARE
// =========================================================

app.UseForwardedHeaders();

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>(); // uses our custom error handling middleware
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseCors(x => x.AllowAnyHeader()
.AllowAnyMethod()
.AllowCredentials()
.WithOrigins("http://localhost:4200", "https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
	options.SwaggerEndpoint(
		"/swagger/v1/swagger.json",
		"Freelancer Marketplace API v1");

	options.SwaggerEndpoint(
		"/swagger/GeneralBugFixingSandbox/swagger.json",
		"General Bug Fixing Sandbox");
});

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

app.MapGet("/hello", () =>
{
	return "Hello OpenTelemetry!";
});

app.Run();

public partial class Program
{
}