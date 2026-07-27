using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Context;

namespace API.Filters;

public class NPlusOneClassicLoggingFilter : IAsyncActionFilter
{
	private const string TargetPath = "/api/PerformanceSandbox/nplus1-classic";

	private readonly ILogger<NPlusOneClassicLoggingFilter> _logger;

	public NPlusOneClassicLoggingFilter(ILogger<NPlusOneClassicLoggingFilter> logger)
	{
		_logger = logger;
	}

	public async Task OnActionExecutionAsync(
		ActionExecutingContext context,
		ActionExecutionDelegate next)
	{
		if (!context.HttpContext.Request.Path
				.Equals(TargetPath, StringComparison.OrdinalIgnoreCase))
		{
			await next();
			return;
		}

		var stopwatch = Stopwatch.StartNew();

		using (LogContext.PushProperty("Operation", "NPlusOneClassic"))
		{
			_logger.LogInformation("N+1 classic endpoint started");

			var executed = await next();

			stopwatch.Stop();

			var projectCount =
				(executed.Result as ObjectResult)?.Value is System.Collections.ICollection items
					? items.Count
					: (int?)null;

			var queryCount =
				context.HttpContext.Items.TryGetValue("QueryCount", out var qc)
					? (int)qc!
					: 0;

			_logger.LogWarning(
				"N+1 classic endpoint completed in {ElapsedMs} ms, returned {ProjectCount} projects, executed {QueryCount} EF queries",
				stopwatch.ElapsedMilliseconds,
				projectCount,
				queryCount);
		}
	}
}
