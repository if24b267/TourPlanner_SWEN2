using TourPlanner.Models;

namespace TourPlanner.Tests.ModelTests;

[TestFixture]
public class TourComputedPropertiesTests
{
    [Test]
    public void Popularity_EqualsNumberOfTourLogs()
    {
        var tour = new Tour
        {
            TourLogs = new List<TourLog> { new TourLog(), new TourLog(), new TourLog() }
        };

        Assert.That(tour.Popularity, Is.EqualTo(3));
    }

    [Test]
    public void ChildFriendliness_IsNull_WhenNoLogsExist()
    {
        var tour = new Tour();

        Assert.That(tour.ChildFriendliness, Is.Null);
    }

    [Test]
    public void ChildFriendliness_IsComputedFromAverageDifficulty()
    {
        var tour = new Tour
        {
            TourLogs = new List<TourLog>
            {
                new TourLog { Difficulty = 2 },
                new TourLog { Difficulty = 4 }
            }
        };

        // Durchschnitt Difficulty = 3 -> 10 - 3*0.5 = 8.5
        Assert.That(tour.ChildFriendliness, Is.EqualTo(8.5));
    }
}