using Microsoft.EntityFrameworkCore;
using TourPlanner.BL.Exceptions;
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

    public async Task<TourLog?> GetByIdAsync(Guid tourId, Guid id)
    {
        return await GetOwnedLogAsync(tourId, id);
    }

    // Zentrale Stelle fuer den Besitz-Check (Log gehoert zu dieser Tour) - verhindert IDOR.
    // Einzige Stelle, die den TourId-Shadow-Property-Namen kennen muss.
    private Task<TourLog?> GetOwnedLogAsync(Guid tourId, Guid id)
    {
        return _context.TourLogs
            .FirstOrDefaultAsync(l => l.Id == id && EF.Property<Guid?>(l, "TourId") == tourId);
    }

    public async Task<TourLog> CreateAsync(Guid tourId, TourLog log)
    {
        var tour = await _context.Tours.Include(t => t.TourLogs).FirstOrDefaultAsync(t => t.Id == tourId);
        if (tour == null) throw new EntityNotFoundException($"Tour {tourId} not found");

        log.DateTime = DateTime.UtcNow;
        // EF Core thinks this is an existing entity because Guid is initialized to NewGuid()
        _context.Entry(log).State = EntityState.Added;
        tour.TourLogs.Add(log);
        
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<TourLog?> UpdateAsync(Guid tourId, Guid id, TourLog log)
    {
        var existing = await GetOwnedLogAsync(tourId, id);
        if (existing == null) return null;

        existing.Comment = log.Comment;
        existing.Difficulty = log.Difficulty;
        existing.TotalDistance = log.TotalDistance;
        existing.TotalTimeHours = log.TotalTimeHours;
        existing.Rating = log.Rating;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid tourId, Guid id)
    {
        var log = await GetOwnedLogAsync(tourId, id);
        if (log == null) return false;

        _context.TourLogs.Remove(log);
        await _context.SaveChangesAsync();
        return true;
    }
}