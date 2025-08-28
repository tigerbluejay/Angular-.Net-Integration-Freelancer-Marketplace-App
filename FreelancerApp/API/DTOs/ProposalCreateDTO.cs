using System.ComponentModel.DataAnnotations;

public class ProposalCreateDTO
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
    public decimal Bid { get; set; } // NEW FIELD

    [Required]
    public int ProjectId { get; set; }

    [Required]
    public int FreelancerUserId { get; set; }

    [Required]
    public int ClientUserId { get; set; }

    // Optional: include file
    public IFormFile? PhotoFile { get; set; }
}