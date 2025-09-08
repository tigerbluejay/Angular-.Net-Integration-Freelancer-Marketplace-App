using System.IO.Compression;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ProjectRepository(DataContext context, IMapper mapper) : IProjectRepository
{
    public async Task<Project?> GetProjectByIdAsync(int id)
    {
        return await context.Projects
       .Include(p => p.Skills)
       .Include(p => p.Client)
           .ThenInclude(c => c.Photo) // <-- include client photo
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

    public async Task<PagedList<ProjectBrowseDTO>> GetProjectsAsync(ProjectParams projectParams)
    {
        var query = context.Projects.AsQueryable();

        // By default, show only open (unassigned) projects
        if (!projectParams.IncludeAssigned)
        {
            query = query.Where(p => p.FreelancerUserId == null); // p.IsAssigned => FreelancerUserId.HasValue
        }

        // Skills filtering
        if (projectParams.SkillNames != null && projectParams.SkillNames.Any())
        {
            var skillNames = projectParams.SkillNames
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim().ToLower())
                .ToList();

            if (skillNames.Any())
            {
                if (projectParams.MatchAllSkills)
                {
                    // Require project to contain ALL requested skills (AND)
                    // Translate into repeated Where + Any for each skill to keep things EF-translatable
                    foreach (var s in skillNames)
                    {
                        var skill = s; // avoid closure problems
                        query = query.Where(p => p.Skills.Any(sk => sk.Name.ToLower() == skill));
                    }
                }
                else
                {
                    // Require project to contain ANY of the requested skills (OR)
                    query = query.Where(p => p.Skills.Any(sk => skillNames.Contains(sk.Name.ToLower())));
                }
            }
        }

        query = query.OrderByDescending(p => p.Id);

        // Projection & paging
        var projected = query.ProjectTo<ProjectBrowseDTO>(mapper.ConfigurationProvider);

        return await PagedList<ProjectBrowseDTO>.CreateAsync(projected,
            projectParams.PageNumber, projectParams.PageSize);
    }


    public async Task<PagedList<ProjectDTO>> GetClientProjectsAsync(int clientUserId, ProjectParams projectParams)
    {
        var query = context.Projects
            .Where(p => p.ClientUserId == clientUserId);

        var projected = query.ProjectTo<ProjectDTO>(mapper.ConfigurationProvider);
        return await PagedList<ProjectDTO>.CreateAsync(projected, projectParams.PageNumber, projectParams.PageSize);
    }

    public IQueryable<Project> Query()
{
    return context.Projects.AsQueryable();
}
}