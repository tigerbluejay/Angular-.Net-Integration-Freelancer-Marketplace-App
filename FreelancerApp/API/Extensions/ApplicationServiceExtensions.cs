using API.Data;
using API.Filters;
using API.Helpers;
using API.Interfaces;
using API.Middleware;
using API.Repository;
using API.Services;
using API.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services,
	IConfiguration config)
	{
		services.AddControllers();
		services.AddHttpContextAccessor(); // 👈

		services.AddDbContext<DataContext>((serviceProvider, opt) =>
		 {
			 var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

			 // opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
			 opt.UseSqlServer(
	config.GetConnectionString("DefaultConnection")
);

			 opt.EnableDetailedErrors();
			 opt.EnableSensitiveDataLogging();

			 if (!config.GetValue<bool>("DisableEfSqlLogging"))
			 {
				 opt.LogTo((message) =>
				 {
					 if (message.Contains("Executed DbCommand"))
					 {
						 var context =
							 httpContextAccessor.HttpContext;

						 var queryCount = 0;

						 if (context != null)
						 {
							 if (context.Items.ContainsKey("QueryCount"))
							 {
								 context.Items["QueryCount"] =
									 (int)context.Items["QueryCount"] + 1;

								 queryCount =
									 (int)context.Items["QueryCount"];
							 }
						 }

						 var match =
							 Regex.Match(message, @"\((\d+)ms\)");

						 if (match.Success)
						 {
							 var duration =
								 int.Parse(match.Groups[1].Value);

							 if (duration > 100)
							 {
								 Debug.WriteLine(
									 $"🐢 SLOW QUERY: {duration}ms");

								 Log.Warning(
									 "Slow EF query detected | DurationMs: {DurationMs} | QueryCount: {QueryCount}",
									 duration,
									 queryCount);
							 }
						 }

						 Debug.WriteLine("──── EF CORE SQL ────");
						 Debug.WriteLine(message);
						 Debug.WriteLine("─────────────────────");

						 Log.Information(
							 "EF Core SQL executed | QueryCount: {QueryCount} | SqlMessage: {SqlMessage}",
							 queryCount,
							 message);
					 }
				 }, LogLevel.Information);
			 }
		 });
		services.AddCors();

		// Lifetime of Services
		// - AddSingleton - Created the first time they are requested
		// Every subsequent request for that service, will use the same instance
		// Good for caching data or maintain a state that should be shared across the application
		// - AddTransient - Created each time they are requested
		// Good for lightweight services
		// - AddScoped - Created once per client request (HTTP request)
		services.AddScoped<ITokenService, TokenService>();
		services.AddScoped<BulkSeedService>();
		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<IPortfolioItemRepository, PortfolioItemRepository>();
		services.AddScoped<IProjectRepository, ProjectRepository>();
		services.AddScoped<IPhotoService, PhotoService>();
		services.AddScoped<IProposalRepository, ProposalRepository>();
		services.AddScoped<IMessageRepository, MessageRepository>();
		services.AddScoped<IUnitOfWork, UnitOfWork>();
		services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
		services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
		services.AddSignalR();
		services.AddSingleton<PresenceTracker>();
		services.AddStackExchangeRedisCache(options =>
		{
			options.Configuration = "localhost:6379";
		});
		services.Configure<MvcOptions>(options =>
		{
			options.Filters.Add<NPlusOneClassicLoggingFilter>();
		});
		return services;

	}
}