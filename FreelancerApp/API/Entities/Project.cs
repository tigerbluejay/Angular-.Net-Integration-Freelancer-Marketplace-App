using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

public class Project
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public bool IsAssigned => FreelancerUserId.HasValue;

    // Skills (you might split this into a separate join table later)
    public ICollection<Skill> Skills { get; set; } = [];

    // Foreign key and navigation for Client
    public int ClientUserId { get; set; }
    public AppUser Client { get; set; }

    // Foreign key and navigation for Freelancer (nullable)
    public int? FreelancerUserId { get; set; }
    public AppUser? Freelancer { get; set; }

    // Foreign key and navigation for Photo
    public int? PhotoId { get; set; }
    public Photo? Photo { get; set; }
    public ICollection<Proposal> Proposals { get; set; } = [];

    // Conversations associated with this project
    public ICollection<ProjectConversation> Conversations { get; set; } = [];
}