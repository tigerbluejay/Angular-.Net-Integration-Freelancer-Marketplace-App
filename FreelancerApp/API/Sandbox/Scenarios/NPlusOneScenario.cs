using API.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace API.Sandbox.Scenarios;

public static class NPlusOneScenario
{
	public static async Task RunAsync(
		DataContext context)
	{
		Log.Information(
			"Running N+1 scenario");

		var projects =
			await context.Projects
				.Take(500)
				.ToListAsync();

		foreach (var project in projects)
		{
			var proposalCount =
				await context.Proposals
					.CountAsync(p =>
						p.ProjectId == project.Id);

			Log.Information(
				"Project {ProjectId} has {Count} proposals",
				project.Id,
				proposalCount);
		}
	}
}