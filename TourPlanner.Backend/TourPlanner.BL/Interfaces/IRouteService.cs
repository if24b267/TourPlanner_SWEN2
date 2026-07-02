using TourPlanner.BL.Models;

namespace TourPlanner.BL.Interfaces;

public interface IRouteService
{
    Task<RouteResult> GetRouteAsync(string from, string to, string transportType);
}