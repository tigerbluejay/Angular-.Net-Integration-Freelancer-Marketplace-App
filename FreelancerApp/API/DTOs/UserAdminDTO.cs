namespace API.DTOs;

public class UserAdminDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }

    public string KnownAs { get; set; }
    public string? Gender { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }

    public bool IsAvailable { get; set; }

    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }

    public string? PhotoUrl { get; set; }                 // Flattened from Photo
    public ICollection<string> Skills { get; set; } = []; // Simplified skill names

    public ICollection<string> Roles { get; set; } = [];  // Flattened from AppUserRole

    public bool IsAccountDisabled { get; set; }
}