namespace TourPlanner.BL.Exceptions;

/// <summary>
/// Basisklasse fuer alle Exceptions, die von der Business-Layer geworfen werden.
/// Layer werfen eigene Exception-Typen statt BCL-Exceptions (z.B. InvalidOperationException),
/// damit aufrufende Layer gezielt auf Business-Fehler reagieren koennen.
/// </summary>
public abstract class BusinessException : Exception
{
    protected BusinessException(string message) : base(message) { }
}
