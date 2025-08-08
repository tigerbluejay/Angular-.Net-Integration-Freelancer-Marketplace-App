using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ProjectRepository(DataContext context) : IProjectRepository
{
    public async Task<Project?> GetProjectByIdAsync(int id)
    {
        return await context.Projects
        .Include(p => p.Skills)
        .Include(p => p.Client)
        .Include(p => p.Photo)
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

    public async Task<IEnumerable<Project>> GetProjectsAsync()
    {
        return await context.Projects
            .Include(p => p.Skills)
            .Include(P => P.Photo)
            .ToListAsync();
    }

    public async Task<Project> CreateAsync(Project project, List<int> skillIds)
    {
        var skills = await context.Skills
            .Where(s => skillIds.Contains(s.Id))
            .ToListAsync();

        project.Skills = skills;

        context.Projects.Add(project);
        await context.SaveChangesAsync();

        return project;
    }

    public async Task<Project?> UpdateAsync(Project project, List<int> skillIds)
    {
        var existingProject = await context.Projects
            .Include(p => p.Skills)
            .FirstOrDefaultAsync(p => p.Id == project.Id);

        if (existingProject == null)
            return null;

        // Update scalar properties
        existingProject.Title = project.Title;
        existingProject.Description = project.Description;

        // Update skills: clear current and assign new
        var newSkills = await context.Skills
            .Where(s => skillIds.Contains(s.Id))
            .ToListAsync();

        existingProject.Skills.Clear();
        foreach (var skill in newSkills)
            existingProject.Skills.Add(skill);

        await context.SaveChangesAsync();

        return existingProject;
    }

    
}