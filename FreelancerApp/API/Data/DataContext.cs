using Microsoft.EntityFrameworkCore;
using API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace API.Data;

public class DataContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, int,
IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>,
IdentityUserToken<int>>(options)
{
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Skill> Skills { get; set;}
    public DbSet<PortfolioItem> PortfolioItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        builder.Entity<AppRole>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        builder.Entity<AppUser>()
            .HasOne(u => u.Photo)
            .WithOne(p => p.User)
            .HasForeignKey<Photo>(p => p.UserId);

        builder.Entity<AppUser>()
            .HasMany(u => u.Skills)
            .WithMany(s => s.Users)
            .UsingEntity<Dictionary<string, object>>(
                "UserSkill",
                j => j.HasOne<Skill>().WithMany().HasForeignKey("SkillId"),
                j => j.HasOne<AppUser>().WithMany().HasForeignKey("AppUserId"));

        builder.Entity<AppUser>()
            .HasMany(u => u.PortfolioItems)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId);
    }

}