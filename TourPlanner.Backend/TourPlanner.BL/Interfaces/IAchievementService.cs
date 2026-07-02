using TourPlanner.BL.Models;

namespace TourPlanner.BL.Interfaces;

public interface IAchievementService
{
    Task<List<AchievementStatusDto>> GetForUserAsync(Guid userId);
    Task EvaluateAsync(Guid userId);
}