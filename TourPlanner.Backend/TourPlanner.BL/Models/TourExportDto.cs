namespace TourPlanner.BL.Models;

public class TourExportDto
{
    public string ExportedAt { get; set; } = string.Empty;
    public string FormatVersion { get; set; } = "1.0";
    public List<TourExportItem> Tours { get; set; } = new();
}

public class TourExportItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TourDescription { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string TransportType { get; set; } = string.Empty;
    public double TourDistance { get; set; }
    public double EstimatedTimeHours { get; set; }
    public string? RouteGeometryJson { get; set; }
    public List<TourLogExportItem> TourLogs { get; set; } = new();
}

public class TourLogExportItem
{
    public Guid Id { get; set; }
    public DateTime DateTime { get; set; }
    public string Comment { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public double TotalDistance { get; set; }
    public double TotalTimeHours { get; set; }
    public int Rating { get; set; }
}