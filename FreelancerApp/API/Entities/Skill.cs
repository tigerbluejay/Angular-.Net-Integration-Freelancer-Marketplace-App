using API.Entities;

public class Skill
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public ICollection<AppUser> Users { get; set; } = [];

    public ICollection<Project> Projects { get; set; } = [];
}