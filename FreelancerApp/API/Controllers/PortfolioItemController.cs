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
}