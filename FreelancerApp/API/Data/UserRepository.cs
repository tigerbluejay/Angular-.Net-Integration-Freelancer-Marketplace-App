using System.IO.Compression;
using API.DTOs;
using API.Entities;
using API.Helpers;
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
        .Include(x => x.ClientProjects)
        .Include(x => x.FreelancerProjects)
        .SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await context.Users
         .Include(x => x.Photo)
         .Include(x => x.Skills)
         .Include(x => x.UserRoles).ThenInclude(x => x.Role)
         .Include(x => x.PortfolioItems)
         .Include(x => x.ClientProjects)
         .Include(x => x.FreelancerProjects)
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

    public async Task UpdateUserAsync(AppUser user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task<PagedList<AppUser>> GetUsersAsync(UserParams userParams)
    {
        var query = context.Users
        .Include(u => u.Photo)
        .Include(u => u.Skills)
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .AsQueryable();

        // Order by the first role alphabetically, then by LastActive as a secondary sort
        query = query
            .OrderBy(u => u.UserRoles.Select(ur => ur.Role.Name).FirstOrDefault())
            .ThenByDescending(u => u.LastActive);

        return await PagedList<AppUser>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
    }
}