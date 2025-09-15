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
public class UsersController(IUnitOfWork unitOfWork, IMapper mapper,
DataContext context) : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
    {
        var users = await unitOfWork.UserRepository.GetMembersAsync();

        return Ok(users);
    }

    [HttpGet("{username}")] // /api/users/jose
    public async Task<ActionResult<MemberDTO>> GetUser(string username)
    {
        var user = await unitOfWork.UserRepository.GetMemberAsync(username);

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

        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);

        if (user == null)
            return NotFound("User not found");

        mapper.Map(memberUpdateDTO, user);

        // Force EF to detect changes or mark as modified
        context.Entry(user).State = EntityState.Modified;

        if (await unitOfWork.Complete())
            return NoContent();

        return BadRequest("Failed to update the user");
    }

    [HttpGet("{username}/projects")]
    public async Task<ActionResult> GetClientProjects(string username, [FromQuery] ProjectParams projectParams)
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        if (user == null) return NotFound();

        var projects = await unitOfWork.ProjectRepository.GetClientProjectsAsync(user.Id, projectParams);
        Response.AddPaginationHeader(projects);
        return Ok(projects);
    }

    [HttpGet("{username}/portfolio")]
    public async Task<ActionResult> GetFreelancerPortfolio(string username, [FromQuery] PortfolioParams portfolioParams)
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        if (user == null) return NotFound();

        var portfolio = await unitOfWork.PortfolioItemRepository.GetFreelancerPortfolioAsync(user.Id, portfolioParams);
        Response.AddPaginationHeader(portfolio);
        return Ok(portfolio);
    }

    // GET: api/users/admin/?pageNumber=1&pageSize=10
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PagedList<UserAdminDTO>>> GetUsers([FromQuery] UserParams userParams)
    {
        var users = await unitOfWork.UserRepository.GetUsersAsync(userParams);

        var userDtos = mapper.Map<IEnumerable<UserAdminDTO>>(users);

        // If you already have a PagedList<T>, you can map directly and keep metadata
        var pagedResult = new PagedList<UserAdminDTO>(
            userDtos.ToList(),
            users.TotalCount,
            users.CurrentPage,
            users.PageSize
        );

        return Ok(pagedResult);
    }

    // PATCH: api/users/{id}/disable
    [HttpPatch("{id}/disable")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DisableUser(int id)
    {
        var user = await unitOfWork.UserRepository.GetUserByIdAsync(id);
        if (user == null) return NotFound();

        if (user.IsAccountDisabled)
            return BadRequest("User account is already disabled.");

        user.IsAccountDisabled = true;
        await unitOfWork.UserRepository.UpdateUserAsync(user);

        return NoContent();
    }

    // PATCH: api/users/{id}/enable
    [HttpPatch("{id}/enable")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EnableUser(int id)
    {
        var user = await unitOfWork.UserRepository.GetUserByIdAsync(id);
        if (user == null) return NotFound();

        if (!user.IsAccountDisabled)
            return BadRequest("User account is already enabled.");

        user.IsAccountDisabled = false;
        await unitOfWork.UserRepository.UpdateUserAsync(user);

        return NoContent();
    }

}