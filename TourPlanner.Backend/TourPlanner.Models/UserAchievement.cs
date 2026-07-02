namespace TourPlanner.Models;

public class UserAchievement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string AchievementKey { get; set; } = string.Empty;
    public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
}