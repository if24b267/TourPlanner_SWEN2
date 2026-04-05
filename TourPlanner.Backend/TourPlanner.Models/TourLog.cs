namespace TourPlanner.Models;

public class TourLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime DateTime { get; set; }
    public string Comment { get; set; } = string.Empty;
    public int Difficulty { get; set; } = 5; // 1-10
    public double TotalDistance { get; set; }
    public double TotalTimeHours { get; set; }
    public int Rating { get; set; } = 5; // 1-10
}