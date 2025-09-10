using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Components.Forms.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, UserManager<AppUser> userManager,
 ITokenService tokenService, IMapper mapper) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
    {
        var user = mapper.Map<AppUser>(registerDTO);
        user.UserName = registerDTO.Username.ToLower();

        var result = await userManager.CreateAsync(user, registerDTO.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // Assign the role
        await userManager.AddToRoleAsync(user, registerDTO.Role);

        return new UserDTO
        {
            Username = user.UserName,
            Token = await tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            IsAccountDisabled = user.IsAccountDisabled
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
    {
        var user = await userManager.Users
            .Include(p => p.Photo)
            .FirstOrDefaultAsync(x => x.UserName == loginDTO.Username.ToLower());

        if (user == null) return Unauthorized("Invalid username");

        // Check password
        var passwordValid = await userManager.CheckPasswordAsync(user, loginDTO.Password);
        if (!passwordValid)
            return Unauthorized("Invalid password");

        return new UserDTO
        {
            Username = user.UserName!,
            KnownAs = user.KnownAs,
            Token = await tokenService.CreateToken(user),
            PhotoUrl = user.Photo?.Url,
            IsAccountDisabled = user.IsAccountDisabled
        };
    }
    private async Task<bool> UserExists(string username)
    {
        // return await context.Users.AnyAsync(x => x.NormalizedUserName == username.ToUpper());
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());

    }

}