// CleanupService.cs

using System.Diagnostics;

namespace API.Services;

public class CleanupService : BackgroundService
{
	private readonly ILogger<CleanupService> _logger;

	public CleanupService(
		ILogger<CleanupService> logger)
	{
		_logger = logger;
	}

	protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			var stopwatch = Stopwatch.StartNew();

			try
			{
				_logger.LogInformation(
					"Cleanup job started");

				// Simulate work
				await Task.Delay(3000, stoppingToken);

				stopwatch.Stop();

				_logger.LogInformation(
					"Cleanup job completed in {ElapsedMs}ms",
					stopwatch.ElapsedMilliseconds);

				if (stopwatch.ElapsedMilliseconds > 1000)
				{
					_logger.LogWarning(
						"Slow cleanup job detected: {ElapsedMs}ms",
						stopwatch.ElapsedMilliseconds);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(
					ex,
					"Cleanup job failed");
			}

			await Task.Delay(
				TimeSpan.FromMinutes(5),
				stoppingToken);
		}
	}
}