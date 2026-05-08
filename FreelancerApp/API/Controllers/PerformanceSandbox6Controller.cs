using System.Text.Json;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerformanceSandbox6Controller : ControllerBase
{
	private readonly DataContext _context;
	private readonly IDistributedCache _cache;

	public PerformanceSandbox6Controller(
		DataContext context,
		IDistributedCache cache)
	{
		_context = context;
		_cache = cache;
	}

	// =========================================
	// 1. NO CACHE
	// =========================================

	[HttpGet("dashboard-no-cache")]
	public async Task<IActionResult> GetDashboardNoCache()
	{
		var result = await BuildDashboard();

		return Ok(result);
	}

	// =========================================
	// 2. REDIS CACHE-ASIDE
	// =========================================

	[HttpGet("dashboard-redis")]
	public async Task<IActionResult> GetDashboardRedis()
	{
		const string cacheKey = "dashboard_summary";

		// TRY REDIS FIRST
		var cachedJson = await _cache.GetStringAsync(cacheKey);

		if (!string.IsNullOrEmpty(cachedJson))
		{
			var cachedResult =
				JsonSerializer.Deserialize<object>(cachedJson);

			return Ok(new
			{
				Source = "redis-cache",
				Data = cachedResult
			});
		}

		// CACHE MISS → DB
		var result = await BuildDashboard();

		var json = JsonSerializer.Serialize(result);

		await _cache.SetStringAsync(
			cacheKey,
			json,
			new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow =
					TimeSpan.FromMinutes(5)
			});

		return Ok(new
		{
			Source = "database",
			Data = result
		});
	}

	// =========================================
	// 3. WRITE + INVALIDATE CACHE
	// =========================================

	[HttpPost("create-project")]
	public async Task<IActionResult> CreateProject()
	{
		var client = await _context.Users.FirstAsync();

		var project = new API.Entities.Project
		{
			Title = Guid.NewGuid().ToString(),
			Description = "Redis cache invalidation test project",
			ClientUserId = client.Id
		};

		_context.Projects.Add(project);

		await _context.SaveChangesAsync();

		// INVALIDATE CACHE
		await _cache.RemoveAsync("dashboard_summary");

		return Ok(new
		{
			Message = "Project created and cache invalidated"
		});
	}

	// =========================================
	// EXPENSIVE QUERY
	// =========================================

	private async Task<object> BuildDashboard()
	{
		// Simulate realistic expensive marketplace query

		var projects = await _context.Projects
			.Include(p => p.Client)
			.Include(p => p.Proposals)
			.Include(p => p.Skills)
			.AsNoTracking()
			.ToListAsync();

		// Simulate CPU work too
		await Task.Delay(300);

		var result = new
		{
			TotalProjects = projects.Count,

			AssignedProjects =
				projects.Count(p => p.FreelancerUserId != null),

			TotalProposals =
				projects.Sum(p => p.Proposals.Count),

			AverageProposalsPerProject =
				projects.Any()
					? projects.Average(p => p.Proposals.Count)
					: 0,

			TopProjects = projects
				.OrderByDescending(p => p.Proposals.Count)
				.Take(5)
				.Select(p => new
				{
					p.Id,
					p.Title,
					ProposalCount = p.Proposals.Count,
					Client = p.Client.KnownAs
				})
				.ToList()
		};

		return result;
	}
}

/*
What This Gives You

Endpoint 1
GET /api/PerformanceSandbox6/dashboard-no-cache

Always: DB hit, joins, aggregations, artificial delay

Endpoint 2
GET /api/PerformanceSandbox6/dashboard-redis

First request: DB, serialization, Redis SET
Next requests: Redis GET only
Huge performance difference.

Endpoint 3
POST /api/PerformanceSandbox6/create-project
Creates project and: DEL dashboard_summary so next dashboard request: rebuilds cache

----------------------------------
What You’ll Observe

With Redis endpoint in Postman:

first request slower
subsequent requests dramatically faster

-------------
With k6:

lower latency
reduced DB pressure
better throughput

-------------
With BenchmarkDotNet:

cleaner comparison between:
pure DB path
Redis path
-------------

Important Small Detail

If your database is tiny: 👉 performance difference may initially look small.

That’s normal.

To exaggerate effects: add more projects, add more proposals, 
increase Task.Delay, add more joins/aggregations

For learning purposes that’s completely fine.

5. Your Experiment Flow

Step 1
Hit: dashboard-no-cache
multiple times.
Observe: consistently slow

Step 2
Hit: dashboard-redis
Observe: first request slower
later requests much faster

Step 3
Hit: create-project, Then hit Redis endpoint again.
Observe: cache invalidated, DB path triggered again

That’s the FULL distributed cache lifecycle in practice:
- cache-aside
- expiration
- invalidation
- distributed cache reuse

Very realistic backend workflow.
*/