using TourPlanner.BL.Models;

namespace TourPlanner.BL.Interfaces;

/// <summary>
/// Strategy Pattern: abstracts the route-calculation algorithm so the concrete
/// implementation (e.g. OpenRouteService, or a mock/fallback strategy in tests)
/// can be swapped without changing TourService.
/// </summary>
public interface IRouteService
{
    Task<RouteResult> GetRouteAsync(string from, string to, string transportType);
}