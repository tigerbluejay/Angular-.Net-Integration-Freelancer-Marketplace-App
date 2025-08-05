using API.Entities;

namespace API.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetProjectByIdAsync(int id);
    void DeleteProject(Project project);
    Task<bool> SaveAllAsync();
    Task<Project> CreateAsync(Project project, List<int> skillIds);
    Task<Project?> UpdateAsync(Project project, List<int> skillIds);
    Task<IEnumerable<Project>> GetProjectsAsync();

}