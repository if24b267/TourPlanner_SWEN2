using TourPlanner.Models;

namespace TourPlanner.BL.Interfaces;

public interface ITourService
{
    Task<List<Tour>> GetAllAsync();
    Task<Tour?> GetByIdAsync(Guid id);
    Task<Tour> CreateAsync(Tour tour);
    Task<Tour?> UpdateAsync(Guid id, Tour tour);
    Task<bool> DeleteAsync(Guid id);
    Task<List<Tour>> SearchAsync(string searchText);
}