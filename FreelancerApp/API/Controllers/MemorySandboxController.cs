using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Entities;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MemorySandboxController : ControllerBase
{
	private readonly DataContext _context;
	private readonly ILogger<MemorySandboxController> _logger;

	// Simulates a poorly designed application cache
	private static readonly Dictionary<int, List<Project>> _projectCache = new();

	public MemorySandboxController(
		DataContext context,
		ILogger<MemorySandboxController> logger)
	{
		_context = context;
		_logger = logger;
	}

	[HttpGet("cache-projects/{cacheKey}")]
	public async Task<IActionResult> CacheProjects(int cacheKey)
	{
		_logger.LogWarning(
			"Loading project graph into cache with key {CacheKey}",
			cacheKey);

		var projects = await _context.Projects
			.Include(p => p.Client)
			.Include(p => p.Proposals)
				.ThenInclude(pr => pr.Freelancer)
			.Include(p => p.Skills)
			.ToListAsync();

		_projectCache[cacheKey] = projects;

		return Ok(new
		{
			CacheKey = cacheKey,
			ProjectsLoaded = projects.Count,
			CacheEntries = _projectCache.Count
		});
	}

	[HttpGet("cache-stats")]
	public IActionResult CacheStats()
	{
		return Ok(new
		{
			CacheEntries = _projectCache.Count,
			TotalProjects =
				_projectCache.Sum(x => x.Value.Count)
		});
	}

	[HttpDelete("cache-clear")]
	public IActionResult ClearCache()
	{
		_projectCache.Clear();

		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();

		return Ok("Cache cleared");
	}
}