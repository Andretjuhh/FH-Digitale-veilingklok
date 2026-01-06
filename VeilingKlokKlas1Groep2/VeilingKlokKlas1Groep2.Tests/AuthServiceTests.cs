using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VeilingKlokApp.Data;
using VeilingKlokApp.Models.Domain;
using VeilingKlokKlas1Groep2.Declarations;
using VeilingKlokKlas1Groep2.Services;
using Xunit;

namespace VeilingKlokKlas1Groep2.Tests
{
    public class AuthServiceTests
    {
        // Mock voor IJwtService
        private class MockJwtService : IJwtService
        {
            public string GenerateAccessToken(Account account, string accountType)
            {
                return $"access_token_for_{account.Id}";
            }

            public string GenerateRefreshToken()
            {
                return $"refresh_token_{Guid.NewGuid()}";
            }

            public System.Security.Claims.ClaimsPrincipal? ValidateAccessToken(string token)
            {
                return null; // Niet nodig voor deze tests
            }

            public int? GetAccountIdFromToken(string token)
            {
                return null; // Niet nodig voor deze tests
            }

            public string? GetAccountTypeFromToken(string token)
            {
                return null; // Niet nodig voor deze tests
            }
        }

        // Helper method om in-memory database te maken
        private VeilingKlokContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<VeilingKlokContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new VeilingKlokContext(options);
        }

        // Helper method om JwtSettings te maken
        private IOptions<JwtSettings> CreateJwtSettings()
        {
            var settings = new JwtSettings
            {
                Secret = "test-secret-key-with-minimum-32-characters-for-security",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                AccessTokenExpirationMinutes = 15,
                RefreshTokenExpirationDays = 7
            };
            return Options.Create(settings);
        }

        // Helper method om HttpResponse mock te maken
        private DefaultHttpContext CreateHttpContext()
        {
            return new DefaultHttpContext();
        }

        [Fact]
        public void SignInAsync_GeneratesTokensAndSavesRefreshToken()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var mockJwtService = new MockJwtService();
            var jwtSettings = CreateJwtSettings();
            var authService = new AuthService(context, mockJwtService, jwtSettings);
            
            var httpContext = CreateHttpContext();
            var account = new Koper
            {
                Id = 1,
                Email = "test@test.nl",
                Password = "hashedpw",
                FirstName = "Test",
                LastName = "User",
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = authService.SignInAsync(account, "Koper", httpContext.Response).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test@test.nl", result.Email);
            Assert.Equal(1, result.AccountId);
            Assert.Equal("Koper", result.AccountType);
            Assert.NotNull(result.AccessToken);
            
            // Verifieer dat refresh token is opgeslagen in database
            Assert.Single(context.RefreshTokens);
        }

        [Fact]
        public void RefreshAsync_WithValidToken_GeneratesNewTokens()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var mockJwtService = new MockJwtService();
            var jwtSettings = CreateJwtSettings();
            var authService = new AuthService(context, mockJwtService, jwtSettings);
            
            var httpContext = CreateHttpContext();
            var account = new Koper
            {
                Id = 1,
                Email = "test@test.nl",
                Password = "hashedpw",
                FirstName = "Test",
                LastName = "User",
                CreatedAt = DateTime.UtcNow
            };
            context.Kopers.Add(account);
            
            var refreshToken = new RefreshToken
            {
                Token = "valid_refresh_token",
                AccountId = account.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };
            context.RefreshTokens.Add(refreshToken);
            context.SaveChanges();

            // Act
            var result = authService.RefreshAsync("valid_refresh_token", httpContext.Response).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test@test.nl", result.Email);
            Assert.Equal(1, result.AccountId);
            Assert.Equal("Koper", result.AccountType);
            
            // Verifieer dat oude token is gerevoked
            var oldToken = context.RefreshTokens.First(rt => rt.Token == "valid_refresh_token");
            Assert.NotNull(oldToken.RevokedAt);
        }

        [Fact]
        public void RefreshAsync_WithInvalidToken_ReturnsNull()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var mockJwtService = new MockJwtService();
            var jwtSettings = CreateJwtSettings();
            var authService = new AuthService(context, mockJwtService, jwtSettings);
            
            var httpContext = CreateHttpContext();

            // Act
            var result = authService.RefreshAsync("invalid_token", httpContext.Response).Result;

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void RefreshAsync_WithEmptyToken_ReturnsNull()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var mockJwtService = new MockJwtService();
            var jwtSettings = CreateJwtSettings();
            var authService = new AuthService(context, mockJwtService, jwtSettings);
            
            var httpContext = CreateHttpContext();

            // Act
            var result = authService.RefreshAsync("", httpContext.Response).Result;

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void RefreshAsync_WithRevokedToken_ReturnsNull()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var mockJwtService = new MockJwtService();
            var jwtSettings = CreateJwtSettings();
            var authService = new AuthService(context, mockJwtService, jwtSettings);
            
            var httpContext = CreateHttpContext();
            var account = new Koper
            {
                Id = 1,
                Email = "test@test.nl",
                Password = "hashedpw",
                FirstName = "Test",
                LastName = "User",
                CreatedAt = DateTime.UtcNow
            };
            context.Kopers.Add(account);
            
            var refreshToken = new RefreshToken
            {
                Token = "revoked_token",
                AccountId = account.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                RevokedAt = DateTime.UtcNow.AddMinutes(-10) // Already revoked
            };
            context.RefreshTokens.Add(refreshToken);
            context.SaveChanges();

            // Act
            var result = authService.RefreshAsync("revoked_token", httpContext.Response).Result;

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void LogoutAsync_WithValidToken_RevokesToken()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var mockJwtService = new MockJwtService();
            var jwtSettings = CreateJwtSettings();
            var authService = new AuthService(context, mockJwtService, jwtSettings);
            
            var httpContext = CreateHttpContext();
            var account = new Koper
            {
                Id = 1,
                Email = "test@test.nl",
                Password = "hashedpw",
                FirstName = "Test",
                LastName = "User",
                CreatedAt = DateTime.UtcNow
            };
            context.Kopers.Add(account);
            
            var refreshToken = new RefreshToken
            {
                Token = "logout_token",
                AccountId = account.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };
            context.RefreshTokens.Add(refreshToken);
            context.SaveChanges();

            // Act
            authService.LogoutAsync("logout_token", httpContext.Response).Wait();

            // Assert
            var revokedToken = context.RefreshTokens.First(rt => rt.Token == "logout_token");
            Assert.NotNull(revokedToken.RevokedAt);
        }

        [Fact]
        public void LogoutAsync_WithEmptyToken_DoesNotThrow()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var mockJwtService = new MockJwtService();
            var jwtSettings = CreateJwtSettings();
            var authService = new AuthService(context, mockJwtService, jwtSettings);
            
            var httpContext = CreateHttpContext();

            // Act & Assert - should not throw exception
            authService.LogoutAsync("", httpContext.Response).Wait();
        }
    }
}
