using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerformanceSandbox2Controller : ControllerBase
{
	private readonly DataContext _context;

	public PerformanceSandbox2Controller(DataContext context)
	{
		_context = context;
	}

	// 🔴 1. TABLE SCAN (Missing Index)
	// Trigger: filter on non-indexed column (e.g., Bio or City)
	[HttpGet("table-scan")]
	public async Task<IActionResult> TableScan(string city)
	{
		var users = await _context.Users
			.Where(u => u.City == city) // likely no index
			.ToListAsync();

		return Ok(users);
	}

	// 🔴 2. N+1 Problem
	[HttpGet("n-plus-one")]
	public async Task<IActionResult> NPlusOne()
	{
		var projects = await _context.Projects.ToListAsync();

		var result = new List<object>();

		foreach (var project in projects)
		{
			var proposals = await _context.Proposals
				.Where(p => p.ProjectId == project.Id)
				.ToListAsync();

			result.Add(new
			{
				project.Id,
				project.Title,
				ProposalCount = proposals.Count
			});
		}

		return Ok(result);
	}

	// 🟡 3. Key Lookup (non-covering index scenario)
	[HttpGet("key-lookup")]
	public async Task<IActionResult> KeyLookup()
	{
		var ordersLike = await _context.Projects
			.Where(p => p.Title.Contains("r")) // assume index exists later
			.Select(p => new
			{
				p.Id,
				p.Title,
				p.Description // forces lookup if not in index
			})
			.ToListAsync();

		return Ok(ordersLike);
	}

	// 🔴 4. Over-fetching (huge joins / includes)
	[HttpGet("over-fetching")]
	public async Task<IActionResult> OverFetching()
	{
		var data = await _context.Projects
			.Include(p => p.Client)
			.Include(p => p.Freelancer)
			.Include(p => p.Proposals)
				.ThenInclude(pr => pr.Freelancer)
			.Include(p => p.Conversations)
				.ThenInclude(c => c.Messages)
			.ToListAsync();

		return Ok(data);
	}

	// 🔴 5. Bad Filtering (Index NOT used)
	[HttpGet("bad-filter")]
	public async Task<IActionResult> BadFilter()
	{
		var result = await _context.Proposals
			.Where(p => p.Created.Year == 2026) // breaks index usage
			.ToListAsync();

		return Ok(result);
	}
}