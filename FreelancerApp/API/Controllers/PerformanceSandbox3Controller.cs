using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectSandbox3Controller(DataContext context) : ControllerBase
{
	////////////////////////////////////////////////////////////
	// 1. STRING CONCATENATION
	////////////////////////////////////////////////////////////

	[HttpGet("string-concat")]
	public ActionResult<string> StringConcat()
	{
		string result = "";

		var projects = context.Projects
			.AsNoTracking()
			.Take(500)
			.ToList();

		foreach (var project in projects)
		{
			result += project.Title + " | ";
		}

		return Ok(result);
	}

	////////////////////////////////////////////////////////////
	// 2. STRING BUILDER
	////////////////////////////////////////////////////////////

	[HttpGet("string-builder")]
	public ActionResult<string> StringBuilderExample()
	{
		var sb = new StringBuilder();

		var projects = context.Projects
			.AsNoTracking()
			.Take(500)
			.ToList();

		foreach (var project in projects)
		{
			sb.Append(project.Title);
			sb.Append(" | ");
		}

		return Ok(sb.ToString());
	}

	////////////////////////////////////////////////////////////
	// 3. LIST.CONTAINS
	////////////////////////////////////////////////////////////

	[HttpGet("list-contains")]
	public async Task<ActionResult<bool>> ListContains()
	{
		var projectIds = await context.Projects
			.AsNoTracking()
			.Select(p => p.Id)
			.ToListAsync();

		bool exists = projectIds.Contains(250);

		return Ok(exists);
	}

	////////////////////////////////////////////////////////////
	// 4. HASHSET.CONTAINS
	////////////////////////////////////////////////////////////

	[HttpGet("hashset-contains")]
	public async Task<ActionResult<bool>> HashSetContains()
	{
		var projectIds = await context.Projects
			.AsNoTracking()
			.Select(p => p.Id)
			.ToListAsync();

		var hashSet = projectIds.ToHashSet();

		bool exists = hashSet.Contains(250);

		return Ok(exists);
	}

	////////////////////////////////////////////////////////////
	// 5. ANY()
	////////////////////////////////////////////////////////////

	[HttpGet("any")]
	public async Task<ActionResult<bool>> AnyExample()
	{
		bool hasProjects = await context.Projects
			.AsNoTracking()
			.AnyAsync();

		return Ok(hasProjects);
	}

	////////////////////////////////////////////////////////////
	// 6. COUNT() > 0
	////////////////////////////////////////////////////////////

	[HttpGet("count")]
	public async Task<ActionResult<bool>> CountExample()
	{
		bool hasProjects = await context.Projects
			.AsNoTracking()
			.CountAsync() > 0;

		return Ok(hasProjects);
	}
}

/*
Suggested Benchmarking Order

Start with:

1. StringConcat vs StringBuilder

You’ll likely observe:

dramatically more allocations with concatenation
increasingly worse performance as size grows
2. Any vs Count

Usually:

Any() wins
because:
SQL can stop early

while:

Count() often scans/counts fully
3. List.Contains vs HashSet.Contains

Initially:
difference may appear small.

Increase dataset size aggressively:

1k
10k
100k

Then HashSet advantages become much clearer.
*/