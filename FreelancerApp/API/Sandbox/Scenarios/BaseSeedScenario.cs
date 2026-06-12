using API.Data;
using API.Entities;
using API.Sandbox.Generators;
using Serilog;

namespace API.Sandbox.Scenarios;

public static class BaseSeedScenario
{
	public static async Task SeedAsync(
		DataContext context)
	{
		Log.Information(
			"Running BaseSeedScenario");

		// =====================================================
		// USERS
		// =====================================================

		var users =
			AppUserGenerator
				.Create()
				.Generate(1000);

		context.Users.AddRange(users);

		await context.SaveChangesAsync();

		Log.Information(
			"Generated {Count} users",
			users.Count);

		// =====================================================
		// PROJECTS
		// =====================================================

		var projects =
			ProjectGenerator
				.Create(users)
				.Generate(5000);

		context.Projects.AddRange(projects);

		await context.SaveChangesAsync();

		Log.Information(
			"Generated {Count} projects",
			projects.Count);

		// =====================================================
		// PROPOSALS
		// =====================================================

		const int totalProposals = 20000;
		const int batchSize = 1000;

		var proposalGenerator =
			ProposalGenerator.Create(
				users,
				projects);

		for (int i = 0; i < totalProposals; i += batchSize)
		{
			var batch =
				proposalGenerator.Generate(batchSize);

			context.Proposals.AddRange(batch);

			await context.SaveChangesAsync();

			// important for large inserts
			context.ChangeTracker.Clear();

			Log.Information(
				"Inserted proposal batch {Current}/{Total}",
				Math.Min(i + batchSize, totalProposals),
				totalProposals);
		}

		Log.Information(
			"BaseSeedScenario complete");
	}
}