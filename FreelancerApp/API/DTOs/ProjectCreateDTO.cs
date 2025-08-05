
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class ProjectCreateDTO
{
    [Required]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PhotoUrl { get; set; }
    public List<string> Skills { get; set; } = new();
}