namespace API.Helpers;

public class ProjectParams
{
    private const int MaxPageSize = 12;

    public int PageNumber { get; set; } = 1;

    private int _pageSize = 6;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

    // List of skill names to filter by (e.g. ["c#", "angular"])
    public List<string> SkillNames { get; set; } = new();

    // If false (default) only return projects that are NOT assigned (FreelancerUserId == null)
    public bool IncludeAssigned { get; set; } = false;

    // If true, require the project to contain ALL skills in SkillNames (AND).
    // If false (default), require the project to contain ANY of the SkillNames (OR).
    public bool MatchAllSkills { get; set; } = false;


}