using TourPlanner.Models;

namespace TourPlanner.BL.Achievements;

public class AchievementDefinition
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public required Func<List<Tour>, bool> IsSatisfied { get; set; }
}