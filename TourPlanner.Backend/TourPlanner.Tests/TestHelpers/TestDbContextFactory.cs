using Microsoft.EntityFrameworkCore;
using TourPlanner.DAL;

namespace TourPlanner.Tests.TestHelpers;

public static class TestDbContextFactory
{
    public static TourDbContext Create()
    {
        var options = new DbContextOptionsBuilder<TourDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TourDbContext(options);
    }
}