using Microsoft.EntityFrameworkCore;
using TourPlanner.Models;

namespace TourPlanner.DAL;

public class TourDbContext : DbContext
{
    public TourDbContext(DbContextOptions<TourDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<TourLog> TourLogs => Set<TourLog>();
    public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

        modelBuilder.Entity<Tour>().HasKey(t => t.Id);
        modelBuilder.Entity<TourLog>().HasKey(t => t.Id);

        modelBuilder.Entity<UserAchievement>().HasKey(a => a.Id);
        modelBuilder.Entity<UserAchievement>()
            .HasIndex(a => new { a.UserId, a.AchievementKey })
            .IsUnique();

        // Cascade delete
        modelBuilder.Entity<Tour>()
            .HasMany(t => t.TourLogs)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}