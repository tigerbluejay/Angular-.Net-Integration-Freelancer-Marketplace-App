using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;

namespace API.Interfaces;

public interface IUserRepository
{
    void Update(AppUser user);
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByUsernameAsync(string username);
    Task<IEnumerable<MemberDTO>> GetMembersAsync();
    Task<MemberDTO?> GetMemberAsync(string username);
    Task UpdateUserAsync(AppUser user);
    Task<PagedList<AppUser>> GetUsersAsync(UserParams userParams);

}