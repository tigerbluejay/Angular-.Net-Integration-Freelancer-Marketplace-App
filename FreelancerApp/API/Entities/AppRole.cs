// Hello from the hook test, 2026-07-28
using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppRole : IdentityRole<int>
{
    public ICollection<AppUserRole> UserRoles { get; set; } = [];
}