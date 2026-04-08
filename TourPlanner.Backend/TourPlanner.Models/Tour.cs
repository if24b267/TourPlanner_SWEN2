using System.ComponentModel.DataAnnotations.Schema;

namespace TourPlanner.Models;

public class Tour
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string TourDescription { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string TransportType { get; set; } = "Car";
    public double TourDistance { get; set; }
    public double EstimatedTimeHours { get; set; }
    public string? RouteImagePath { get; set; }
    public List<TourLog> TourLogs { get; set; } = new();

    // Computed
    [NotMapped]
    public int Popularity => TourLogs.Count;
    [NotMapped]
    public double? ChildFriendliness => TourLogs.Any() ?
        Math.Max(0, 10 - TourLogs.Average(l => l.Difficulty) * 0.5) : null;
}