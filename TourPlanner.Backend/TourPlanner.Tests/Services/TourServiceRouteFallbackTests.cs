using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TourPlanner.BL.Interfaces;
using TourPlanner.BL.Models;
using TourPlanner.BL.Services;
using TourPlanner.Models;
using TourPlanner.Tests.TestHelpers;

namespace TourPlanner.Tests.Services;

[TestFixture]
public class TourServiceRouteFallbackTests
{
    [Test]
    public async Task CreateAsync_UsesFallbackDistance_WhenRouteServiceThrows()
    {
        var context = TestDbContextFactory.Create();
        var routeServiceMock = new Mock<IRouteService>();
        routeServiceMock
            .Setup(r => r.GetRouteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new HttpRequestException("simulierter ORS-Ausfall"));

        var achievementServiceMock = new Mock<IAchievementService>();

        var service = new TourService(context, routeServiceMock.Object, achievementServiceMock.Object, NullLogger<TourService>.Instance);

        var tour = new Tour { UserId = Guid.NewGuid(), Name = "Test", From = "A", To = "B" };
        var created = await service.CreateAsync(tour);

        Assert.That(created.TourDistance, Is.GreaterThan(0));
        Assert.That(created.EstimatedTimeHours, Is.GreaterThan(0));
        Assert.That(created.RouteGeometryJson, Is.Null);
    }

    [Test]
    public async Task CreateAsync_UsesRealRouteData_WhenRouteServiceSucceeds()
    {
        var context = TestDbContextFactory.Create();
        var routeServiceMock = new Mock<IRouteService>();
        routeServiceMock
            .Setup(r => r.GetRouteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new RouteResult
            {
                DistanceKm = 42,
                DurationHours = 2.5,
                GeometryGeoJson = "{\"type\":\"LineString\"}"
            });

        var achievementServiceMock = new Mock<IAchievementService>();

        var service = new TourService(context, routeServiceMock.Object, achievementServiceMock.Object, NullLogger<TourService>.Instance);

        var tour = new Tour { UserId = Guid.NewGuid(), Name = "Test", From = "A", To = "B" };
        var created = await service.CreateAsync(tour);

        Assert.That(created.TourDistance, Is.EqualTo(42));
        Assert.That(created.EstimatedTimeHours, Is.EqualTo(2.5));
        Assert.That(created.RouteGeometryJson, Is.EqualTo("{\"type\":\"LineString\"}"));
    }
}