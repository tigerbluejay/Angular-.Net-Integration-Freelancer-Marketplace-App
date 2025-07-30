using System.Security.Claims;
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
        // foreach (var claim in User.Claims)
        // {
        //     Console.WriteLine($"CLAIM: {claim.Type} = {claim.Value}");
        // }

        // 1. Get username from token
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (username == null) return Unauthorized();

        // 2. Get the portfolio item
        var item = await portfolioRepository.GetPortfolioItemByIdAsync(id);
        if (item == null) return NotFound("Portfolio item not found");

        // 3. Get the current user (with portfolio items loaded)
        var user = await userRepository.GetUserByUsernameAsync(username);
        if (user == null) return Unauthorized();

        portfolioRepository.DeletePortfolioItem(item);

        if (await portfolioRepository.SaveAllAsync())
            return NoContent();

        return BadRequest("Failed to delete the portfolio item.");
    }
}