using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;

namespace API.Services
{
	public class BulkSeedService
	{
		private readonly DataContext _context;

		public BulkSeedService(DataContext context)
		{
			_context = context;
		}

		public async Task SeedUsersAsync()
		{
			// HERE

			using var connection =
				(SqlConnection)_context.Database.GetDbConnection();

			await connection.OpenAsync();

			// Build DataTable...
			var table = new DataTable();

			table.Columns.Add("FirstName", typeof(string));
			table.Columns.Add("LastName", typeof(string));

			table.Columns.Add("DateOfBirth", typeof(DateTime));

			table.Columns.Add("KnownAs", typeof(string));

			table.Columns.Add("Gender", typeof(string));
			table.Columns.Add("Country", typeof(string));
			table.Columns.Add("City", typeof(string));

			table.Columns.Add("Bio", typeof(string));
			table.Columns.Add("LookingFor", typeof(string));

			table.Columns.Add("Website", typeof(string));
			table.Columns.Add("LinkedIn", typeof(string));
			table.Columns.Add("GitHub", typeof(string));

			table.Columns.Add("IsAvailable", typeof(bool));

			table.Columns.Add("Created", typeof(DateTime));
			table.Columns.Add("LastActive", typeof(DateTime));

			//table.Columns.Add("PhotoId", typeof(int));

			table.Columns.Add("IsAccountDisabled", typeof(bool));

			table.Columns.Add("UserName", typeof(string));
			table.Columns.Add("NormalizedUserName", typeof(string));

			table.Columns.Add("Email", typeof(string));
			table.Columns.Add("NormalizedEmail", typeof(string));

			table.Columns.Add("EmailConfirmed", typeof(bool));

			table.Columns.Add("PasswordHash", typeof(string));

			table.Columns.Add("SecurityStamp", typeof(string));
			table.Columns.Add("ConcurrencyStamp", typeof(string));

			//table.Columns.Add("PhoneNumber", typeof(string));

			table.Columns.Add("PhoneNumberConfirmed", typeof(bool));

			table.Columns.Add("TwoFactorEnabled", typeof(bool));

			//table.Columns.Add("LockoutEnd", typeof(DateTimeOffset));

			table.Columns.Add("LockoutEnabled", typeof(bool));

			table.Columns.Add("AccessFailedCount", typeof(int));

			// Create SqlBulkCopy...

			var hasher = new PasswordHasher<AppUser>();

			var passwordHash = hasher.HashPassword(
				new AppUser
				{
					KnownAs = "Seeder"
				},
				"Pa$$w0rd");


			for (int i = 1; i <= 200000; i++)
			{
				var row = table.NewRow();

				row["FirstName"] = "John";
				row["LastName"] = "Doe";

				row["DateOfBirth"] = new DateTime(1990, 1, 1);

				row["KnownAs"] = $"User {i}";

				row["Gender"] = DBNull.Value;
				row["Country"] = DBNull.Value;
				row["City"] = DBNull.Value;

				row["Bio"] = DBNull.Value;
				row["LookingFor"] = DBNull.Value;

				row["Website"] = DBNull.Value;
				row["LinkedIn"] = DBNull.Value;
				row["GitHub"] = DBNull.Value;

				row["IsAvailable"] = true;

				row["Created"] = DateTime.UtcNow;
				row["LastActive"] = DateTime.UtcNow;

				row["IsAccountDisabled"] = false;

				row["UserName"] = $"user{i}";
				row["NormalizedUserName"] = $"USER{i}";

				row["Email"] = $"user{i}@gmail.com";
				row["NormalizedEmail"] = $"USER{i}@GMAIL.COM";

				row["EmailConfirmed"] = true;

				row["PasswordHash"] = passwordHash;

				row["SecurityStamp"] = Guid.NewGuid().ToString();
				row["ConcurrencyStamp"] = Guid.NewGuid().ToString();

				row["PhoneNumberConfirmed"] = false;

				row["TwoFactorEnabled"] = false;

				row["LockoutEnabled"] = false;

				row["AccessFailedCount"] = 0;

				table.Rows.Add(row);
			}

			using var bulkCopy = new SqlBulkCopy(connection);
			bulkCopy.DestinationTableName = "AspNetUsers";
			bulkCopy.BatchSize = 5000;
			bulkCopy.BulkCopyTimeout = 0;
			bulkCopy.NotifyAfter = 5000;

			bulkCopy.SqlRowsCopied += (_, e) =>
			{
				Console.WriteLine($"{e.RowsCopied:N0} rows...");
			};


			bulkCopy.ColumnMappings.Add("FirstName", "FirstName");
			bulkCopy.ColumnMappings.Add("LastName", "LastName");
			bulkCopy.ColumnMappings.Add("DateOfBirth", "DateOfBirth");
			bulkCopy.ColumnMappings.Add("KnownAs", "KnownAs");

			bulkCopy.ColumnMappings.Add("Gender", "Gender");
			bulkCopy.ColumnMappings.Add("Country", "Country");
			bulkCopy.ColumnMappings.Add("City", "City");

			bulkCopy.ColumnMappings.Add("Bio", "Bio");
			bulkCopy.ColumnMappings.Add("LookingFor", "LookingFor");

			bulkCopy.ColumnMappings.Add("Website", "Website");
			bulkCopy.ColumnMappings.Add("LinkedIn", "LinkedIn");
			bulkCopy.ColumnMappings.Add("GitHub", "GitHub");

			bulkCopy.ColumnMappings.Add("IsAvailable", "IsAvailable");

			bulkCopy.ColumnMappings.Add("Created", "Created");
			bulkCopy.ColumnMappings.Add("LastActive", "LastActive");

			bulkCopy.ColumnMappings.Add("IsAccountDisabled", "IsAccountDisabled");

			bulkCopy.ColumnMappings.Add("UserName", "UserName");
			bulkCopy.ColumnMappings.Add("NormalizedUserName", "NormalizedUserName");

			bulkCopy.ColumnMappings.Add("Email", "Email");
			bulkCopy.ColumnMappings.Add("NormalizedEmail", "NormalizedEmail");

			bulkCopy.ColumnMappings.Add("EmailConfirmed", "EmailConfirmed");

			bulkCopy.ColumnMappings.Add("PasswordHash", "PasswordHash");

			bulkCopy.ColumnMappings.Add("SecurityStamp", "SecurityStamp");
			bulkCopy.ColumnMappings.Add("ConcurrencyStamp", "ConcurrencyStamp");

			bulkCopy.ColumnMappings.Add("PhoneNumberConfirmed", "PhoneNumberConfirmed");

			bulkCopy.ColumnMappings.Add("TwoFactorEnabled", "TwoFactorEnabled");

			bulkCopy.ColumnMappings.Add("LockoutEnabled", "LockoutEnabled");

			bulkCopy.ColumnMappings.Add("AccessFailedCount", "AccessFailedCount");

			// WriteToServerAsync(...)
			var sw = Stopwatch.StartNew();

			await bulkCopy.WriteToServerAsync(table);

			sw.Stop();

			Console.WriteLine($"Imported {table.Rows.Count:N0} users");
			Console.WriteLine($"Elapsed: {sw.Elapsed}");
			Console.WriteLine(
				$"Rows/sec: {table.Rows.Count / Math.Max(sw.Elapsed.TotalSeconds, 0.001):N0}");
		}
	}
}
