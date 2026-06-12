using API.Data;
using API.Sandbox.Scenarios;
using Bogus;
using Serilog;

namespace API.Sandbox;

public static class SandboxSeeder
{
	public static async Task SeedAsync(
		DataContext context)
	{
		Log.Information(
			"Starting sandbox seed pipeline");

		Randomizer.Seed =
			new Random(12345);

		// =====================================================
		// BASE DATASET
		// =====================================================

		await BaseSeedScenario
			.SeedAsync(context);

		// =====================================================
		// SPECIALIZED SANDBOX SCENARIOS
		// =====================================================
		// comment the scenarios you dont want to reproduce
		// uncomment the scenarios you want to reproduce

		var users =
			context.Users.ToList();

		var projects =
			context.Projects.ToList();

		// three clients have a huge number of projects
		await HotClientScenario.RunAsync(
			context,
			users);

		// once project has a huge number of proposals
		await HeavyProposalScenario.RunAsync(
			context,
			users,
			projects);

		// activate NPlusOneScenario to expose a very innefficient query
		// due to one project having a huge number of proposals
		// await NPlusOneScenario.RunAsync(context);

		// active SlowDashboardScenario to expose a very innefficient query
		// due to one project having a huge number of proposals
		// await SlowDashboardScenario.RunAsync(context);

		Log.Information(
			"Sandbox seed pipeline complete");
	}
}