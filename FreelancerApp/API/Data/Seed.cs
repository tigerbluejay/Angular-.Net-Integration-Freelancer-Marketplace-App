using System.Text.Json;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Identity;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager,
        DataContext context)
    {
        // Remove dependent entities first
        context.Photos.RemoveRange(context.Photos);
        context.Skills.RemoveRange(context.Skills);
        context.PortfolioItems.RemoveRange(context.PortfolioItems);
        context.Projects.RemoveRange(context.Projects);
        await context.SaveChangesAsync();

        // Delete existing users and roles
        foreach (var user in userManager.Users.ToList())
            await userManager.DeleteAsync(user);

        foreach (var role in roleManager.Roles.ToList())
            await roleManager.DeleteAsync(role);

        // Seed roles
        var roles = new[] { "Freelancer", "Client", "Admin" }.Select(r => new AppRole { Name = r });
        foreach (var role in roles)
            await roleManager.CreateAsync(role);

        // Load users from JSON
        var userData = JsonSerializer.Deserialize<List<SeedingUserDTO>>(
            await File.ReadAllTextAsync("Data/UserSeedData.json"));
        if (userData is null) return;

        var users = new List<AppUser>();

        foreach (var dto in userData)
        {
            var user = new AppUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth,
                KnownAs = dto.KnownAs,
                Gender = dto.Gender,
                Country = dto.Country,
                City = dto.City,
                Bio = dto.Bio,
                LookingFor = dto.LookingFor,
                Website = dto.Website,
                LinkedIn = dto.LinkedIn,
                GitHub = dto.GitHub,
                IsAvailable = dto.IsAvailable
            };

            var result = await userManager.CreateAsync(user, "Pa$$w0rd");
            if (!result.Succeeded) continue;

            var role = user.UserName.StartsWith("freelancer", StringComparison.OrdinalIgnoreCase) ? "Freelancer" : "Client";
            await userManager.AddToRoleAsync(user, role);
            users.Add(user);
        }

        // Add photos
        var photos = new List<Photo>
        {
            new Photo { Url = "https://randomuser.me/api/portraits/women/1.jpg", User = users[0] },
            new Photo { Url = "https://randomuser.me/api/portraits/men/1.jpg", User = users[1] },
            new Photo { Url = "https://randomuser.me/api/portraits/women/2.jpg", User = users[2] },
            new Photo { Url = "https://randomuser.me/api/portraits/men/2.jpg", User = users[3] },
            new Photo { Url = "https://randomuser.me/api/portraits/women/3.jpg", User = users[4] },
            new Photo { Url = "https://randomuser.me/api/portraits/men/3.jpg", User = users[5] },
            new Photo { Url = "https://randomuser.me/api/portraits/women/4.jpg", User = users[6] },
            new Photo { Url = "https://randomuser.me/api/portraits/men/4.jpg", User = users[7] },
            new Photo { Url = "https://randomuser.me/api/portraits/women/5.jpg", User = users[8] },
            new Photo { Url = "https://randomuser.me/api/portraits/men/5.jpg", User = users[9] }
        };
        context.Photos.AddRange(photos);

        // Add portfolio items
        var portfolioItems = Enumerable.Range(1, 7).Select(i =>
{
    var photo = new Photo
    {
        Url = $"https://picsum.photos/id/{236 + i}/600/400"
    };

    return new PortfolioItem
    {
        Title = $"Portfolio Item {i}",
        Description = "Sample portfolio description.",
        User = users[0],
        Photo = photo
    };
}).ToList();

        context.PortfolioItems.AddRange(portfolioItems);

        // Seed skills
        var skillData = new List<Skill>
        {
            new() { Name = "C#" },
            new() { Name = "ASP.NET Core" },
            new() { Name = "SQL" },
            new() { Name = "Angular" },
            new() { Name = "JavaScript" }
        };
        context.Skills.AddRange(skillData);
        await context.SaveChangesAsync(); // Track the skills

        // Assign skills to users
        users[0].Skills = new List<Skill> { skillData[0], skillData[1] };
        users[1].Skills = new List<Skill> { skillData[2], skillData[3] };
        users[2].Skills = new List<Skill> { skillData[0], skillData[4] };
        users[3].Skills = new List<Skill> { skillData[1] };
        users[4].Skills = new List<Skill> { skillData[2], skillData[3] };

        // Add projects with tracked skills
        var projectData = new List<Project>
        {
            new() { Title = "Project 1", Photo = new Photo { Url = "https://picsum.photos/id/537/600/400" }, Description = "Description", ClientUserId = users[5].Id, Skills = new List<Skill> { skillData[0], skillData[1] }},
            new() { Title = "Project 2", Photo = new Photo { Url = "https://picsum.photos/id/538/600/400" }, Description = "Description", ClientUserId = users[5].Id, Skills = new List<Skill> { skillData[2], skillData[3] }},
            new() { Title = "Project 3", Photo = new Photo { Url = "https://picsum.photos/id/539/600/400" }, Description = "Description", ClientUserId = users[5].Id, Skills = new List<Skill> { skillData[1], skillData[2] }},
            new() { Title = "Project 4", Photo = new Photo { Url = "https://picsum.photos/id/541/600/400" }, Description = "Description", ClientUserId = users[5].Id, Skills = new List<Skill> { skillData[4] }},
            new() { Title = "Project 5", Photo = new Photo { Url = "https://picsum.photos/id/542/600/400" }, Description = "Description", ClientUserId = users[5].Id, Skills = new List<Skill> { skillData[1] }},
            new() { Title = "Project 6", Photo = new Photo { Url = "https://picsum.photos/id/543/600/400" }, Description = "Description", ClientUserId = users[5].Id, Skills = new List<Skill> { skillData[0], skillData[4] }},
            new() { Title = "Project 7", Photo = new Photo { Url = "https://picsum.photos/id/544/600/400" }, Description = "Description", ClientUserId = users[5].Id, Skills = new List<Skill> { skillData[2], skillData[3] }}
        };

        context.Projects.AddRange(projectData);
        await context.SaveChangesAsync();

        // Optional: assign freelancer projects later if needed
        users[0].FreelancerProjects = new List<Project> { projectData[0], projectData[2] };

        // Update users to track their relationships
        foreach (var user in users)
            context.Users.Update(user);

        await context.SaveChangesAsync();

        // Create admin
        var admin = new AppUser { UserName = "admin", KnownAs = "admin" };
        await userManager.CreateAsync(admin, "Pa$$w0rd");
        await userManager.AddToRolesAsync(admin, new[] { "Admin" });

        await context.SaveChangesAsync();
    }
}