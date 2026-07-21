using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PerformanceSandbox9Controller : ControllerBase
	{
		private readonly ILogger<PerformanceSandbox9Controller> _logger;

		public PerformanceSandbox9Controller(
			ILogger<PerformanceSandbox9Controller> logger)
		{
			_logger = logger;
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
	}
}