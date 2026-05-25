// PerformanceSandbox7Controller.cs

using System.Diagnostics;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerformanceSandbox7Controller : ControllerBase
{
	private readonly ILogger<PerformanceSandbox7Controller> _logger;
	private readonly DataContext _context;
	private readonly IDistributedCache _cache;

	public PerformanceSandbox7Controller(
		ILogger<PerformanceSandbox7Controller> logger,
		DataContext context,
		IDistributedCache cache)
	{
		_logger = logger;
		_context = context;
		_cache = cache;
	}

	// =========================================================
	// 1. FAST ENDPOINT
	// =========================================================
	[HttpGet("fast")]
	public IActionResult Fast()
	{
		var stopwatch = Stopwatch.StartNew();

		_logger.LogInformation(
			"Fast endpoint execution started");

		stopwatch.Stop();

		_logger.LogInformation(
			"Fast endpoint completed in {ElapsedMs}ms",
			stopwatch.ElapsedMilliseconds);

		return Ok(new
		{
			message = "Fast endpoint executed",
			elapsedMs = stopwatch.ElapsedMilliseconds
		});
	}

	// =========================================================
	// 2. INTENTIONALLY SLOW ENDPOINT
	// =========================================================
	[HttpGet("slow")]
	public async Task<IActionResult> Slow()
	{
		var stopwatch = Stopwatch.StartNew();

		_logger.LogInformation(
			"Slow endpoint started");

		await Task.Delay(2500);

		stopwatch.Stop();

		_logger.LogInformation(
			"Slow endpoint completed in {ElapsedMs}ms",
			stopwatch.ElapsedMilliseconds);

		if (stopwatch.ElapsedMilliseconds > 1000)
		{
			_logger.LogWarning(
				"Slow operation detected: {ElapsedMs}ms",
				stopwatch.ElapsedMilliseconds);
		}

		return Ok(new
		{
			message = "Slow endpoint executed",
			elapsedMs = stopwatch.ElapsedMilliseconds
		});
	}

	// =========================================================
	// 3. ENDPOINT THAT THROWS
	// =========================================================
	[HttpGet("throw")]
	public IActionResult Throw()
	{
		try
		{
			_logger.LogInformation(
				"Throw endpoint started");

			throw new Exception("Intentional sandbox exception");
		}
		catch (Exception ex)
		{
			_logger.LogError(
				ex,
				"Intentional exception occurred in throw endpoint");

			return StatusCode(500, new
			{
				error = ex.Message
			});
		}
	}

	// =========================================================
	// 4. DATABASE QUERY ENDPOINT
	// =========================================================
	[HttpGet("projects")]
	public async Task<IActionResult> GetProjects()
	{
		var stopwatch = Stopwatch.StartNew();

		try
		{
			_logger.LogInformation(
				"Projects query started");

			var projects = await _context.Projects
				.Take(20)
				.ToListAsync();

			stopwatch.Stop();

			_logger.LogInformation(
				"Projects query completed in {ElapsedMs}ms",
				stopwatch.ElapsedMilliseconds);

			if (stopwatch.ElapsedMilliseconds > 1000)
			{
				_logger.LogWarning(
					"Slow query detected: {ElapsedMs}ms",
					stopwatch.ElapsedMilliseconds);
			}

			return Ok(projects);
		}
		catch (Exception ex)
		{
			_logger.LogError(
				ex,
				"Projects query failed");

			return StatusCode(500,
				"Database query failed");
		}
	}

	// =========================================================
	// 5. REDIS CACHE ENDPOINT
	// =========================================================
	[HttpGet("cached-users")]
	public async Task<IActionResult> CachedUsers()
	{
		var stopwatch = Stopwatch.StartNew();

		const string cacheKey = "sandbox-users";

		try
		{
			var cachedValue =
				await _cache.GetStringAsync(cacheKey);

			if (!string.IsNullOrEmpty(cachedValue))
			{
				stopwatch.Stop();

				_logger.LogInformation(
					"Users retrieved from Redis cache in {ElapsedMs}ms",
					stopwatch.ElapsedMilliseconds);

				return Ok(new
				{
					source = "redis-cache",
					data = cachedValue
				});
			}

			_logger.LogWarning(
				"Redis cache miss occurred");

			var users = await _context.Users
				.Take(10)
				.Select(x => new
				{
					x.Id,
					x.UserName,
					x.KnownAs,
					x.City,
					x.Country
				})
				.ToListAsync();

			var serialized =
				System.Text.Json.JsonSerializer.Serialize(users);

			await _cache.SetStringAsync(
				cacheKey,
				serialized,
				new DistributedCacheEntryOptions
				{
					AbsoluteExpirationRelativeToNow =
						TimeSpan.FromMinutes(5)
				});

			stopwatch.Stop();

			_logger.LogInformation(
				"Users retrieved from DB and cached in {ElapsedMs}ms",
				stopwatch.ElapsedMilliseconds);

			return Ok(new
			{
				source = "database",
				data = users
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(
				ex,
				"Redis cache endpoint failed");

			return StatusCode(500,
				"Redis cache operation failed");
		}
	}

	// =========================================================
	// 6. BUSINESS ACTION / STATE TRANSITION ENDPOINT
	// =========================================================
	[HttpPost("assign-project/{projectId}/{freelancerId}")]
	public async Task<IActionResult> AssignProject(
		int projectId,
		int freelancerId)
	{
		var stopwatch = Stopwatch.StartNew();

		try
		{
			var project = await _context.Projects
				.FirstOrDefaultAsync(x => x.Id == projectId);

			if (project == null)
			{
				_logger.LogWarning(
					"Project {ProjectId} was not found",
					projectId);

				return NotFound("Project not found");
			}

			project.FreelancerUserId = freelancerId;

			await _context.SaveChangesAsync();

			stopwatch.Stop();

			_logger.LogInformation(
				"Project {ProjectId} assigned to Freelancer {FreelancerId}",
				projectId,
				freelancerId);

			_logger.LogInformation(
				"Project assignment completed in {ElapsedMs}ms",
				stopwatch.ElapsedMilliseconds);

			return Ok(new
			{
				message = "Project assigned successfully"
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(
				ex,
				"Project assignment failed for Project {ProjectId}",
				projectId);

			return StatusCode(500,
				"Assignment failed");
		}
	}
}