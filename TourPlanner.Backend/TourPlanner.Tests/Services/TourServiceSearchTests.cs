using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TourPlanner.BL.Interfaces;
using TourPlanner.BL.Services;
using TourPlanner.DAL;
using TourPlanner.Models;
using TourPlanner.Tests.TestHelpers;

namespace TourPlanner.Tests.Services;

[TestFixture]
public class TourServiceSearchTests
{
    private record TestSetup(TourDbContext Context, TourService Service, Guid UserId);

    private async Task<TestSetup> SetupAsync()
    {
        var context = TestDbContextFactory.Create();
        var routeServiceMock = new Mock<IRouteService>();
        var achievementServiceMock = new Mock<IAchievementService>();

        var service = new TourService(context, routeServiceMock.Object, achievementServiceMock.Object, NullLogger<TourService>.Instance);

        var userId = Guid.NewGuid();

        context.Tours.Add(new Tour
        {
            UserId = userId,
            Name = "Donauinsel Runde",
            From = "Wien",
            To = "Tulln",
            TourLogs = new List<TourLog> { new(), new(), new() } // Popularity = 3
        });
        context.Tours.Add(new Tour
        {
            UserId = userId,
            Name = "Frisch angelegte Tour",
            From = "Graz",
            To = "Salzburg",
            TourLogs = new List<TourLog>() // Popularity = 0
        });
        await context.SaveChangesAsync();

        return new TestSetup(context, service, userId);
    }

    [Test]
    public async Task SearchAsync_FindsMatchByTourName()
    {
        var setup = await SetupAsync();
        var result = await setup.Service.SearchAsync("Donauinsel", setup.UserId);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Donauinsel Runde"));
    }

    [Test]
    public async Task SearchAsync_FindsMatchByTag_Beliebt()
    {
        var setup = await SetupAsync();
        var result = await setup.Service.SearchAsync("beliebt", setup.UserId);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Donauinsel Runde"));
    }

    [Test]
    public async Task SearchAsync_FindsMatchByTag_Neu()
    {
        var setup = await SetupAsync();
        var result = await setup.Service.SearchAsync("neu", setup.UserId);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Frisch angelegte Tour"));
    }

    [Test]
    public async Task SearchAsync_FindsMatchByNumericPopularity()
    {
        var setup = await SetupAsync();
        var result = await setup.Service.SearchAsync("3", setup.UserId);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Donauinsel Runde"));
    }

    [Test]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatchFound()
    {
        var setup = await SetupAsync();
        var result = await setup.Service.SearchAsync("nonexistentxyz", setup.UserId);

        Assert.That(result, Is.Empty);
    }
}