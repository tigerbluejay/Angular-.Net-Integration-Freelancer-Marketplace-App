using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProjectController(IProjectRepository projectRepository,
IUserRepository userRepository, IMapper mapper, DataContext context) : BaseApiController
{
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        var project = await projectRepository.GetProjectByIdAsync(id);

        if (project == null)
            return NotFound();

        if (project.Client.UserName != userId)
            return Forbid();

        projectRepository.DeleteProject(project);

        if (await projectRepository.SaveAllAsync())
            return NoContent();

        return BadRequest("Failed to delete the project");
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDTO>> CreateProject(ProjectCreateDTO dto)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (username == null) return Unauthorized();

        var user = await userRepository.GetUserByUsernameAsync(username);
        if (user == null) return NotFound("User not found");

        // Lookup skill IDs by names
        var skillIds = await context.Skills
            .Where(s => dto.Skills.Contains(s.Name))
            .Select(s => s.Id)
            .ToListAsync();

        var project = mapper.Map<Project>(dto);
        project.ClientUserId = user.Id;

        var createdProject = await projectRepository.CreateAsync(project, skillIds);

        var projectDto = mapper.Map<ProjectDTO>(createdProject);

        return CreatedAtAction(nameof(GetProjectById), new { id = createdProject.Id }, projectDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectBrowseDTO>> GetProjectById(int id)
    {
        var project = await projectRepository.GetProjectByIdAsync(id);
        if (project == null)
            return NotFound();

        var projectDto = mapper.Map<ProjectBrowseDTO>(project);
        return Ok(projectDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProjectDTO>> UpdateProject(int id, ProjectUpdateDTO dto)
    {
        if (id != dto.Id)
            return BadRequest("Project ID mismatch");

        var existingProject = await projectRepository.GetProjectByIdAsync(id);
        if (existingProject == null)
            return NotFound();

        // Lookup skill IDs by names
        var skillIds = await context.Skills
            .Where(s => dto.Skills.Contains(s.Name))
            .Select(s => s.Id)
            .ToListAsync();

        // Map non-skills properties from DTO to existing entity
        mapper.Map(dto, existingProject);

        var updatedProject = await projectRepository.UpdateAsync(existingProject, skillIds);

        if (updatedProject == null)
            return NotFound();

        var projectDto = mapper.Map<ProjectDTO>(updatedProject);
        return Ok(projectDto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectBrowseDTO>>> GetProjects([FromQuery] ProjectParams projectParams)
    {
        var projects = await projectRepository.GetProjectsAsync(projectParams);

        Response.AddPaginationHeader(projects);

        return Ok(projects);
    }

    [HttpPatch("assign-proposals-with-projects")]
    public async Task<IActionResult> PatchAssignProposalswithProjects(
    [FromBody] ProposalWithProjectAssignCombinedDTO patchDto)
{
    var project = await context.Projects.FindAsync(patchDto.Project.Id);
    var proposal = await context.Proposals.FindAsync(patchDto.Proposal.Id);

    if (project == null || proposal == null)
        return NotFound();

    if (patchDto.Project.FreelancerUserId != null)
        project.FreelancerUserId = patchDto.Project.FreelancerUserId;

    if (patchDto.Proposal.IsAccepted != null)
        proposal.IsAccepted = patchDto.Proposal.IsAccepted;

    await context.SaveChangesAsync();
    return NoContent();
}      

}