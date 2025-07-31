using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ProjectRepository(DataContext context) : IProjectRepository
{
    public async Task<Project?> GetProjectByIdAsync(int id)
    {
        return await context.Projects
        .Include(p => p.Client)
        .FirstOrDefaultAsync(p => p.Id == id);
    }

    public void DeleteProject(Project project)
    {
        context.Projects.Remove(project);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}