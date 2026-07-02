using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourPlanner.BL.Achievements;
using TourPlanner.BL.Interfaces;
using TourPlanner.BL.Models;
using TourPlanner.DAL;
using TourPlanner.Models;

namespace TourPlanner.BL.Services;

public class AchievementService : IAchievementService
{
    private readonly TourDbContext _context;
    private readonly ILogger<AchievementService> _logger;

    public AchievementService(TourDbContext context, ILogger<AchievementService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<AchievementStatusDto>> GetForUserAsync(Guid userId)
    {
        await EvaluateAsync(userId);

        var unlocked = await _context.UserAchievements
            .Where(a => a.UserId == userId)
            .ToDictionaryAsync(a => a.AchievementKey, a => a.UnlockedAt);

        return AchievementCatalog.All.Select(def => new AchievementStatusDto
        {
            Key = def.Key,
            Name = def.Name,
            Description = def.Description,
            Icon = def.Icon,
            Unlocked = unlocked.ContainsKey(def.Key),
            UnlockedAt = unlocked.TryGetValue(def.Key, out var date) ? date : null
        }).ToList();
    }

    public async Task EvaluateAsync(Guid userId)
    {
        var tours = await _context.Tours
            .Include(t => t.TourLogs)
            .Where(t => t.UserId == userId)
            .ToListAsync();

        var alreadyUnlockedKeys = await _context.UserAchievements
            .Where(a => a.UserId == userId)
            .Select(a => a.AchievementKey)
            .ToListAsync();

        var newlyUnlocked = new List<UserAchievement>();

        foreach (var def in AchievementCatalog.All)
        {
            if (alreadyUnlockedKeys.Contains(def.Key)) continue;

            if (def.IsSatisfied(tours))
            {
                newlyUnlocked.Add(new UserAchievement
                {
                    UserId = userId,
                    AchievementKey = def.Key
                });
                _logger.LogInformation("Achievement freigeschaltet fuer User {UserId}: {Key}", userId, def.Key);
            }
        }

        if (newlyUnlocked.Any())
        {
            _context.UserAchievements.AddRange(newlyUnlocked);
            await _context.SaveChangesAsync();
        }
    }
}