using Microsoft.EntityFrameworkCore;
using TourPlanner.Models;

namespace TourPlanner.DAL;

public class TourDbContext : DbContext
{
    public TourDbContext(DbContextOptions<TourDbContext> options) : base(options) { }

    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<TourLog> TourLogs => Set<TourLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tour>().HasKey(t => t.Id);
        modelBuilder.Entity<TourLog>().HasKey(t => t.Id);

        // Cascade delete
        modelBuilder.Entity<Tour>()
            .HasMany(t => t.TourLogs)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}