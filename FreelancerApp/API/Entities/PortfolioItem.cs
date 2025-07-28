namespace API.Entities;

public class PortfolioItem
{
    public int Id { get; set; }

    public string PhotoUrl { get; set; } = string.Empty;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    // Foreign key to AppUser
    public int UserId { get; set; }

    // Navigation property
    public AppUser User { get; set; } = null!;
}