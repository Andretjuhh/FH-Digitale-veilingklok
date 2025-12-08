using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VeilingKlokApp.Controllers;
using VeilingKlokApp.Data;
using VeilingKlokApp.Models.Domain;
using VeilingKlokApp.Models.OutputDTOs;
using VeilingKlokKlas1Groep2.Declarations;
using VeilingKlokKlas1Groep2.Models.InputDTOs;
using VeilingKlokKlas1Groep2.Services;
using Xunit;

namespace VeilingKlokKlas1Groep2.Tests;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_ReturnsOk_ForValidCredentials()
    {
        var options = new DbContextOptionsBuilder<VeilingKlokContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new VeilingKlokContext(options);
        var passwordHasher = new PasswordHasher();

        var account = new Koper
        {
            Email = "user@example.com",
            Password = passwordHasher.HashPassword("P@ssw0rd!"),
            FirstName = "Jane",
            LastName = "Doe",
            CreatedAt = DateTime.UtcNow
        };

        context.Kopers.Add(account);
        await context.SaveChangesAsync();

        var expectedAuthResponse = new AuthResponse
        {
            AccessToken = "token-123",
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7),
            AccountId = account.Id,
            Email = account.Email,
            AccountType = "Koper"
        };

        var controller = new AuthController(
            context,
            passwordHasher,
            new FakeJwtService(),
            Options.Create(new JwtSettings()),
            new FakeAuthService(expectedAuthResponse))
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = await controller.Login(new LoginRequest
        {
            Email = account.Email,
            Password = "P@ssw0rd!"
        });

        var okResult = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<AuthResponse>(okResult.Value);
        Assert.Equal(expectedAuthResponse.AccessToken, payload.AccessToken);
        Assert.Equal(account.Email, payload.Email);
        Assert.Equal("Koper", payload.AccountType);
    }

    private sealed class FakeJwtService : IJwtService
    {
        public string GenerateAccessToken(Account account, string accountType) => "token";
        public string GenerateRefreshToken() => "refresh";
        public ClaimsPrincipal? ValidateAccessToken(string token) => null;
        public int? GetAccountIdFromToken(string token) => null;
        public string? GetAccountTypeFromToken(string token) => null;
    }

    private sealed class FakeAuthService : IAuthService
    {
        private readonly AuthResponse _authResponse;

        public FakeAuthService(AuthResponse authResponse)
        {
            _authResponse = authResponse;
        }

        public Task<AuthResponse> SignInAsync(Account account, string accountType, HttpResponse response)
        {
            return Task.FromResult(_authResponse);
        }

        public Task<AuthResponse?> RefreshAsync(string refreshTokenString, HttpResponse response)
        {
            return Task.FromResult<AuthResponse?>(null);
        }

        public Task LogoutAsync(string refreshTokenString, HttpResponse response)
        {
            return Task.CompletedTask;
        }
    }
}
