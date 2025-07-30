namespace API.DTOs;

public class MemberDTO
{
    public int Id { get; set; }                         // From IdentityUser<int>
    public string UserName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }

    public string? Bio { get; set; }
    public string? LookingFor { get; set; }
    public string? Website { get; set; }
    public string? LinkedIn { get; set; }
    public string? GitHub { get; set; }

    public bool IsAvailable { get; set; }

    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }

    public string? PhotoUrl { get; set; }                 // Flattened from Photo
    public ICollection<string> Skills { get; set; } = []; // Simplified skill names

    public ICollection<string> Roles { get; set; } = [];  // Flattened from AppUserRole

    public ICollection<PortfolioItemDTO> PortfolioItems { get; set; } = new List<PortfolioItemDTO>();

    public List<ProjectDTO> ClientProjects { get; set; } = [];

    public List<ProjectDTO> FreelancerProjects { get; set; } = [];

}
/*
Notes:
PhotoUrl can be mapped from AppUser.Photo.Url using AutoMapper or manual projection.
Skills flattens each Skill.Name from the AppUser.Skills collection.
Roles flattens the roles from the AppUser.UserRoles navigation property.
*/