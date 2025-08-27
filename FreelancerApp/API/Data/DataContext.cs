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
    public DbSet<Skill> Skills { get; set; }
    public DbSet<PortfolioItem> PortfolioItems { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Proposal> Proposals { get; set; } // ← add this

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

        builder.Entity<Project>()
            .HasOne(p => p.Client)
            .WithMany(u => u.ClientProjects)
            .HasForeignKey(p => p.ClientUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Project>()
            .HasOne(p => p.Freelancer)
            .WithMany(u => u.FreelancerProjects)
            .HasForeignKey(p => p.FreelancerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Project>()
            .HasMany(p => p.Skills)
            .WithMany(s => s.Projects)
            .UsingEntity(j => j.ToTable("ProjectSkills"));

        builder.Entity<Project>()
            .HasOne(p => p.Photo)
            .WithOne(photo => photo.Project)
            .HasForeignKey<Project>(p => p.PhotoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PortfolioItem>()
            .HasOne(p => p.Photo)
            .WithOne(photo => photo.PortfolioItem)
            .HasForeignKey<PortfolioItem>(p => p.PhotoId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- Proposal ↔ Freelancer (AppUser) ---
        builder.Entity<Proposal>()
            .HasOne(p => p.Freelancer)
            .WithMany(u => u.FreelancerProposals)
            .HasForeignKey(p => p.FreelancerUserId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

        // --- Proposal ↔ Client (AppUser) ---
        builder.Entity<Proposal>()
            .HasOne(p => p.Client)
            .WithMany(u => u.ClientProposals)
            .HasForeignKey(p => p.ClientUserId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

        // --- Proposal ↔ Project ---
        builder.Entity<Proposal>()
            .HasOne(p => p.Project)
            .WithMany(pr => pr.Proposals)
            .HasForeignKey(p => p.ProjectId)
            .OnDelete(DeleteBehavior.Cascade); // Delete proposals if project is deleted

        // --- Proposal ↔ Photo (One-to-One) ---
        builder.Entity<Proposal>()
            .HasOne(p => p.Photo)
            .WithOne(ph => ph.Proposal)
            .HasForeignKey<Proposal>(p => p.PhotoId)
            .OnDelete(DeleteBehavior.SetNull); // Keep proposal but nullify photo if photo deleted
    }

}