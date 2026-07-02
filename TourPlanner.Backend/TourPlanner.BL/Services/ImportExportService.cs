using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourPlanner.BL.Interfaces;
using TourPlanner.BL.Models;
using TourPlanner.DAL;
using TourPlanner.Models;

namespace TourPlanner.BL.Services;

public class ImportExportService : IImportExportService
{
    private readonly TourDbContext _context;
    private readonly ILogger<ImportExportService> _logger;

    public ImportExportService(TourDbContext context, ILogger<ImportExportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TourExportDto> ExportAsync(Guid userId)
    {
        var tours = await _context.Tours
            .Include(t => t.TourLogs)
            .Where(t => t.UserId == userId)
            .ToListAsync();

        var dto = new TourExportDto
        {
            ExportedAt = DateTime.UtcNow.ToString("O"),
            Tours = tours.Select(t => new TourExportItem
            {
                Id = t.Id,
                Name = t.Name,
                TourDescription = t.TourDescription,
                From = t.From,
                To = t.To,
                TransportType = t.TransportType,
                TourDistance = t.TourDistance,
                EstimatedTimeHours = t.EstimatedTimeHours,
                RouteGeometryJson = t.RouteGeometryJson,
                TourLogs = t.TourLogs.Select(l => new TourLogExportItem
                {
                    Id = l.Id,
                    DateTime = l.DateTime,
                    Comment = l.Comment,
                    Difficulty = l.Difficulty,
                    TotalDistance = l.TotalDistance,
                    TotalTimeHours = l.TotalTimeHours,
                    Rating = l.Rating
                }).ToList()
            }).ToList()
        };

        _logger.LogInformation("Export erstellt fuer User {UserId}: {Count} Tours", userId, dto.Tours.Count);
        return dto;
    }

    public async Task<int> ImportAsync(Guid userId, TourExportDto data)
    {
        var importedCount = 0;

        foreach (var item in data.Tours)
        {
            // Falls die Tour (gleiche Id) beim User schon existiert, ueberspringen -
            // verhindert Duplikate bei mehrfachem Import derselben Datei
            var exists = await _context.Tours.AnyAsync(t => t.Id == item.Id && t.UserId == userId);
            if (exists)
            {
                _logger.LogWarning("Tour {TourId} existiert bereits, wird beim Import uebersprungen", item.Id);
                continue;
            }

            var tour = new Tour
            {
                Id = item.Id,
                UserId = userId,
                Name = item.Name,
                TourDescription = item.TourDescription,
                From = item.From,
                To = item.To,
                TransportType = item.TransportType,
                TourDistance = item.TourDistance,
                EstimatedTimeHours = item.EstimatedTimeHours,
                RouteGeometryJson = item.RouteGeometryJson,
                TourLogs = item.TourLogs.Select(l => new TourLog
                {
                    Id = l.Id,
                    DateTime = l.DateTime,
                    Comment = l.Comment,
                    Difficulty = l.Difficulty,
                    TotalDistance = l.TotalDistance,
                    TotalTimeHours = l.TotalTimeHours,
                    Rating = l.Rating
                }).ToList()
            };

            _context.Tours.Add(tour);
            importedCount++;
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Import abgeschlossen fuer User {UserId}: {Count} Tours importiert", userId, importedCount);

        return importedCount;
    }
}