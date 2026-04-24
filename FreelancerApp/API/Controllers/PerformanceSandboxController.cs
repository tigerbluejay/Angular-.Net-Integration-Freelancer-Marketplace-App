using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerformanceSandboxController : ControllerBase
{
	private readonly DataContext _context;

	public PerformanceSandboxController(DataContext context)
	{
		_context = context;
	}

	// ------------------------------------------------------------
	// 1. 🔴 Classic N+1 (Loop + Query)
	// ------------------------------------------------------------
	[HttpGet("nplus1-classic")]
	public async Task<ActionResult> GetProjectsWithClient_NPlus1()
	{
		var projects = await _context.Projects.ToListAsync();

		var result = new List<object>();

		foreach (var project in projects)
		{
			// 🚨 One query per project
			var client = await _context.Users
				.FirstOrDefaultAsync(u => u.Id == project.ClientUserId);

			result.Add(new
			{
				ProjectId = project.Id,
				ProjectTitle = project.Title,
				ClientName = client != null ? client.KnownAs : "Unknown"
			});
		}

		return Ok(result);
	}

	/* fix: one query with join or projection to get all data at once
			no per row db calls
			only required columns selected

	var projects = await _context.Projects
	.Select(p => new
	{
		p.Id,
		ClientName = p.Client.UserName
	})
	.ToListAsync();
	*/



	// ------------------------------------------------------------
	// 2. 🟠 Nested N+1 (Explosion)
	// Projects → Proposals → Freelancer
	// ------------------------------------------------------------
	[HttpGet("nplus1-nested")]
	public async Task<ActionResult> GetProjectsWithProposals_NestedNPlus1()
	{
		var projects = await _context.Projects.ToListAsync();

		var result = new List<object>();

		foreach (var project in projects)
		{
			// 🚨 Query per project
			var proposals = await _context.Proposals
				.Where(p => p.ProjectId == project.Id)
				.ToListAsync();

			var proposalDtos = new List<object>();

			foreach (var proposal in proposals)
			{
				// 🚨 Query per proposal (multiplies problem)
				var freelancer = await _context.Users
					.FirstOrDefaultAsync(u => u.Id == proposal.FreelancerUserId);

				proposalDtos.Add(new
				{
					ProposalId = proposal.Id,
					Bid = proposal.Bid,
					FreelancerName = freelancer?.KnownAs
				});
			}

			result.Add(new
			{
				ProjectId = project.Id,
				Proposals = proposalDtos
			});
		}

		return Ok(result);
	}


	/* fix: ef builds join across all levels in one query
			executes once
			still returns nested structure without multiple db calls
				only required columns selected

	var data = await _context.Projects
    .Select(p => new
    {
        p.Id,
        Proposals = p.Proposals.Select(pr => new
        {
            pr.Id,
            FreelancerName = pr.Freelancer.UserName
        }).ToList()
    })
    .ToListAsync();
	*/

	// ------------------------------------------------------------
	// 3. 🟡 Hidden N+1 in Projection (FirstOrDefault inside Select)
	// ------------------------------------------------------------
	[HttpGet("nplus1-hidden-projection")]
	public async Task<ActionResult> GetProjects_HiddenNPlus1()
	{
		var result = await _context.Projects
			.Select(p => new
			{
				p.Id,
				p.Title,

				// 🚨 Looks clean, but can generate correlated subquery per row
				Client = _context.Users
					.FirstOrDefault(u => u.Id == p.ClientUserId),

				ProposalCount = _context.Proposals
					.Count(pr => pr.ProjectId == p.Id)
			})
			.ToListAsync();

		return Ok(result);
	}

	/* fix: uses navigation property, sql translates to join, executes once, no per row subqueries
			avoids separate queries per row for related data

	var result = await _context.Projects
    .Select(p => new
    {
        p.Id,
        LatestProposal = p.Proposals
            .OrderByDescending(pr => pr.CreatedAt)
            .Select(pr => new { pr.Id, pr.CreatedAt })
            .FirstOrDefault()
    })
    .ToListAsync();
	*/

	// ------------------------------------------------------------
	// 4. 🔵 Aggregation N+1 (Count inside loop)
	// ------------------------------------------------------------
	[HttpGet("nplus1-aggregation")]
	public async Task<ActionResult> GetProjectProposalCounts_NPlus1()
	{
		var projects = await _context.Projects.ToListAsync();

		var result = new List<object>();

		foreach (var project in projects)
		{
			// 🚨 Query per project
			var proposalCount = await _context.Proposals
				.CountAsync(p => p.ProjectId == project.Id);

			result.Add(new
			{
				ProjectId = project.Id,
				ProjectTitle = project.Title,
				ProposalCount = proposalCount
			});
		}

		return Ok(result);
	}


	/* fix 1: no loops in memory, aggregation done in sql
	   fix 2: single grouped query if you only need counts, more efficient than join + group by

	var projects = await _context.Projects
	.Select(p => new
	{
		p.Id,
		ProposalCount = p.Proposals.Count()
	})
	.ToListAsync();

	🧠 Alternative (more control)
		var counts = await _context.Proposals
    .GroupBy(p => p.ProjectId)
    .Select(g => new
    {
        ProjectId = g.Key,
        Count = g.Count()
    })
    .ToListAsync();
	*/

	// ------------------------------------------------------------
	// 5. 💣 Include Cartesian Explosion
	// ------------------------------------------------------------
	[HttpGet("include-explosion")]
	public async Task<ActionResult> GetProjects_WithIncludesExplosion()
	{
		var projects = await _context.Projects
			.Include(p => p.Client)
			.Include(p => p.Proposals)
				.ThenInclude(pr => pr.Freelancer)
			.Include(p => p.Skills)
			.ToListAsync();

		// Looks innocent, but SQL will explode rows

		var result = projects.Select(p => new
		{
			p.Id,
			p.Title,
			Client = p.Client.KnownAs,

			ProposalCount = p.Proposals.Count,

			Skills = p.Skills.Select(s => s.Name).ToList(),

			Freelancers = p.Proposals
				.Select(pr => pr.Freelancer.KnownAs)
				.Distinct()
				.ToList()
		});

		return Ok(result);
	}
}

/* fix 1: only fetch what you need, avoids duplication entirely
 * fix 2: if you need includes, use split queries to avoid cartesian explosion
 		  breaks one huge join into multiple smaller queries, still avoids N+1 but prevents row multiplication
 

		✅ Fix #1: Projection (BEST default)
	var projects = await _context.Projects
		.Select(p => new
		{
			p.Id,
			Proposals = p.Proposals.Select(pr => new
			{
				pr.Id,
				Messages = pr.Messages.Select(m => new
				{
					m.Id,
					m.Content
				}).ToList()
			}).ToList()
		})
		.ToListAsync();

		✅ Fix #2: Split queries (when Include needed)
	var projects = await _context.Projects
		.Include(p => p.Proposals)
		.ThenInclude(pr => pr.Messages)
		.AsSplitQuery()
		.ToListAsync();
	*/