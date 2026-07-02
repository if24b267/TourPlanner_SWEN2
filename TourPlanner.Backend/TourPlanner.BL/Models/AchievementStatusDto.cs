namespace TourPlanner.BL.Models;

public class AchievementStatusDto
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool Unlocked { get; set; }
    public DateTime? UnlockedAt { get; set; }
}