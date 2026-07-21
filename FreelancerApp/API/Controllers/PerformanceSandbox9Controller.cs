using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PerformanceSandbox9Controller : ControllerBase
	{
		private readonly ILogger<PerformanceSandbox9Controller> _logger;
		private static readonly ActivitySource ActivitySource =
			new("PerformanceSandbox.Custom");
		private readonly DataContext _context;

		public PerformanceSandbox9Controller(
			ILogger<PerformanceSandbox9Controller> logger,
			DataContext context)
		{
			_logger = logger;
			_context = context;
		}

		// =========================================================
		// INTENTIONALLY BLOCKED THREAD ENDPOINT
		// =========================================================
		[HttpGet("blocked")]
		public IActionResult Blocked()
		{
			var stopwatch = Stopwatch.StartNew();

			_logger.LogInformation("Blocked endpoint started.");

			// Intentionally blocks the current ThreadPool worker thread.
			// This is for PerfView Thread Time demonstrations only.
			Thread.Sleep(2500);

			stopwatch.Stop();

			_logger.LogInformation(
				"Blocked endpoint completed in {ElapsedMs}ms.",
				stopwatch.ElapsedMilliseconds);

			if (stopwatch.ElapsedMilliseconds > 1000)
			{
				_logger.LogWarning(
					"Blocking operation detected: {ElapsedMs}ms.",
					stopwatch.ElapsedMilliseconds);
			}

			return Ok(new
			{
				message = "Blocked endpoint executed",
				elapsedMs = stopwatch.ElapsedMilliseconds
			});
		}

		// Endpoint to demonstrate OpenTelemetry tracing with ActivitySource
		// and Entity Framework Core.
		[HttpGet("dashboard")]
		public async Task<IActionResult> GetDashboard()
		{
			using var activity = ActivitySource.StartActivity("BuildDashboard");

			activity?.SetTag("dashboard.userId", 123);

			var projects = await _context.Projects
				.Include(p => p.Proposals)
				.ToListAsync();

			activity?.SetTag("dashboard.projectCount", projects.Count);

			await Task.Delay(300); // Simulate expensive processing

			return Ok(new
			{
				Count = projects.Count
			});
		}

		//Endpoint to demonstrate OpenTelemetry tracing with ActivitySource
		[HttpGet("external")]
		public async Task<IActionResult> CallExternalApi(
		[FromServices] IHttpClientFactory factory)
		{
			using var activity = ActivitySource.StartActivity("CallWeatherApi");

			var client = factory.CreateClient();

			var response = await client.GetAsync(
				"https://jsonplaceholder.typicode.com/posts/1");

			activity?.SetTag("external.statusCode", (int)response.StatusCode);

			return Ok(await response.Content.ReadAsStringAsync());
		}


	}
}