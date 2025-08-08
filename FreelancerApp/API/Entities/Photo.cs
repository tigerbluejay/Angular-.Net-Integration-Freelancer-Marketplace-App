using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("Photos")]
public class Photo
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public string? PublicId { get; set; }

    // Navigation for user photo (optional one-to-one)
    public AppUser? User { get; set; }
    public int? UserId { get; set; }

    // Navigation for project (optional one-to-one)
    public Project? Project { get; set; }
    // Navigation for portfolio item (optional one-to-one)
    public PortfolioItem? PortfolioItem { get; set; }
}