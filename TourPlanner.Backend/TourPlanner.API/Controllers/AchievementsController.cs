using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourPlanner.BL.Interfaces;

namespace TourPlanner.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AchievementsController : ControllerBase
{
    private readonly IAchievementService _achievementService;

    public AchievementsController(IAchievementService achievementService)
    {
        _achievementService = achievementService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _achievementService.GetForUserAsync(CurrentUserId);
        return Ok(result);
    }
}