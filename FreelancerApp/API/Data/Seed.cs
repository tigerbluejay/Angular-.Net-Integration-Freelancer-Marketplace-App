using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager,
        DataContext context)
    {

        // Delete existing users
        var usersToDelete = userManager.Users.ToList();
        foreach (var user in usersToDelete)
        {
            await userManager.DeleteAsync(user);
        }

        // Delete existing roles
        var rolesToDelete = roleManager.Roles.ToList();
        foreach (var role in rolesToDelete)
        {
            await roleManager.DeleteAsync(role);
        }

        // Seed roles
        var roles = new List<AppRole>
        {
            new AppRole { Name = "Freelancer" },
            new AppRole { Name = "Client" },
            new AppRole { Name = "Admin" }
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }

        // Seed users
        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

        if (users == null) return;


        // Seed skills
        var skillData = await File.ReadAllTextAsync("Data/SkillSeedData.json");
        var skills = JsonSerializer.Deserialize<List<Skill>>(skillData, options);
        if (skills == null) return;

        context.Skills.RemoveRange(context.Skills); // clear existing
        await context.Skills.AddRangeAsync(skills);
        await context.SaveChangesAsync();

        var rnd = new Random();

        for (int i = 0; i < users.Count; i++)
        {
            var user = users[i];
            user.UserName = user.UserName!.ToLower();

            user.Skills = skills.OrderBy(_ => rnd.Next()).Take(rnd.Next(2, 5)).ToList();

            await userManager.CreateAsync(user, "Pa$$w0rd");

            if (i < 5)
            {
                await userManager.AddToRoleAsync(user, "Freelancer");
            }
            else
            {
                await userManager.AddToRoleAsync(user, "Client");
            }
        }

        // Create an admin user example
        var admin = new AppUser { UserName = "admin" };
        await userManager.CreateAsync(admin, "Pa$$w0rd");
        await userManager.AddToRolesAsync(admin, new[] { "Admin" });
    }
}