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

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(IUserRepository userRepository, IProjectRepository projectRepository,
    IPortfolioItemRepository portfolioItemRepository, IMapper mapper, DataContext context) : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
    {
        var users = await userRepository.GetMembersAsync();

        return Ok(users);
    }

    [HttpGet("{username}")] // /api/users/jose
    public async Task<ActionResult<MemberDTO>> GetUser(string username)
    {
        var user = await userRepository.GetMemberAsync(username);

        if (user == null) return NotFound();

        return user;
    }

    [Authorize]
    [HttpPut("update-profile")]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(username))
            return BadRequest("No username found in token");

        var user = await userRepository.GetUserByUsernameAsync(username);

        if (user == null)
            return NotFound("User not found");

        mapper.Map(memberUpdateDTO, user);

        // Force EF to detect changes or mark as modified
        context.Entry(user).State = EntityState.Modified;

        if (await userRepository.SaveAllAsync())
            return NoContent();

        return BadRequest("Failed to update the user");
    }

    [HttpGet("{username}/projects")]
    public async Task<ActionResult> GetClientProjects(string username, [FromQuery] ProjectParams projectParams)
    {
        var user = await userRepository.GetUserByUsernameAsync(username);
        if (user == null) return NotFound();

        var projects = await projectRepository.GetClientProjectsAsync(user.Id, projectParams);
        Response.AddPaginationHeader(projects);
        return Ok(projects);
    }

    [HttpGet("{username}/portfolio")]
    public async Task<ActionResult> GetFreelancerPortfolio(string username, [FromQuery] PortfolioParams portfolioParams)
    {
        var user = await userRepository.GetUserByUsernameAsync(username);
        if (user == null) return NotFound();

        var portfolio = await portfolioItemRepository.GetFreelancerPortfolioAsync(user.Id, portfolioParams);
        Response.AddPaginationHeader(portfolio);
        return Ok(portfolio);
    }

}