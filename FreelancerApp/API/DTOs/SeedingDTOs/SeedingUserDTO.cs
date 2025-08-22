namespace API.DTOs;

public class SeedingUserDTO
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Optional: Plain-text password for hashing during seeding
    public string Password { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string? KnownAs { get; set; }
    public string? Gender { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }

    public string? Bio { get; set; }
    public string? LookingFor { get; set; }
    public string? Website { get; set; }
    public string? LinkedIn { get; set; }
    public string? GitHub { get; set; }

    public bool IsAvailable { get; set; } = true;

}