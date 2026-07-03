namespace TourPlanner.BL.Exceptions;

/// <summary>
/// Wird geworfen, wenn die Routen-/Geocoding-Abfrage bei OpenRouteService fehlschlaegt,
/// z.B. weil fuer eine Adresse keine Koordinaten gefunden wurden.
/// </summary>
public class RouteLookupException : BusinessException
{
    public RouteLookupException(string message) : base(message) { }
}
