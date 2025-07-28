using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppUser : IdentityUser<int>
{
    // ASP.NET Identity provides:
    // Id, UserName, Email, PasswordHash, etc.

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }

    public string? Bio { get; set; }              // Brief intro
    public string? LookingFor { get; set; }       // Optional: e.g., “remote gigs”, “short-term contracts”
    public string? Website { get; set; }          // Optional portfolio URL
    public string? LinkedIn { get; set; }
    public string? GitHub { get; set; }

    public bool IsAvailable { get; set; } = true; // Can toggle availability for new projects

    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;

    // Navigation property — single photo (profile picture)
    public Photo? Photo { get; set; }
    public int? PhotoId { get; set; }

    // Navigation property - many skills per user (and many users per skill)
    public ICollection<Skill> Skills { get; set; } = [];

    // Identity role relationship
    public ICollection<AppUserRole> UserRoles { get; set; } = [];

    // One-to-many relationship with PortfolioItems
    public ICollection<PortfolioItem> PortfolioItems { get; set; } = [];

    // Project relationships
    // public ICollection<Project> ProjectsAsClient { get; set; } = [];
    // public ICollection<Project> ProjectsAsFreelancer { get; set; } = [];

}