namespace TourPlanner.BL.Configuration;

public class JwtOptions
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "TourPlanner";
    public string Audience { get; set; } = "TourPlannerClient";
    public int ExpiryMinutes { get; set; } = 120;
}