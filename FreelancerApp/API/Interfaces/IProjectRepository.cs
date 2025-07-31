using API.Entities;

namespace API.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetProjectByIdAsync(int id);
    void DeleteProject(Project project);
    Task<bool> SaveAllAsync();
}