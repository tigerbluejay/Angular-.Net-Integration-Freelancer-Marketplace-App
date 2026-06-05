using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Serilog.Context;
using System.Diagnostics;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerformanceSandbox8Controller : ControllerBase
{
	private readonly DataContext _context;
	private readonly ILogger<PerformanceSandbox8Controller> _logger;
	private readonly IDistributedCache _cache;

	public PerformanceSandbox8Controller(
		DataContext context,
		ILogger<PerformanceSandbox8Controller> logger,
		IDistributedCache cache)
	{
		_context = context;
		_logger = logger;
		_cache = cache;
	}

	// =========================================================
	// SLOW ENDPOINT
	// =========================================================
	[HttpGet("slow")]
	public async Task<IActionResult> Slow()
	{
		var stopwatch = Stopwatch.StartNew();

		using (LogContext.PushProperty(
			"Operation",
			"SlowEndpoint"))
		{
			_logger.LogInformation(
				"Slow endpoint started");

			await Task.Delay(5000);

			stopwatch.Stop();

			_logger.LogWarning(
				"Slow endpoint completed in {ElapsedMs} ms",
				stopwatch.ElapsedMilliseconds);

			return Ok(new
			{
				elapsedMs = stopwatch.ElapsedMilliseconds
			});
		}
	}

	// =========================================================
	// N+1 QUERY ENDPOINT
	// =========================================================
	[HttpGet("nplus1")]
	public async Task<IActionResult> NPlusOne()
	{
		var stopwatch = Stopwatch.StartNew();

		using (LogContext.PushProperty(
			"Operation",
			"NPlusOne"))
		{
			_logger.LogInformation(
				"N+1 query endpoint started");

			var users = await _context.Users
				.Take(20)
				.ToListAsync();

			var results = new List<object>();

			foreach (var user in users)
			{
				var userStopwatch = Stopwatch.StartNew();

				var photos = await _context.Photos
					.Where(x => x.UserId == user.Id)
					.ToListAsync();

				userStopwatch.Stop();

				_logger.LogInformation(
					"N+1 query executed for UserId {UserId} in {ElapsedMs} ms with {PhotoCount} photos",
					user.Id,
					userStopwatch.ElapsedMilliseconds,
					photos.Count);

				results.Add(new
				{
					user.Id,
					user.UserName,
					PhotoCount = photos.Count
				});
			}

			stopwatch.Stop();

			_logger.LogWarning(
				"N+1 endpoint completed in {ElapsedMs} ms",
				stopwatch.ElapsedMilliseconds);

			return Ok(results);
		}
	}

	// =========================================================
	// EXCEPTION ENDPOINT
	// =========================================================
	[HttpGet("exception")]
	public IActionResult Exception()
	{
		using (LogContext.PushProperty(
			"Operation",
			"ExceptionEndpoint"))
		{
			try
			{
				_logger.LogInformation(
					"Exception endpoint started");

				throw new InvalidOperationException(
					"Intentional sandbox exception");
			}
			catch (Exception ex)
			{
				_logger.LogError(
					ex,
					"Exception endpoint failed with message {ErrorMessage}",
					ex.Message);

				throw;
			}
		}
	}

	// =========================================================
	// TIMEOUT ENDPOINT
	// =========================================================
	[HttpGet("timeout")]
	public async Task<IActionResult> Timeout()
	{
		var stopwatch = Stopwatch.StartNew();

		using var cts = new CancellationTokenSource();

		cts.CancelAfter(TimeSpan.FromSeconds(2));

		try
		{
			_logger.LogInformation(
				"Timeout endpoint started with timeout {TimeoutSeconds} seconds",
				2);

			await Task.Delay(
				TimeSpan.FromSeconds(10),
				cts.Token);

			return Ok();
		}
		catch (TaskCanceledException ex)
		{
			stopwatch.Stop();

			_logger.LogError(
				ex,
				"Timeout endpoint cancelled after {ElapsedMs} ms",
				stopwatch.ElapsedMilliseconds);

			return StatusCode(408, new
			{
				message = "Operation timed out"
			});
		}
	}

	// =========================================================
	// RETRY ENDPOINT
	// =========================================================
	[HttpGet("retry")]
	public async Task<IActionResult> Retry()
	{
		var stopwatch = Stopwatch.StartNew();

		for (var attempt = 1; attempt <= 3; attempt++)
		{
			try
			{
				_logger.LogInformation(
					"Retry attempt {AttemptNumber} started",
					attempt);

				if (attempt < 3)
				{
					throw new Exception(
						"Simulated transient failure");
				}

				await Task.Delay(500);

				stopwatch.Stop();

				_logger.LogInformation(
					"Retry endpoint succeeded after {AttemptCount} attempts in {ElapsedMs} ms",
					attempt,
					stopwatch.ElapsedMilliseconds);

				return Ok(new
				{
					attempts = attempt,
					elapsedMs = stopwatch.ElapsedMilliseconds
				});
			}
			catch (Exception ex)
			{
				_logger.LogWarning(
					ex,
					"Retry attempt {AttemptNumber} failed with message {ErrorMessage}",
					attempt,
					ex.Message);

				await Task.Delay(1000);
			}
		}

		stopwatch.Stop();

		_logger.LogError(
			"Retry endpoint failed after {ElapsedMs} ms",
			stopwatch.ElapsedMilliseconds);

		return StatusCode(500);
	}

	// =========================================================
	// BAD EF QUERY
	// =========================================================
	[HttpGet("bad-ef-query")]
	public async Task<IActionResult> BadEfQuery()
	{
		var stopwatch = Stopwatch.StartNew();

		_logger.LogInformation(
			"Bad EF query endpoint started");

		var users = await _context.Users
			.ToListAsync();

		_logger.LogWarning(
			"Entire Users table loaded into memory with {UserCount} records",
			users.Count);

		var filtered = users
			.Where(x =>
				x.UserName!.ToLower().Contains("a"))
			.OrderBy(x => x.UserName)
			.ToList();

		stopwatch.Stop();

		_logger.LogWarning(
			"Bad EF query completed in {ElapsedMs} ms returning {ResultCount} records",
			stopwatch.ElapsedMilliseconds,
			filtered.Count);

		return Ok(filtered);
	}

	// =========================================================
	// CACHE ENDPOINT
	// =========================================================
	[HttpGet("cache")]
	public async Task<IActionResult> Cache()
	{
		var cacheKey = "sandbox-cache-key";

		var stopwatch = Stopwatch.StartNew();

		var cachedValue =
			await _cache.GetStringAsync(cacheKey);

		if (cachedValue != null)
		{
			stopwatch.Stop();

			_logger.LogInformation(
				"Cache hit for Key {CacheKey} in {ElapsedMs} ms",
				cacheKey,
				stopwatch.ElapsedMilliseconds);

			return Ok(new
			{
				source = "cache",
				value = cachedValue
			});
		}

		_logger.LogWarning(
			"Cache miss for Key {CacheKey}",
			cacheKey);

		await Task.Delay(2000);

		var generatedValue =
			$"Generated at {DateTime.UtcNow}";

		await _cache.SetStringAsync(
			cacheKey,
			generatedValue,
			new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow =
					TimeSpan.FromMinutes(5)
			});

		stopwatch.Stop();

		_logger.LogInformation(
			"Cache populated for Key {CacheKey} in {ElapsedMs} ms",
			cacheKey,
			stopwatch.ElapsedMilliseconds);

		return Ok(new
		{
			source = "database",
			value = generatedValue
		});
	}
}