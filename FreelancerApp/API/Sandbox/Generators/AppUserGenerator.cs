
using API.Entities;
using Bogus;

namespace API.Sandbox.Generators;

public static class AppUserGenerator
{
	public static Faker<AppUser> Create()
	{
		return new Faker<AppUser>()


			// Regular Entity Data
			.RuleFor(u => u.FirstName,
				f => f.Name.FirstName())

			.RuleFor(u => u.LastName,
				f => f.Name.LastName())

			.RuleFor(u => u.KnownAs,
				(f, u) => u.FirstName)

			.RuleFor(u => u.UserName,
				(f, u) =>
					$"{u.FirstName}.{u.LastName}"
					.ToLower()
					+ Guid.NewGuid().ToString("N")[..6])

			.RuleFor(u => u.Email,
				(f, u) =>
					$"{u.UserName}@example.com")

			.RuleFor(u => u.Country,
				f => f.Address.Country())

			.RuleFor(u => u.City,
				f => f.Address.City())

			.RuleFor(u => u.Bio,
				f => f.Lorem.Paragraph())

			.RuleFor(u => u.LookingFor,
				f => f.PickRandom(
					"Remote contracts",
					"Long-term clients",
					"Quick gigs",
					"Startup work",
					"Enterprise consulting"))

			.RuleFor(u => u.IsAvailable,
				f => f.Random.Bool(0.8f))

			.RuleFor(u => u.Created,
				f => f.Date.Past(3))

			.RuleFor(u => u.LastActive,
				f => f.Date.Recent(30))

		// Identity Inherited Entity Data
		.RuleFor(u => u.NormalizedUserName,
			(f, u) => u.UserName!.ToUpper())

		.RuleFor(u => u.NormalizedEmail,
			(f, u) => u.Email!.ToUpper())

		.RuleFor(u => u.SecurityStamp,
			f => Guid.NewGuid().ToString());
	}
}
