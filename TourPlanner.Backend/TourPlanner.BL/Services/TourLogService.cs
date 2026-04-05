using Microsoft.EntityFrameworkCore;
using TourPlanner.BL.Interfaces;
using TourPlanner.DAL;
using TourPlanner.Models;

namespace TourPlanner.BL.Services;

public class TourLogService : ITourLogService
{
    private readonly TourDbContext _context;

    public TourLogService(TourDbContext context)
    {
        _context = context;
    }

    public async Task<List<TourLog>> GetByTourIdAsync(Guid tourId)
    {
        var tour = await _context.Tours
            .Include(t => t.TourLogs)
            .FirstOrDefaultAsync(t => t.Id == tourId);

        return tour?.TourLogs?.ToList() ?? new List<TourLog>();
    }

    public async Task<TourLog?> GetByIdAsync(Guid id)
    {
        return await _context.TourLogs.FindAsync(id);
    }

    public async Task<TourLog> CreateAsync(Guid tourId, TourLog log)
    {
        var tour = await _context.Tours.FindAsync(tourId);
        if (tour == null) throw new Exception("Tour not found");

        log.DateTime = DateTime.UtcNow;
        tour.TourLogs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<TourLog?> UpdateAsync(Guid id, TourLog log)
    {
        var existing = await _context.TourLogs.FindAsync(id);
        if (existing == null) return null;

        existing.Comment = log.Comment;
        existing.Difficulty = log.Difficulty;
        existing.TotalDistance = log.TotalDistance;
        existing.TotalTimeHours = log.TotalTimeHours;
        existing.Rating = log.Rating;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var log = await _context.TourLogs.FindAsync(id);
        if (log == null) return false;

        _context.TourLogs.Remove(log);
        await _context.SaveChangesAsync();
        return true;
    }
}