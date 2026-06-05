using System.Diagnostics;

namespace API.Middleware;

public class RequestTimingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<RequestTimingMiddleware> _logger;

	public RequestTimingMiddleware(
		RequestDelegate next,
		ILogger<RequestTimingMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var stopwatch = Stopwatch.StartNew();

		await _next(context);

		stopwatch.Stop();

		var elapsedMs =
			stopwatch.Elapsed.TotalMilliseconds;

		_logger.LogInformation(
			"HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs} ms",
			context.Request.Method,
			context.Request.Path,
			context.Response.StatusCode,
			elapsedMs);
	}
}