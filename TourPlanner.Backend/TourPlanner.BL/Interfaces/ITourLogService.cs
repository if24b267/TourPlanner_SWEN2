using TourPlanner.Models;

namespace TourPlanner.BL.Interfaces;

public interface ITourLogService
{
    Task<List<TourLog>> GetByTourIdAsync(Guid tourId);
    Task<TourLog?> GetByIdAsync(Guid id);
    Task<TourLog> CreateAsync(Guid tourId, TourLog log);
    Task<TourLog?> UpdateAsync(Guid id, TourLog log);
    Task<bool> DeleteAsync(Guid id);
}