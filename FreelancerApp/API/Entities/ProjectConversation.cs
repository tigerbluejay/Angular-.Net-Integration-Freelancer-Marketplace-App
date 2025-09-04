
namespace API.Entities;

public class ProjectConversation
{
    public int Id { get; set; }

    // Foreign keys
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public int ClientId { get; set; }
    public AppUser Client { get; set; } = null!;

    public int FreelancerId { get; set; }
    public AppUser Freelancer { get; set; } = null!;

    // Navigation
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}