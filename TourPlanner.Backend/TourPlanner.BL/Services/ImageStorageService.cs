using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TourPlanner.BL.Configuration;
using TourPlanner.BL.Interfaces;

namespace TourPlanner.BL.Services;

public class ImageStorageService : IImageStorageService
{
    private readonly StorageOptions _options;
    private readonly ILogger<ImageStorageService> _logger;

    private static readonly Dictionary<string, string> ContentTypeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".webp"] = "image/webp"
    };

    public ImageStorageService(IOptions<StorageOptions> options, ILogger<ImageStorageService> logger)
    {
        _options = options.Value;
        _logger = logger;

        if (!Directory.Exists(_options.BaseDirectory))
        {
            Directory.CreateDirectory(_options.BaseDirectory);
            _logger.LogInformation("Storage-Verzeichnis angelegt: {Dir}", _options.BaseDirectory);
        }
    }

    public async Task<string> SaveImageAsync(Guid tourId, Stream fileStream, string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
        if (!ContentTypeMap.ContainsKey(extension))
            throw new InvalidOperationException($"Dateityp {extension} wird nicht unterstuetzt. Erlaubt: {string.Join(", ", ContentTypeMap.Keys)}");

        var fileName = $"{tourId}{extension}";
        var fullPath = Path.Combine(_options.BaseDirectory, fileName);

        await using (var outputStream = new FileStream(fullPath, FileMode.Create))
        {
            await fileStream.CopyToAsync(outputStream);
        }

        _logger.LogInformation("Bild gespeichert fuer Tour {TourId} unter {Path}", tourId, fullPath);

        // relativer Pfad wird in der DB gespeichert, nicht der absolute Systempfad
        return fileName;
    }

    public Task<(Stream Stream, string ContentType)?> GetImageAsync(string relativePath)
    {
        var fullPath = Path.Combine(_options.BaseDirectory, relativePath);
        if (!File.Exists(fullPath))
            return Task.FromResult<(Stream, string)?>(null);

        var extension = Path.GetExtension(fullPath).ToLowerInvariant();
        var contentType = ContentTypeMap.GetValueOrDefault(extension, "application/octet-stream");

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        return Task.FromResult<(Stream, string)?>((stream, contentType));
    }

    public void DeleteImage(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return;

        var fullPath = Path.Combine(_options.BaseDirectory, relativePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            _logger.LogInformation("Bild geloescht: {Path}", fullPath);
        }
    }
}