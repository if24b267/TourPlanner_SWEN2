using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourPlanner.BL.Exceptions;
using TourPlanner.BL.Interfaces;

namespace TourPlanner.API.Controllers;

[ApiController]
[Route("api/tours/{tourId}/image")]
[Authorize]
public class TourImagesController : ControllerBase
{
    private readonly IImageStorageService _imageStorage;
    private readonly ITourService _tourService;

    public TourImagesController(IImageStorageService imageStorage, ITourService tourService)
    {
        _imageStorage = imageStorage;
        _tourService = tourService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10_000_000)] // 10 MB
    public async Task<IActionResult> Upload(Guid tourId, IFormFile file)
    {
        var tour = await _tourService.GetByIdAsync(tourId, CurrentUserId);
        if (tour == null) return NotFound();

        if (file == null || file.Length == 0)
            return BadRequest("Keine Datei uebermittelt");

        try
        {
            _imageStorage.DeleteImage(tour.RouteImagePath);

            await using var stream = file.OpenReadStream();
            var relativePath = await _imageStorage.SaveImageAsync(tourId, stream, file.FileName);

            tour.RouteImagePath = relativePath;
            await _tourService.UpdateImagePathAsync(tourId, relativePath, CurrentUserId);

            return Ok(new { path = relativePath });
        }
        catch (UnsupportedFileTypeException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [AllowAnonymous] // Bild-Anzeige im <img>-Tag im Frontend braucht keinen Auth-Header
    public async Task<IActionResult> Get(Guid tourId)
    {
        var tour = await _tourService.GetByIdRawAsync(tourId);
        if (tour?.RouteImagePath == null) return NotFound();

        var result = await _imageStorage.GetImageAsync(tour.RouteImagePath);
        if (result == null) return NotFound();

        return File(result.Value.Stream, result.Value.ContentType);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid tourId)
    {
        var tour = await _tourService.GetByIdAsync(tourId, CurrentUserId);
        if (tour == null) return NotFound();

        _imageStorage.DeleteImage(tour.RouteImagePath);
        await _tourService.UpdateImagePathAsync(tourId, null, CurrentUserId);

        return NoContent();
    }
}