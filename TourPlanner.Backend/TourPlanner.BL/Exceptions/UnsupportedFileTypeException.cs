namespace TourPlanner.BL.Exceptions;

/// <summary>
/// Wird geworfen, wenn beim Bild-Upload ein nicht unterstuetzter Dateityp uebermittelt wird.
/// </summary>
public class UnsupportedFileTypeException : BusinessException
{
    public UnsupportedFileTypeException(string message) : base(message) { }
}
