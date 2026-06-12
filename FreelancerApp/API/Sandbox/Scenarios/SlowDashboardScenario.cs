using API.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace API.Sandbox.Scenarios;

public static class SlowDashboardScenario
{
	public static async Task RunAsync(
		DataContext context)
	{
		Log.Information(
			"Running SlowDashboardScenario");

		var dashboard =
			await context.Projects
				.Select(p => new
				{
					p.Id,
					p.Title,
					ProposalCount =
						p.Proposals.Count,

					LatestProposal =
						p.Proposals
							.OrderByDescending(x => x.Created)
							.Select(x => x.Bid)
							.FirstOrDefault()
				})
				.OrderByDescending(x => x.ProposalCount)
				.Take(100)
				.ToListAsync();

		Log.Information(
			"Dashboard rows: {Count}",
			dashboard.Count);
	}
}