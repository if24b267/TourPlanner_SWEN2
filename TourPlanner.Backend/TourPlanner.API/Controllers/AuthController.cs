using Microsoft.AspNetCore.Mvc;
using TourPlanner.BL.Interfaces;

namespace TourPlanner.API.Controllers;

public record RegisterRequest(string Username, string Password);
public record LoginRequest(string Username, string Password);

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request.Username, request.Password);
        if (!result.Success) return BadRequest(new { message = result.ErrorMessage });
        return Ok(new { token = result.Token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.Username, request.Password);
        if (!result.Success) return Unauthorized(new { message = result.ErrorMessage });
        return Ok(new { token = result.Token });
    }
}