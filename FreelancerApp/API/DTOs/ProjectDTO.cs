namespace API.DTOs;

public class ProjectDTO
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? PhotoUrl { get; set; }
    public bool IsAssigned { get; set; }

    public List<string> Skills { get; set; } = [];
}