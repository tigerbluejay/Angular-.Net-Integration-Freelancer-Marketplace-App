using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PortfolioItemController(
    IPortfolioItemRepository portfolioRepository,
    IUserRepository userRepository, IMapper mapper) : BaseApiController
{
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePortfolioItem(int id)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (username == null) return Unauthorized();

        var user = await userRepository.GetUserByUsernameAsync(username);
        if (user == null) return Unauthorized();

        var item = await portfolioRepository.GetPortfolioItemByIdAsync(id);

        if (item != null)
        {
            portfolioRepository.DeletePortfolioItem(item);
            await portfolioRepository.SaveAllAsync();
        }

        // Even if item was null, act like it’s gone
        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<PortfolioItemDTO>> CreatePortfolioItem(PortfolioItemCreateDTO dto)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (username == null) return Unauthorized();

        var user = await userRepository.GetUserByUsernameAsync(username);

        if (user == null)
            return NotFound("User not found");

        var portfolioItem = mapper.Map<PortfolioItem>(dto);
        portfolioItem.UserId = user.Id;
        portfolioItem.Created = DateTime.UtcNow;

        var createdItem = await portfolioRepository.CreateAsync(portfolioItem);

        return CreatedAtAction(nameof(GetPortfolioItem), new { id = createdItem.Id }, mapper.Map<PortfolioItemDTO>(createdItem));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PortfolioItemDTO>> GetPortfolioItem(int id)
    {
        var item = await portfolioRepository.GetPortfolioItemByIdAsync(id);
        if (item == null) return NotFound();

        return mapper.Map<PortfolioItemDTO>(item);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdatePortfolioItem(int id, PortfolioItemUpdateDTO dto)
    {
        if (id != dto.Id)
            return BadRequest("ID in URL does not match ID in body");

        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (username == null) return Unauthorized();

        var user = await userRepository.GetUserByUsernameAsync(username);
        if (user == null) return NotFound("User not found");

        var item = await portfolioRepository.GetPortfolioItemByIdAsync(id);
        if (item == null) return NotFound("Portfolio item not found");

        if (item.UserId != user.Id)
            return Forbid(); // Can't update someone else's item

        mapper.Map(dto, item); // Apply updates to the entity

        var success = await portfolioRepository.UpdateAsync(item);

        if (!success)
            return StatusCode(500, "Failed to update portfolio item");

        return NoContent(); // 204
    }
}