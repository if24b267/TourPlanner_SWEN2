using Microsoft.EntityFrameworkCore;
using TourPlanner.BL.Interfaces;
using TourPlanner.DAL;
using TourPlanner.Models;

namespace TourPlanner.BL.Services;

public class TourService : ITourService
{
    private readonly TourDbContext _context;

    public TourService(TourDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tour>> GetAllAsync()
    {
        return await _context.Tours
            .Include(t => t.TourLogs)
            .ToListAsync();
    }

    public async Task<Tour?> GetByIdAsync(Guid id)
    {
        return await _context.Tours
            .Include(t => t.TourLogs)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Tour> CreateAsync(Tour tour)
    {
        tour.Id = Guid.NewGuid();

        // Mock route data (replace with OpenRouteService later)
        if (tour.TourDistance == 0)
        {
            tour.TourDistance = new Random().Next(5, 100);
            tour.EstimatedTimeHours = tour.TourDistance / 15; // avg 15 km/h
        }

        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();
        return tour;
    }

    public async Task<Tour?> UpdateAsync(Guid id, Tour tour)
    {
        var existing = await _context.Tours.FindAsync(id);
        if (existing == null) return null;

        existing.Name = tour.Name;
        existing.TourDescription = tour.TourDescription;
        existing.From = tour.From;
        existing.To = tour.To;
        existing.TransportType = tour.TransportType;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var tour = await _context.Tours.FindAsync(id);
        if (tour == null) return false;

        _context.Tours.Remove(tour);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Tour>> SearchAsync(string searchText)
    {
        var lower = searchText.ToLower();
        return await _context.Tours
            .Include(t => t.TourLogs)
            .Where(t =>
                t.Name.ToLower().Contains(lower) ||
                t.From.ToLower().Contains(lower) ||
                t.To.ToLower().Contains(lower) ||
                t.TourDescription.ToLower().Contains(lower) ||
                t.TourLogs.Any(l => l.Comment.ToLower().Contains(lower)))
            .ToListAsync();
    }
}