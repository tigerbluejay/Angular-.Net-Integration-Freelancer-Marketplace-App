namespace API.DTOs;

public class ProjectConversationDTO
{
    public int ConversationId { get; set; }
    public int ProjectId { get; set; }
    public string ProjectTitle { get; set; } = null!;

    public int ClientId { get; set; }
    public string ClientUsername { get; set; } = null!;
    public string ClientPhotoUrl { get; set; } = null!;

    public int FreelancerId { get; set; }
    public string FreelancerUsername { get; set; } = null!;
    public string FreelancerPhotoUrl { get; set; } = null!;

    public string LastMessage { get; set; } = null!;
    public DateTime LastMessageSent { get; set; }
    public int UnreadCount { get; set; }
}