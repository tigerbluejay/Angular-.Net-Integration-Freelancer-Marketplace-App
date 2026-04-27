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

		Debug.WriteLine($"===== REQUEST START {requestId} =====");

		await _next(context);

		var count = context.Items["QueryCount"] ?? 0;

		Debug.WriteLine($"===== REQUEST END {requestId} | Queries: {count} =====");
	}
}