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
        // Delete Photos and Skills (dependent on Users)
        context.Photos.RemoveRange(context.Photos);
        context.Skills.RemoveRange(context.Skills);
        await context.SaveChangesAsync();

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

        // Load user data from JSON
        var userData = JsonSerializer.Deserialize<List<SeedingUserDTO>>(
            await File.ReadAllTextAsync("Data/UserSeedData.json")
        );

        if (userData == null) return;

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
            if (!result.Succeeded)
            {
                Console.WriteLine($"Failed to create user {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                continue;
            }

            var role = user.UserName.StartsWith("freelancer", StringComparison.OrdinalIgnoreCase) ? "Freelancer" : "Client";
            await userManager.AddToRoleAsync(user, role);

            users.Add(user); // for use in seeding photos
        }

        // Seed photos using created users
        var photos = new List<Photo>
        {
            new Photo { Url = "https://randomuser.me/api/portraits/women/1.jpg", UserId = users[0].Id },
            new Photo { Url = "https://randomuser.me/api/portraits/men/1.jpg", UserId = users[1].Id },
            new Photo { Url = "https://randomuser.me/api/portraits/women/2.jpg", UserId = users[2].Id },
            new Photo { Url = "https://randomuser.me/api/portraits/men/2.jpg", UserId = users[3].Id },
            new Photo { Url = "https://randomuser.me/api/portraits/women/3.jpg", UserId = users[4].Id },
            new Photo { Url = "https://randomuser.me/api/portraits/men/3.jpg", UserId = users[5].Id },
            new Photo { Url = "https://randomuser.me/api/portraits/women/4.jpg", UserId = users[6].Id },
            new Photo { Url = "https://randomuser.me/api/portraits/men/4.jpg", UserId = users[7].Id },
            new Photo { Url = "https://randomuser.me/api/portraits/women/5.jpg", UserId = users[8].Id },
            new Photo { Url = "https://randomuser.me/api/portraits/men/5.jpg", UserId = users[9].Id }
        };

        // Clear existing photos (optional)
        context.Photos.RemoveRange(context.Photos);
        await context.SaveChangesAsync();

        // save photos to DB
        context.Photos.AddRange(photos);
        await context.SaveChangesAsync();


        // Seed PortfolioItems using created users
        var portfolioItems = new List<PortfolioItem>
        {
            new PortfolioItem { Id = 1, PhotoUrl = "https://picsum.photos/id/237/600/400", Title = "Portofilio Item 1", UserId = users[0].Id, Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur." },
            new PortfolioItem { Id = 2, PhotoUrl = "https://picsum.photos/id/238/600/400", Title = "Portofilio Item 2", UserId = users[0].Id, Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur." },
            new PortfolioItem { Id = 3, PhotoUrl = "https://picsum.photos/id/239/600/400", Title = "Portofilio Item 3", UserId = users[0].Id, Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur." },
            new PortfolioItem { Id = 4, PhotoUrl = "https://picsum.photos/id/240/600/400", Title = "Portofilio Item 4", UserId = users[0].Id, Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur." },
            new PortfolioItem { Id = 5, PhotoUrl = "https://picsum.photos/id/241/600/400", Title = "Portofilio Item 5", UserId = users[0].Id, Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur." },
            new PortfolioItem { Id = 6, PhotoUrl = "https://picsum.photos/id/242/600/400", Title = "Portofilio Item 6", UserId = users[0].Id, Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur." },
            new PortfolioItem { Id = 7, PhotoUrl = "https://picsum.photos/id/243/600/400", Title = "Portofilio Item 7", UserId = users[0].Id, Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur." }
        };

        // Clear existing portfolioItems (optional)
        context.PortfolioItems.RemoveRange(context.PortfolioItems);
        await context.SaveChangesAsync();

        // Add portfolioItems to DB
        context.PortfolioItems.AddRange(portfolioItems);
        await context.SaveChangesAsync();


        // Seed skills
        // After you have created users and have the `users` list available

        // Example skill data - you could also load this from a JSON file like you do already
        var skillData = new List<Skill>
        {
            new Skill { Name = "C#" },
            new Skill { Name = "ASP.NET Core" },
            new Skill { Name = "SQL" },
            new Skill { Name = "Angular" },
            new Skill { Name = "JavaScript" },
        };

        // Clear existing skills (optional)
        context.Skills.RemoveRange(context.Skills);
        await context.SaveChangesAsync();

        // Add skills to DB
        await context.Skills.AddRangeAsync(skillData);
        await context.SaveChangesAsync();

        // Now assign some skills to your users
        // Example: assign first skill to first user, second skill to second user, etc.
        users[0].Skills = new List<Skill> { skillData[0], skillData[1] };  // user 0 knows C#, ASP.NET Core
        users[1].Skills = new List<Skill> { skillData[2], skillData[3] };  // user 1 knows SQL, Angular
        users[2].Skills = new List<Skill> { skillData[0], skillData[4] };  // user 0 knows C#, ASP.NET Core
        users[3].Skills = new List<Skill> { skillData[1] };  // user 1 knows SQL, Angular
        users[4].Skills = new List<Skill> { skillData[2], skillData[3] };  // user 1 knows SQL, Angular
 
        // For the rest, assign skills as needed or skip

        // Update users to persist skill assignments
        foreach (var user in users)
        {
            // This attaches skills to the user in the context's tracking
            context.Users.Update(user);
        }

        await context.SaveChangesAsync();


        // Create admin user
        var admin = new AppUser { UserName = "admin" };
        await userManager.CreateAsync(admin, "Pa$$w0rd");
        await userManager.AddToRolesAsync(admin, new[] { "Admin" });
        await context.SaveChangesAsync();
    }
}