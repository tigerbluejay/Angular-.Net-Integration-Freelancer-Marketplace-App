using System.Diagnostics;

public class RequestLoggingMiddleware
{
	private readonly RequestDelegate _next;

	public RequestLoggingMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var requestId = Guid.NewGuid().ToString("N")[..8];

		context.Items["QueryCount"] = 0;

		var stopwatch = Stopwatch.StartNew();

		Debug.WriteLine($"===== REQUEST START {requestId} | {DateTime.Now:HH:mm:ss.fff} =====");

		await _next(context);

		stopwatch.Stop();

		var count = context.Items["QueryCount"] ?? 0;

		Debug.WriteLine(
			$"===== REQUEST END {requestId} | {DateTime.Now:HH:mm:ss.fff} | " +
			$"Duration: {stopwatch.Elapsed.TotalMilliseconds:N2} ms | Queries: {count} =====");
	}
}