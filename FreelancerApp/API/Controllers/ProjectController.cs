using System.Security.Claims;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProjectController(IProjectRepository projectRepository) : BaseApiController
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
}