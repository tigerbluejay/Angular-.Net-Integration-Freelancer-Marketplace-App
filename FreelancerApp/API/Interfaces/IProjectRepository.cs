using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetProjectByIdAsync(int id);
    void DeleteProject(Project project);
    Task<Project> CreateAsync(Project project, List<int> skillIds);
    Task<Project?> UpdateAsync(Project project, List<int> skillIds);
    Task<IEnumerable<Project>> GetProjectsAsync();
    Task<PagedList<ProjectBrowseDTO>> GetProjectsAsync(ProjectParams projectParams);
    Task<PagedList<ProjectDTO>> GetClientProjectsAsync(int clientUserId, ProjectParams projectParams);
    public IQueryable<Project> Query();

}