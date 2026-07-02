using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourPlanner.BL.Interfaces;
using TourPlanner.DAL;
using TourPlanner.Models;

namespace TourPlanner.BL.Services;

public class TourService : ITourService
{
    private readonly TourDbContext _context;
    private readonly IRouteService _routeService;
    private readonly IAchievementService _achievementService;
    private readonly ILogger<TourService> _logger;

    public TourService(TourDbContext context, IRouteService routeService, IAchievementService achievementService, ILogger<TourService> logger)
    {
        _context = context;
        _routeService = routeService;
        _achievementService = achievementService;
        _logger = logger;
    }

    public async Task<List<Tour>> GetAllForUserAsync(Guid userId)
    {
        return await _context.Tours
            .Include(t => t.TourLogs)
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    public async Task<Tour?> GetByIdAsync(Guid id, Guid userId)
    {
        return await _context.Tours
            .Include(t => t.TourLogs)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    public async Task<Tour?> GetByIdRawAsync(Guid id)
    {
        return await _context.Tours.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Tour> CreateAsync(Tour tour)
    {
        tour.Id = Guid.NewGuid();
        await PopulateRouteDataAsync(tour);

        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        await _achievementService.EvaluateAsync(tour.UserId);

        return tour;
    }

    public async Task<Tour?> UpdateAsync(Guid id, Tour tour, Guid userId)
    {
        var existing = await _context.Tours.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (existing == null) return null;

        existing.Name = tour.Name;
        existing.TourDescription = tour.TourDescription;
        existing.From = tour.From;
        existing.To = tour.To;
        existing.TransportType = tour.TransportType;

        await PopulateRouteDataAsync(existing);

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var tour = await _context.Tours.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (tour == null) return false;

        _context.Tours.Remove(tour);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Tour>> SearchAsync(string searchText, Guid userId)
    {
        var lower = searchText.Trim().ToLower();

        var allTours = await _context.Tours
            .Include(t => t.TourLogs)
            .Where(t => t.UserId == userId)
            .ToListAsync();

        var isNumeric = double.TryParse(lower, out var numericValue);

        return allTours.Where(t =>
            t.Name.ToLower().Contains(lower) ||
            t.From.ToLower().Contains(lower) ||
            t.To.ToLower().Contains(lower) ||
            t.TourDescription.ToLower().Contains(lower) ||
            t.TourLogs.Any(l => l.Comment.ToLower().Contains(lower)) ||
            MatchesComputedValueTag(t, lower) ||
            (isNumeric && MatchesComputedValueNumber(t, numericValue))
        ).ToList();
    }

    public async Task UpdateImagePathAsync(Guid id, string? imagePath, Guid userId)
    {
        var tour = await _context.Tours.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (tour == null) return;

        tour.RouteImagePath = imagePath;
        await _context.SaveChangesAsync();
    }

    private async Task PopulateRouteDataAsync(Tour tour)
    {
        try
        {
            var route = await _routeService.GetRouteAsync(tour.From, tour.To, tour.TransportType);
            tour.TourDistance = route.DistanceKm;
            tour.EstimatedTimeHours = route.DurationHours;
            tour.RouteGeometryJson = route.GeometryGeoJson;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "OpenRouteService-Aufruf fehlgeschlagen fuer Tour '{TourName}', verwende Fallback-Werte", tour.Name);

            if (tour.TourDistance == 0)
            {
                tour.TourDistance = new Random().Next(5, 100);
                tour.EstimatedTimeHours = tour.TourDistance / 15.0;
            }
        }
    }

    private static bool MatchesComputedValueTag(Tour tour, string searchLower)
    {
        var isPopular = tour.Popularity >= 3;
        var isNew = tour.Popularity == 0;
        var isChildFriendly = tour.ChildFriendliness is >= 7;

        return (searchLower is "beliebt" or "popular") && isPopular
            || (searchLower is "neu" or "new") && isNew
            || (searchLower is "kinderfreundlich" or "child-friendly" or "childfriendly") && isChildFriendly;
    }

    private static bool MatchesComputedValueNumber(Tour tour, double number)
    {
        var popularityMatch = tour.Popularity == (int)number;
        var childFriendlinessMatch = tour.ChildFriendliness.HasValue
            && Math.Abs(tour.ChildFriendliness.Value - number) < 0.5;

        return popularityMatch || childFriendlinessMatch;
    }
}