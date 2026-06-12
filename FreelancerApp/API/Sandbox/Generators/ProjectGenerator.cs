using API.Entities;
using Bogus;

namespace API.Sandbox.Generators;

public static class ProjectGenerator
{
	private static readonly string[] ProjectTitles =
	[
		"E-Commerce Platform",
		"Mobile Banking App",
		"CRM Dashboard",
		"AI Chatbot Integration",
		"Bug Fix Sprint",
		"Performance Optimization",
		"API Refactor",
		"Admin Portal",
		"Inventory System",
		"Marketplace Backend"
	];

	public static Faker<Project> Create(
		List<AppUser> clients)
	{
		return new Faker<Project>()
			.RuleFor(p => p.Title,
				f => f.PickRandom(ProjectTitles))

			.RuleFor(p => p.Description,
				f => f.Lorem.Paragraphs(2))

			// FK ONLY
			.RuleFor(p => p.ClientUserId,
				f => f.PickRandom(clients).Id);
	}
}