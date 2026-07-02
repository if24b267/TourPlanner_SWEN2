namespace TourPlanner.BL.Interfaces;

public interface IImageStorageService
{
    Task<string> SaveImageAsync(Guid tourId, Stream fileStream, string originalFileName);
    Task<(Stream Stream, string ContentType)?> GetImageAsync(string relativePath);
    void DeleteImage(string? relativePath);
}