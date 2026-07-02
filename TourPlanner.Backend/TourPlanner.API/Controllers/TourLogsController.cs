using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourPlanner.BL.Interfaces;
using TourPlanner.Models;

namespace TourPlanner.API.Controllers;

[ApiController]
[Route("api/tours/{tourId}/logs")]
[Authorize]
public class TourLogsController : ControllerBase
{
    private readonly ITourLogService _logService;
    private readonly ITourService _tourService;

    public TourLogsController(ITourLogService logService, ITourService tourService)
    {
        _logService = logService;
        _tourService = tourService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<TourLog>>> GetByTour(Guid tourId)
    {
        var tour = await _tourService.GetByIdAsync(tourId, CurrentUserId);
        if (tour == null) return NotFound();

        return await _logService.GetByTourIdAsync(tourId);
    }

    [HttpPost]
    public async Task<ActionResult<TourLog>> Create(Guid tourId, TourLog log)
    {
        var tour = await _tourService.GetByIdAsync(tourId, CurrentUserId);
        if (tour == null) return NotFound();

        if (log.Difficulty < 1 || log.Difficulty > 10)
            return BadRequest("Difficulty must be 1-10");
        if (log.Rating < 1 || log.Rating > 10)
            return BadRequest("Rating must be 1-10");
        if (log.TotalDistance < 0)
            return BadRequest("Distance cannot be negative");
        if (log.TotalTimeHours < 0)
            return BadRequest("Time cannot be negative");

        try
        {
            var created = await _logService.CreateAsync(tourId, log);
            return CreatedAtAction(nameof(GetByTour), new { tourId }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TourLog>> Update(Guid tourId, Guid id, TourLog log)
    {
        var tour = await _tourService.GetByIdAsync(tourId, CurrentUserId);
        if (tour == null) return NotFound();

        if (log.Difficulty < 1 || log.Difficulty > 10)
            return BadRequest("Difficulty must be 1-10");
        if (log.Rating < 1 || log.Rating > 10)
            return BadRequest("Rating must be 1-10");

        var updated = await _logService.UpdateAsync(id, log);
        if (updated == null) return NotFound();
        return updated;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid tourId, Guid id)
    {
        var tour = await _tourService.GetByIdAsync(tourId, CurrentUserId);
        if (tour == null) return NotFound();

        var success = await _logService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}