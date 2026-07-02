using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TourPlanner.BL.Configuration;
using TourPlanner.BL.Services;
using TourPlanner.Tests.TestHelpers;

namespace TourPlanner.Tests.Services;

[TestFixture]
public class AuthServiceTests
{
    private AuthService CreateService()
    {
        var context = TestDbContextFactory.Create();
        var jwtOptions = Options.Create(new JwtOptions
        {
            Secret = "unit-test-secret-mindestens-32-zeichen-lang",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryMinutes = 60
        });

        return new AuthService(context, jwtOptions, NullLogger<AuthService>.Instance);
    }

    [Test]
    public async Task RegisterAsync_Succeeds_WithValidCredentials()
    {
        var service = CreateService();
        var result = await service.RegisterAsync("newuser", "password123");

        Assert.That(result.Success, Is.True);
        Assert.That(result.Token, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task RegisterAsync_Fails_WhenUsernameAlreadyExists()
    {
        var service = CreateService();
        await service.RegisterAsync("duplicate", "password123");
        var result = await service.RegisterAsync("duplicate", "otherpassword");

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorMessage, Does.Contain("bereits vergeben"));
    }

    [Test]
    public async Task RegisterAsync_Fails_WhenPasswordTooShort()
    {
        var service = CreateService();
        var result = await service.RegisterAsync("shortpw", "123");

        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task LoginAsync_Succeeds_WithCorrectPassword()
    {
        var service = CreateService();
        await service.RegisterAsync("loginuser", "correctpassword");
        var result = await service.LoginAsync("loginuser", "correctpassword");

        Assert.That(result.Success, Is.True);
        Assert.That(result.Token, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task LoginAsync_Fails_WithWrongPassword()
    {
        var service = CreateService();
        await service.RegisterAsync("wrongpw", "correctpassword");
        var result = await service.LoginAsync("wrongpw", "incorrectpassword");

        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task LoginAsync_Fails_WhenUserDoesNotExist()
    {
        var service = CreateService();
        var result = await service.LoginAsync("ghost", "whatever");

        Assert.That(result.Success, Is.False);
    }
}