using API.Entities;
using Bogus;

namespace API.Sandbox.Generators;

public static class ProposalGenerator
{
	public static Faker<Proposal> Create(
		List<AppUser> freelancers,
		List<Project> projects)
	{
		return new Faker<Proposal>()

			.RuleFor(p => p.Title,
				f => "Proposal for " +
					 f.Company.CatchPhrase())

			.RuleFor(p => p.Description,
				f => f.Lorem.Paragraph())

			.RuleFor(p => p.Bid,
				f => Math.Round(
					f.Random.Decimal(100, 10000),
					2))

			.RuleFor(p => p.Created,
				f => f.Date.Recent(90))

			// choose project once
			.RuleFor(p => p.ProjectId,
				f => f.PickRandom(projects).Id)

			// derive client from selected project
			.RuleFor(p => p.ClientUserId,
				(f, p) =>
					projects.First(x =>
						x.Id == p.ProjectId)
					.ClientUserId)

			// freelancer
			.RuleFor(p => p.FreelancerUserId,
				f => f.PickRandom(freelancers).Id)

			.RuleFor(p => p.IsAccepted,
				f => f.Random.Bool(0.1f)
					? true
					: null);
	}
}