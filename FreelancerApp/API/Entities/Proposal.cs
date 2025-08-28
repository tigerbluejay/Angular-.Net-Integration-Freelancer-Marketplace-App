using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

public class Proposal
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public decimal Bid { get; set; }

    // Proposal Status (null = pending, true = accepted, false = rejected)
    public bool? IsAccepted { get; set; }

    // Foreign key & navigation for Freelancer
    public int FreelancerUserId { get; set; }
    public AppUser Freelancer { get; set; }

    // Foreign key & navigation for Client (redundant, but useful for quick lookups)
    public int ClientUserId { get; set; }
    public AppUser Client { get; set; }

    // Foreign key & navigation for Project
    public int ProjectId { get; set; }
    public Project Project { get; set; }

    // Foreign key & navigation for Photo
    public int? PhotoId { get; set; }
    public Photo? Photo { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;
}