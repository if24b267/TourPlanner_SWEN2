using TourPlanner.BL.Achievements;
using TourPlanner.Models;

namespace TourPlanner.Tests.Achievements;

[TestFixture]
public class AchievementCatalogTests
{
    private static AchievementDefinition GetDefinition(string key)
        => AchievementCatalog.All.First(a => a.Key == key);

    [Test]
    public void FirstTour_IsSatisfied_WhenAtLeastOneTourExists()
    {
        var tours = new List<Tour> { new Tour() };
        Assert.That(GetDefinition("first_tour").IsSatisfied(tours), Is.True);
    }

    [Test]
    public void FirstTour_IsNotSatisfied_WhenNoToursExist()
    {
        var tours = new List<Tour>();
        Assert.That(GetDefinition("first_tour").IsSatisfied(tours), Is.False);
    }

    [Test]
    public void FirstLog_IsSatisfied_WhenAtLeastOneLogExists()
    {
        var tours = new List<Tour>
        {
            new Tour { TourLogs = new List<TourLog> { new TourLog() } }
        };
        Assert.That(GetDefinition("first_log").IsSatisfied(tours), Is.True);
    }

    [Test]
    public void FirstLog_IsNotSatisfied_WhenNoLogsExist()
    {
        var tours = new List<Tour> { new Tour() };
        Assert.That(GetDefinition("first_log").IsSatisfied(tours), Is.False);
    }

    [Test]
    public void CenturyRider_IsSatisfied_WhenTotalDistanceReaches100Km()
    {
        var tours = new List<Tour>
        {
            new Tour { TourLogs = new List<TourLog>
            {
                new TourLog { TotalDistance = 60 },
                new TourLog { TotalDistance = 45 }
            }}
        };
        Assert.That(GetDefinition("century_rider").IsSatisfied(tours), Is.True);
    }

    [Test]
    public void CenturyRider_IsNotSatisfied_WhenTotalDistanceBelow100Km()
    {
        var tours = new List<Tour>
        {
            new Tour { TourLogs = new List<TourLog> { new TourLog { TotalDistance = 40 } } }
        };
        Assert.That(GetDefinition("century_rider").IsSatisfied(tours), Is.False);
    }

    [Test]
    public void Endurance_IsSatisfied_WhenSingleLogHasAtLeast5Hours()
    {
        var tours = new List<Tour>
        {
            new Tour { TourLogs = new List<TourLog> { new TourLog { TotalTimeHours = 5.5 } } }
        };
        Assert.That(GetDefinition("endurance").IsSatisfied(tours), Is.True);
    }

    [Test]
    public void Endurance_IsNotSatisfied_WhenNoLogReaches5Hours()
    {
        var tours = new List<Tour>
        {
            new Tour { TourLogs = new List<TourLog> { new TourLog { TotalTimeHours = 2 } } }
        };
        Assert.That(GetDefinition("endurance").IsSatisfied(tours), Is.False);
    }

    [Test]
    public void MountainGoat_IsSatisfied_WhenDifficultyIsAtLeast9()
    {
        var tours = new List<Tour>
        {
            new Tour { TourLogs = new List<TourLog> { new TourLog { Difficulty = 9 } } }
        };
        Assert.That(GetDefinition("mountain_goat").IsSatisfied(tours), Is.True);
    }

    [Test]
    public void MountainGoat_IsNotSatisfied_WhenDifficultyBelow9()
    {
        var tours = new List<Tour>
        {
            new Tour { TourLogs = new List<TourLog> { new TourLog { Difficulty = 8 } } }
        };
        Assert.That(GetDefinition("mountain_goat").IsSatisfied(tours), Is.False);
    }

    [Test]
    public void Collector_IsSatisfied_WhenAtLeast5ToursExist()
    {
        var tours = Enumerable.Range(0, 5).Select(_ => new Tour()).ToList();
        Assert.That(GetDefinition("collector").IsSatisfied(tours), Is.True);
    }

    [Test]
    public void Collector_IsNotSatisfied_WhenFewerThan5ToursExist()
    {
        var tours = Enumerable.Range(0, 4).Select(_ => new Tour()).ToList();
        Assert.That(GetDefinition("collector").IsSatisfied(tours), Is.False);
    }

    [Test]
    public void TopRated_IsSatisfied_WhenAverageRatingAtLeast8WithMin3Logs()
    {
        var tours = new List<Tour>
        {
            new Tour { TourLogs = new List<TourLog>
            {
                new TourLog { Rating = 8 },
                new TourLog { Rating = 9 },
                new TourLog { Rating = 8 }
            }}
        };
        Assert.That(GetDefinition("top_rated").IsSatisfied(tours), Is.True);
    }

    [Test]
    public void TopRated_IsNotSatisfied_WhenFewerThan3Logs()
    {
        var tours = new List<Tour>
        {
            new Tour { TourLogs = new List<TourLog>
            {
                new TourLog { Rating = 10 },
                new TourLog { Rating = 10 }
            }}
        };
        Assert.That(GetDefinition("top_rated").IsSatisfied(tours), Is.False);
    }

    [Test]
    public void WeeklyWarrior_IsSatisfied_When5LogsWithin7Days()
    {
        var baseDate = new DateTime(2026, 1, 1);
        var tours = new List<Tour>
        {
            new Tour { TourLogs = Enumerable.Range(0, 5)
                .Select(i => new TourLog { DateTime = baseDate.AddDays(i) })
                .ToList() }
        };
        Assert.That(GetDefinition("weekly_warrior").IsSatisfied(tours), Is.True);
    }

    [Test]
    public void WeeklyWarrior_IsNotSatisfied_When5LogsSpreadOverMoreThan7Days()
    {
        var baseDate = new DateTime(2026, 1, 1);
        var tours = new List<Tour>
        {
            new Tour { TourLogs = Enumerable.Range(0, 5)
                .Select(i => new TourLog { DateTime = baseDate.AddDays(i * 3) })
                .ToList() }
        };
        Assert.That(GetDefinition("weekly_warrior").IsSatisfied(tours), Is.False);
    }
}