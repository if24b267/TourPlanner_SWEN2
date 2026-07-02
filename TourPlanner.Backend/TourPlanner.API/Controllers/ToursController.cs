using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourPlanner.BL.Interfaces;
using TourPlanner.Models;

namespace TourPlanner.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ToursController : ControllerBase
{
    private readonly ITourService _tourService;

    public ToursController(ITourService tourService)
    {
        _tourService = tourService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<Tour>>> GetAll()
    {
        return await _tourService.GetAllForUserAsync(CurrentUserId);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Tour>> GetById(Guid id)
    {
        var tour = await _tourService.GetByIdAsync(id, CurrentUserId);
        if (tour == null) return NotFound();
        return tour;
    }

    [HttpPost]
    public async Task<ActionResult<Tour>> Create(Tour tour)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(tour.Name))
            return BadRequest("Name is required");
        if (string.IsNullOrWhiteSpace(tour.From))
            return BadRequest("From is required");
        if (string.IsNullOrWhiteSpace(tour.To))
            return BadRequest("To is required");

        tour.UserId = CurrentUserId;
        var created = await _tourService.CreateAsync(tour);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Tour>> Update(Guid id, Tour tour)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _tourService.UpdateAsync(id, tour, CurrentUserId);
        if (updated == null) return NotFound();
        return updated;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _tourService.DeleteAsync(id, CurrentUserId);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<Tour>>> Search([FromQuery] string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return await _tourService.GetAllForUserAsync(CurrentUserId);

        return await _tourService.SearchAsync(text, CurrentUserId);
    }
}