namespace API.Entities;

public class PortfolioItem
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    // Foreign key to AppUser
    public int UserId { get; set; }

    // Navigation property
    public AppUser User { get; set; } = null!;

        // Foreign key and navigation for Photo
    public int? PhotoId { get; set; }
    public Photo? Photo { get; set; }

}