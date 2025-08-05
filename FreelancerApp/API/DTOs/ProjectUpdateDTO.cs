
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class ProjectUpdateDTO
{
    public int Id { get; set; }  // Needed to know which project to update
    [Required]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PhotoUrl { get; set; }
    public List<string> Skills { get; set; } = new();
}