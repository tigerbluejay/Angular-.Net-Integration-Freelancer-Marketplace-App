using System.IO.Compression;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository(DataContext context, IMapper mapper) : IUserRepository
{
    public async Task<MemberDTO?> GetMemberAsync(string username)
    {
        return await context.Users
            .Where(x => x.UserName == username)
            .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
    {
        return await context.Users
            .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<AppUser?> GetUserByIdAsync(int id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<AppUser?> GetUserByUsernameAsync(string username)
    {
        return await context.Users
        .Include(x => x.Photo)
        .Include(x => x.Skills)
        .Include(x => x.UserRoles).ThenInclude(x => x.Role)
        .Include(x => x.PortfolioItems)
        .SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    { 
        return await context.Users
         .Include(x => x.Photo)
         .Include(x => x.Skills)
         .Include(x => x.UserRoles).ThenInclude(x => x.Role)
         .Include(x => x.PortfolioItems)
         .ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void Update(AppUser user)
    {
        // explicity tell EF that this entity has been modified
        context.Entry(user).State = EntityState.Modified;
    }
}