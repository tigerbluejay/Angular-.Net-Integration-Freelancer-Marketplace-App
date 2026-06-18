using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerformanceSandbox3Controller(DataContext context) : ControllerBase
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
			for (int i = 0; i < 500; i++)
			{
				result += project.Title + i + " | ";
			}
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
			for (int i = 0; i < 500; i++)
			{
				sb.Append(project.Title);
				sb.Append(i);
				sb.Append(" | ");
			}
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

	/// <summary>
	///  Additional Artificial Method to Simulate an Amplified N+1 Scenario with existing db data.
	/// </summary>
	/// <returns></returns>


	[HttpGet("nplus1-amplified")]
	public async Task<ActionResult> NPlus1Amplified()
	{
		int queryCount = 0;

		var result = new List<object>();

		for (int repeat = 0; repeat < 100; repeat++)
		{
			var projects = await context.Projects.ToListAsync();
			queryCount++;

			foreach (var project in projects)
			{
				var proposals = await context.Proposals
					.Where(p => p.ProjectId == project.Id)
					.ToListAsync();

				queryCount++;

				foreach (var proposal in proposals)
				{
					var freelancer = await context.Users
						.FirstOrDefaultAsync(u => u.Id == proposal.FreelancerUserId);

					queryCount++;

					result.Add(new
					{
						ProjectId = project.Id,
						ProposalId = proposal.Id,
						FreelancerName = freelancer?.KnownAs
					});
				}
			}
		}

		return Ok(new
		{
			QueryCount = queryCount,
			ResultCount = result.Count
		});
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