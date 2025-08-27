namespace API.DTOs
{
    public class ProposalDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int ProjectId { get; set; }
    public int FreelancerUserId { get; set; }
    public int ClientUserId { get; set; }

    public string? FreelancerUsername { get; set; }
    public string? ClientUsername { get; set; }
    public string? ProjectTitle { get; set; }

    // Only expose the URL; keep PublicId internal
    public string? PhotoUrl { get; set; }

    public bool? IsAccepted { get; set; }
}
}