using API.Data;
using API.Entities;
using API.Sandbox.Generators;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace API.Sandbox.Scenarios;

public static class HotClientScenario
	// three clients have more than 600 projects each, the rest just 15 or so
{
	public static async Task RunAsync(
		DataContext context,
		List<AppUser> users)
	{
		Log.Information(
			"Running HotClientScenario");

		var hotClients =
			users.Take(3).ToList();

		var projectGenerator =
			ProjectGenerator.Create(hotClients);

		var hotProjects =
			projectGenerator.Generate(2000);

		context.Projects.AddRange(hotProjects);

		await context.SaveChangesAsync();

		Log.Information(
			"Generated hot client projects");
	}
}