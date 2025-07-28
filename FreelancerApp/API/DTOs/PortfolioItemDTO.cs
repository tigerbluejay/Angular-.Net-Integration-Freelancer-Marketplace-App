namespace API.DTOs;

public class PortfolioItemDTO
{
    public int Id { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime Created { get; set; }
}