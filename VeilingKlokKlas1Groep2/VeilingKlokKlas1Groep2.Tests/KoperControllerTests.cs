using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeilingKlokApp.Controllers;
using VeilingKlokApp.Data;
using VeilingKlokApp.Models;
using VeilingKlokApp.Models.Domain;
using VeilingKlokKlas1Groep2.Declarations;
using VeilingKlokKlas1Groep2.Services;
using Xunit;

namespace VeilingKlokKlas1Groep2.Tests
{
    public class KoperControllerTests
    {
        // Mock implementaties
        private class MockPasswordHasher : IPasswordHasher
        {
            public string HashPassword(string password) => $"hashed_{password}";
            public bool VerifyPassword(string hashedPassword, string providedPassword) => true;
        }

        private class MockAuthService : IAuthService
        {
            public Task<VeilingKlokApp.Models.OutputDTOs.AuthResponse> SignInAsync(Account account, string accountType, HttpResponse response)
            {
                return Task.FromResult(new VeilingKlokApp.Models.OutputDTOs.AuthResponse
                {
                    AccountId = account.Id,
                    AccessToken = "fake_token",
                    AccountType = accountType,
                    Email = account.Email,
                    AccessTokenExpiresAt = DateTime.UtcNow.AddHours(1),
                    RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7)
                });
            }

            public Task<VeilingKlokApp.Models.OutputDTOs.AuthResponse?> RefreshAsync(string refreshTokenString, HttpResponse response)
            {
                return Task.FromResult<VeilingKlokApp.Models.OutputDTOs.AuthResponse?>(new VeilingKlokApp.Models.OutputDTOs.AuthResponse
                {
                    AccessToken = "new_fake_token",
                    AccessTokenExpiresAt = DateTime.UtcNow.AddHours(1),
                    RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7)
                });
            }

            public Task LogoutAsync(string refreshTokenString, HttpResponse response)
            {
                return Task.CompletedTask;
            }
        }

        private VeilingKlokContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<VeilingKlokContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            return new VeilingKlokContext(options);
        }

        [Fact]
        public void CreateKoperAccount_WithValidData_ReturnsOk()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var mockHasher = new MockPasswordHasher();
            var mockAuth = new MockAuthService();
            var controller = new KoperController(context, mockHasher, mockAuth);
            
            var newKoper = new NewKoperAccount
            {
                Email = "koper@test.nl",
                Password = "P@ssw0rd123!",
                FirstName = "Jan",
                LastName = "Jansen",
                Adress = "Teststraat 1",
                PostCode = "1234AB",
                Regio = "Noord"
            };

            // Act
            var result = controller.CreateKoperAccount(newKoper).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void CreateKoperAccount_WithNullData_ReturnsBadRequest()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var mockHasher = new MockPasswordHasher();
            var mockAuth = new MockAuthService();
            var controller = new KoperController(context, mockHasher, mockAuth);

            // Act
            var result = controller.CreateKoperAccount(null).Result;

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<HtppError>(badRequestResult.Value);
            Assert.Equal(400, error.code);
        }

        [Fact]
        public void CreateKoperAccount_WithDuplicateEmail_ReturnsConflict()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var mockHasher = new MockPasswordHasher();
            var mockAuth = new MockAuthService();
            
            // Voeg bestaande koper toe
            context.Kopers.Add(new Koper
            {
                Email = "duplicate@test.nl",
                Password = "hashedpw",
                FirstName = "Bestaande",
                LastName = "Gebruiker",
                CreatedAt = DateTime.UtcNow
            });
            context.SaveChanges();

            var controller = new KoperController(context, mockHasher, mockAuth);
            
            var newKoper = new NewKoperAccount
            {
                Email = "duplicate@test.nl",
                Password = "P@ssw0rd123!",
                FirstName = "Nieuwe",
                LastName = "Gebruiker",
                Adress = "Teststraat 1",
                PostCode = "1234AB",
                Regio = "Noord"
            };

            // Act
            var result = controller.CreateKoperAccount(newKoper).Result;

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            var error = Assert.IsType<HtppError>(conflictResult.Value);
            Assert.Equal(409, error.code);
        }

        [Fact]
        public void GetKoperAccount_WithValidId_ReturnsKoperDetails()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var koperId = 1;
            
            var koper = new Koper
            {
                Id = koperId,
                Email = "koper@test.nl",
                Password = "hashedpw",
                FirstName = "Jan",
                LastName = "Jansen",
                Adress = "Teststraat 1",
                PostCode = "1234AB",
                Regio = "Noord",
                CreatedAt = DateTime.UtcNow
            };
            context.Kopers.Add(koper);
            context.SaveChanges();

            var mockHasher = new MockPasswordHasher();
            var mockAuth = new MockAuthService();
            var controller = new KoperController(context, mockHasher, mockAuth);

            // Act
            var result = controller.GetKoperAccount(koperId).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void GetKoperAccount_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var mockHasher = new MockPasswordHasher();
            var mockAuth = new MockAuthService();
            var controller = new KoperController(context, mockHasher, mockAuth);

            // Act
            var result = controller.GetKoperAccount(999).Result;

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var error = Assert.IsType<HtppError>(notFoundResult.Value);
            Assert.Equal(404, error.code);
        }
    }
}
