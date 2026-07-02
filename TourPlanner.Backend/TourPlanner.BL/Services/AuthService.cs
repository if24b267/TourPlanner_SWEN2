using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TourPlanner.BL.Configuration;
using TourPlanner.BL.Interfaces;
using TourPlanner.BL.Models;
using TourPlanner.DAL;
using TourPlanner.Models;

namespace TourPlanner.BL.Services;

public class AuthService : IAuthService
{
    private readonly TourDbContext _context;
    private readonly JwtOptions _jwtOptions;
    private readonly PasswordHasher<User> _passwordHasher = new();
    private readonly ILogger<AuthService> _logger;

    public AuthService(TourDbContext context, IOptions<JwtOptions> jwtOptions, ILogger<AuthService> logger)
    {
        _context = context;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<AuthResult> RegisterAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return new AuthResult { Success = false, ErrorMessage = "Username und Passwort sind erforderlich" };

        if (password.Length < 6)
            return new AuthResult { Success = false, ErrorMessage = "Passwort muss mindestens 6 Zeichen haben" };

        var exists = await _context.Users.AnyAsync(u => u.Username == username);
        if (exists)
            return new AuthResult { Success = false, ErrorMessage = "Username bereits vergeben" };

        var user = new User { Username = username };
        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Neuer User registriert: {Username}", username);

        return new AuthResult { Success = true, Token = GenerateToken(user) };
    }

    public async Task<AuthResult> LoginAsync(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
            return new AuthResult { Success = false, ErrorMessage = "Ungueltiger Username oder Passwort" };

        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Fehlgeschlagener Login-Versuch fuer {Username}", username);
            return new AuthResult { Success = false, ErrorMessage = "Ungueltiger Username oder Passwort" };
        }

        _logger.LogInformation("User eingeloggt: {Username}", username);
        return new AuthResult { Success = true, Token = GenerateToken(user) };
    }

    private string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}