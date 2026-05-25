using API.Data;
using API.Entities;
using API.Extensions;
using API.Middleware;
using API.Services;
using API.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Serilog.Events;

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
	.Enrich.FromLogContext()
	.CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =========================================================
// REDIS
// =========================================================
builder.Services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = "localhost:6379";
});
// =========================================================
// BACKGROUND SERVICE
// =========================================================

builder.Services.AddHostedService<CleanupService>();
var app = builder.Build();

// DATABASE MIGRATION AND SEEDING

// SEEDING
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DataContext>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
        
        await context.Database.MigrateAsync();
        await context.Database.ExecuteSqlRawAsync("DELETE FROM [Connections]");
        await Seed.SeedUsers(userManager, roleManager, context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during seeding");
    }
}

// MIDDLEWARE
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


app.UseMiddleware<ExceptionMiddleware>(); // uses our custom error handling middleware
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseCors(x => x.AllowAnyHeader()
.AllowAnyMethod()
.AllowCredentials()
.WithOrigins("http://localhost:4200", "https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

app.Run();
