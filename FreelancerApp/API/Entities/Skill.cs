// Hello from the hook test, 2026-07-28
using API.Entities;

public class Skill
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public ICollection<AppUser> Users { get; set; } = [];

    public ICollection<Project> Projects { get; set; } = [];
}