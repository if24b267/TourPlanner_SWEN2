using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TourPlanner.BL.Configuration;
using TourPlanner.BL.Exceptions;
using TourPlanner.BL.Interfaces;
using TourPlanner.BL.Models;

namespace TourPlanner.BL.Services;

public class OpenRouteService : IRouteService
{
    private readonly HttpClient _httpClient;
    private readonly OpenRouteServiceOptions _options;
    private readonly ILogger<OpenRouteService> _logger;

    // Mapping unserer Tour-Transporttypen auf ORS-Profile
    private static readonly Dictionary<string, string> ProfileMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Car"] = "driving-car",
        ["Bike"] = "cycling-regular",
        ["Hike"] = "foot-hiking",
        ["Running"] = "foot-walking",
        ["Vacation"] = "driving-car"
    };

    public OpenRouteService(HttpClient httpClient, IOptions<OpenRouteServiceOptions> options, ILogger<OpenRouteService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
    }

    public async Task<RouteResult> GetRouteAsync(string from, string to, string transportType)
    {
        var profile = ProfileMap.TryGetValue(transportType, out var mapped) ? mapped : "driving-car";

        var fromCoords = await GeocodeAsync(from);
        var toCoords = await GeocodeAsync(to);

        var jsonBody = JsonSerializer.Serialize(new
        {
            coordinates = new[] { fromCoords, toCoords }
        });

        using var request = new HttpRequestMessage(HttpMethod.Post, $"/v2/directions/{profile}/geojson")
        {
            Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
        };
        request.Headers.TryAddWithoutValidation("Authorization", _options.ApiKey);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var feature = doc.RootElement.GetProperty("features")[0];
        var segment = feature.GetProperty("properties").GetProperty("segments")[0];

        var distanceMeters = segment.GetProperty("distance").GetDouble();
        var durationSeconds = segment.GetProperty("duration").GetDouble();
        var geometry = feature.GetProperty("geometry").GetRawText();

        return new RouteResult
        {
            DistanceKm = Math.Round(distanceMeters / 1000.0, 2),
            DurationHours = Math.Round(durationSeconds / 3600.0, 2),
            GeometryGeoJson = geometry
        };
    }

    private async Task<double[]> GeocodeAsync(string address)
    {
        var url = $"/geocode/search?api_key={Uri.EscapeDataString(_options.ApiKey)}&text={Uri.EscapeDataString(address)}&size=1";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var features = doc.RootElement.GetProperty("features");
        if (features.GetArrayLength() == 0)
            throw new RouteLookupException($"Keine Koordinaten gefunden fuer Adresse: {address}");

        var coords = features[0].GetProperty("geometry").GetProperty("coordinates");
        return new[] { coords[0].GetDouble(), coords[1].GetDouble() };
    }
}