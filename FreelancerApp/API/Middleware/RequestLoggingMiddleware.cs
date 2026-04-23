using System.Diagnostics;

namespace API.Middleware
{
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

			Debug.WriteLine($"=============================================================");
			Debug.WriteLine($"===== REQUEST START {requestId} {DateTime.Now:HH:mm:ss} =====");
			Debug.WriteLine($"=============================================================");

			await _next(context);

			Debug.WriteLine($"=============================================================");
			Debug.WriteLine($"===== REQUEST END {requestId} {DateTime.Now:HH:mm:ss} =======");
			Debug.WriteLine($"=============================================================");

		}
	}
}
