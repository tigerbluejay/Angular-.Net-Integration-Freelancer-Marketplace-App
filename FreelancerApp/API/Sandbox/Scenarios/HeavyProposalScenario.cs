using API.Data;
using API.Entities;
using API.Sandbox.Generators;
using Serilog;

namespace API.Sandbox.Scenarios;

public static class HeavyProposalScenario
	// single project has thousands of proposals
{
	public static async Task RunAsync(
		DataContext context,
		List<AppUser> users,
		List<Project> projects)
	{
		Log.Information(
			"Running HeavyProposalScenario");

		var hotProject =
			projects.First();

		var proposalGenerator =
			ProposalGenerator.Create(
				users,
				[hotProject]);

		const int total = 10000;
		const int batchSize = 1000;

		for (int i = 0; i < total; i += batchSize)
		{
			var batch =
				proposalGenerator.Generate(batchSize);

			context.Proposals.AddRange(batch);

			await context.SaveChangesAsync();

			context.ChangeTracker.Clear();

			Log.Information(
				"HeavyProposalScenario batch {Current}/{Total}",
				Math.Min(i + batchSize, total),
				total);
		}

		Log.Information(
			"Generated heavy proposal load");
	}
}