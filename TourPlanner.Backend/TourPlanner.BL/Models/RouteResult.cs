namespace TourPlanner.BL.Models;

public class RouteResult
{
    public double DistanceKm { get; set; }
    public double DurationHours { get; set; }
    public string? GeometryGeoJson { get; set; }
}