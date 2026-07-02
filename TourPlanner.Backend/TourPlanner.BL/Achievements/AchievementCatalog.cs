using TourPlanner.Models;

namespace TourPlanner.BL.Achievements;

public static class AchievementCatalog
{
    public static readonly List<AchievementDefinition> All = new()
    {
        new AchievementDefinition
        {
            Key = "first_tour",
            Name = "Erste Schritte",
            Description = "Lege deine erste Tour an.",
            Icon = "🚀",
            IsSatisfied = tours => tours.Any()
        },
        new AchievementDefinition
        {
            Key = "first_log",
            Name = "Log-Buch eroeffnet",
            Description = "Erfasse deinen ersten Tour-Log.",
            Icon = "📖",
            IsSatisfied = tours => tours.SelectMany(t => t.TourLogs).Any()
        },
        new AchievementDefinition
        {
            Key = "century_rider",
            Name = "Hundert Kilometer",
            Description = "Erreiche insgesamt 100 km an geloggter Distanz.",
            Icon = "💯",
            IsSatisfied = tours => tours.SelectMany(t => t.TourLogs).Sum(l => l.TotalDistance) >= 100
        },
        new AchievementDefinition
        {
            Key = "endurance",
            Name = "Ausdauersportler",
            Description = "Absolviere eine Tour mit mindestens 5 Stunden Dauer.",
            Icon = "🏃",
            IsSatisfied = tours => tours.SelectMany(t => t.TourLogs).Any(l => l.TotalTimeHours >= 5)
        },
        new AchievementDefinition
        {
            Key = "mountain_goat",
            Name = "Bergziege",
            Description = "Meistere eine Tour mit Schwierigkeit 9 oder hoeher.",
            Icon = "🐐",
            IsSatisfied = tours => tours.SelectMany(t => t.TourLogs).Any(l => l.Difficulty >= 9)
        },
        new AchievementDefinition
        {
            Key = "collector",
            Name = "Sammler",
            Description = "Lege mindestens 5 verschiedene Tours an.",
            Icon = "🗂️",
            IsSatisfied = tours => tours.Count >= 5
        },
        new AchievementDefinition
        {
            Key = "top_rated",
            Name = "Bestbewertet",
            Description = "Erreiche eine Durchschnittsbewertung von mindestens 8 (bei min. 3 Logs).",
            Icon = "⭐",
            IsSatisfied = tours =>
            {
                var logs = tours.SelectMany(t => t.TourLogs).ToList();
                return logs.Count >= 3 && logs.Average(l => l.Rating) >= 8;
            }
        },
        new AchievementDefinition
        {
            Key = "weekly_warrior",
            Name = "Wochenkrieger",
            Description = "Erfasse 5 Logs innerhalb von 7 Tagen.",
            Icon = "🔥",
            IsSatisfied = tours =>
            {
                var dates = tours.SelectMany(t => t.TourLogs)
                    .Select(l => l.DateTime)
                    .OrderBy(d => d)
                    .ToList();

                // Sliding-Window: pruefe fuer jeden Log, ob innerhalb der naechsten 7 Tage
                // mindestens 4 weitere Logs liegen (=> 5 insgesamt)
                for (var i = 0; i < dates.Count; i++)
                {
                    var windowEnd = dates[i].AddDays(7);
                    var countInWindow = dates.Count(d => d >= dates[i] && d < windowEnd);
                    if (countInWindow >= 5) return true;
                }

                return false;
            }
        }
    };
}