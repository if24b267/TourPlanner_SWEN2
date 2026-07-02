using TourPlanner.Models;

namespace TourPlanner.BL.Interfaces;

public interface ITourService
{
    Task<List<Tour>> GetAllForUserAsync(Guid userId);
    Task<Tour?> GetByIdAsync(Guid id, Guid userId);
    Task<Tour?> GetByIdRawAsync(Guid id);
    Task<Tour> CreateAsync(Tour tour);
    Task<Tour?> UpdateAsync(Guid id, Tour tour, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<List<Tour>> SearchAsync(string searchText, Guid userId);
    Task UpdateImagePathAsync(Guid id, string? imagePath, Guid userId);
}