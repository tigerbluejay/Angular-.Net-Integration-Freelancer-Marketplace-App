using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace API.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory
	: WebApplicationFactory<Program>
{
	private SqliteConnection? _connection;

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseEnvironment("Testing");

		builder.ConfigureServices(services =>
		{
			var descriptor = services.SingleOrDefault(
				d => d.ServiceType == typeof(DbContextOptions<DataContext>));

			if (descriptor != null)
				services.Remove(descriptor);

			_connection = new SqliteConnection("DataSource=:memory:");
			_connection.Open();

			services.AddDbContext<DataContext>(options =>
			{
				options.UseSqlite(_connection);
			});

			var provider = services.BuildServiceProvider();

			using var scope = provider.CreateScope();

			var db = scope.ServiceProvider.GetRequiredService<DataContext>();

			db.Database.EnsureCreated();

			var userManager =
				scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

			var roleManager =
				scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

			Seed.SeedUsers(
				userManager,
				roleManager,
				db)
			.GetAwaiter()
			.GetResult();
		});
	}

	protected override void Dispose(bool disposing)
	{
		_connection?.Dispose();
		base.Dispose(disposing);
	}
}