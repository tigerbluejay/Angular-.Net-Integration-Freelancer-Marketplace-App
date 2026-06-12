
using API.Data;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Respawn.Graph;

namespace API.Sandbox.Infrastructure;

public static class SandboxDatabaseReset
{
	public static async Task ResetAsync(
		DataContext context)
	{
		var connection =
			context.Database.GetDbConnection();

		await connection.OpenAsync();

		var respawner =
			await Respawner.CreateAsync(
				connection,
				new RespawnerOptions
				{
					DbAdapter = DbAdapter.SqlServer,

					TablesToIgnore = new[]
						{
							new Table("__EFMigrationsHistory")
						}
				});

		await respawner.ResetAsync(connection);
	}
}