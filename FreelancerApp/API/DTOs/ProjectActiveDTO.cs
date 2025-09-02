namespace API.DTOs;

public class ProjectActiveDTO
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsAssigned { get; set; }
    public List<string> SkillNames { get; set; } = new();
    public int ClientUserId { get; set; }
    public string? ClientKnownAs { get; set; }
    public string? ClientPhotoUrl { get; set; }
    public int? FreelancerUserId { get; set; }
    public string? FreelancerKnownAs { get; set; }
    public string? FreelancerPhotoUrl { get; set; }
    public string? PhotoUrl { get; set; }
}