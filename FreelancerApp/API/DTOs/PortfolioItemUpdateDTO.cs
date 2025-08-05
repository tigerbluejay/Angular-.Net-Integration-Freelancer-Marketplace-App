using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class PortfolioItemUpdateDTO
{
    [Required]
    public int Id { get; set; }

    public string? PhotoUrl { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }
}