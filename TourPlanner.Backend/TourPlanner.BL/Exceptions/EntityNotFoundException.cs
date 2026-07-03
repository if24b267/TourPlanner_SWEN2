namespace TourPlanner.BL.Exceptions;

/// <summary>
/// Wird geworfen, wenn eine referenzierte Entitaet (z.B. eine Tour) nicht existiert.
/// </summary>
public class EntityNotFoundException : BusinessException
{
    public EntityNotFoundException(string message) : base(message) { }
}
