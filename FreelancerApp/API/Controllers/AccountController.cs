using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")] // account/register
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
    {
        if (await UserExists(registerDTO.Username)) return BadRequest("Username is taken");

        return Ok();

        /* 
                var user = new AppUser
                {
                    UserName = registerDTO.Username.ToLower(),
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();

                return new UserDTO
                {
                    Username = user.UserName,
                    Token = tokenService.CreateToken(user)
                };

         */
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
    {
        var user = await context.Users
        .FirstOrDefaultAsync(x => x.UserName == loginDTO.Username.ToLower());

        if (user == null) return Unauthorized("Invalid username");

        return new UserDTO
        {
            Username = user.UserName!,
            Token = tokenService.CreateToken(user)

        };
        
    }

    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(x => x.NormalizedUserName == username.ToUpper());
    }
    
}