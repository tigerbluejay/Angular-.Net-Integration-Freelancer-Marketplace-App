public class ProjectBrowseDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsAssigned { get; set; }
    public List<string> SkillNames { get; set; } = new();
    public int ClientUserId { get; set; }
    public string ClientUserName { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string ClientPhotoUrl { get; set; }
}