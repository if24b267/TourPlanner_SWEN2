using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TourPlanner.BL.Interfaces;
using TourPlanner.BL.Services;
using TourPlanner.DAL;
using TourPlanner.Models;
using TourPlanner.Tests.TestHelpers;

namespace TourPlanner.Tests.Services;

[TestFixture]
public class TourServiceCrudTests
{
    private TourService CreateService(out TourDbContext context)
    {
        context = TestDbContextFactory.Create();
        var routeServiceMock = new Mock<IRouteService>();
        routeServiceMock
            .Setup(r => r.GetRouteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new HttpRequestException("kein Netzwerk in Tests"));
        var achievementServiceMock = new Mock<IAchievementService>();

        return new TourService(context, routeServiceMock.Object, achievementServiceMock.Object, NullLogger<TourService>.Instance);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsNull_WhenTourBelongsToDifferentUser()
    {
        var service = CreateService(out var context);
        var ownerId = Guid.NewGuid();
        var otherId = Guid.NewGuid();

        var tour = new Tour { UserId = ownerId, Name = "Privat", From = "A", To = "B" };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var result = await service.GetByIdAsync(tour.Id, otherId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task UpdateAsync_ReturnsNull_WhenTourBelongsToDifferentUser()
    {
        var service = CreateService(out var context);
        var ownerId = Guid.NewGuid();
        var otherId = Guid.NewGuid();

        var tour = new Tour { UserId = ownerId, Name = "Original", From = "A", To = "B" };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var updateData = new Tour { Name = "Gehackt", From = "X", To = "Y" };
        var result = await service.UpdateAsync(tour.Id, updateData, otherId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_ReturnsFalse_WhenTourDoesNotExist()
    {
        var service = CreateService(out _);
        var result = await service.DeleteAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task DeleteAsync_ReturnsTrue_AndRemovesTour_WhenOwnedByUser()
    {
        var service = CreateService(out var context);
        var userId = Guid.NewGuid();

        var tour = new Tour { UserId = userId, Name = "Loeschbar", From = "A", To = "B" };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var result = await service.DeleteAsync(tour.Id, userId);

        Assert.That(result, Is.True);
        Assert.That(context.Tours.Any(t => t.Id == tour.Id), Is.False);
    }
}