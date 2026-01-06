using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeilingKlokApp.Controllers;
using VeilingKlokApp.Data;
using VeilingKlokApp.Models;
using VeilingKlokApp.Models.Domain;
using VeilingKlokKlas1Groep2.Declarations;
using VeilingKlokKlas1Groep2.Models;
using VeilingKlokKlas1Groep2.Services;
using Xunit;

namespace VeilingKlokKlas1Groep2.Tests
{
    public class VeilingmeesterControllerTests
    {
        // Mock implementations
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
        public void CreateVeilingmeesterAccount_WithValidData_ReturnsOk()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new VeilingmeesterController(context, new MockPasswordHasher(), new MockAuthService());

            var newVeilingmeester = new NewVeilingMeesterAccount
            {
                Email = "auctioneer@test.com",
                Password = "SecurePass123!",
                Regio = "Noord",
                AuthorisatieCode = "AUTH123"
            };

            // Act
            var result = controller.CreateVeilingmeesterAccount(newVeilingmeester).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var authResponse = Assert.IsType<VeilingKlokApp.Models.OutputDTOs.AuthResponse>(okResult.Value);
            Assert.Equal("auctioneer@test.com", authResponse.Email);
            Assert.Equal("Veilingmeester", authResponse.AccountType);
        }

        [Fact]
        public void CreateVeilingmeesterAccount_WithNullData_ReturnsBadRequest()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new VeilingmeesterController(context, new MockPasswordHasher(), new MockAuthService());

            // Act
            var result = controller.CreateVeilingmeesterAccount(null).Result;

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<HtppError>(badRequestResult.Value);
            Assert.Equal(400, error.code);
        }

        [Fact]
        public void CreateVeilingmeesterAccount_WithDuplicateEmail_ReturnsConflict()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new VeilingmeesterController(context, new MockPasswordHasher(), new MockAuthService());

            // Add existing veilingmeester
            var existing = new Veilingmeester
            {
                Email = "existing@test.com",
                Password = "hashed_password",
                CreatedAt = DateTime.UtcNow,
                Regio = "Zuid",
                AuthorisatieCode = "AUTH456"
            };
            context.Veilingmeesters.Add(existing);
            context.SaveChanges();

            var newVeilingmeester = new NewVeilingMeesterAccount
            {
                Email = "existing@test.com",
                Password = "NewPass123!",
                Regio = "Oost",
                AuthorisatieCode = "AUTH789"
            };

            // Act
            var result = controller.CreateVeilingmeesterAccount(newVeilingmeester).Result;

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            var error = Assert.IsType<HtppError>(conflictResult.Value);
            Assert.Equal(409, error.code);
        }

        [Fact]
        public void GetVeilingmeesterAccount_WithValidId_ReturnsVeilingmeesterDetails()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new VeilingmeesterController(context, new MockPasswordHasher(), new MockAuthService());

            var veilingmeester = new Veilingmeester
            {
                Email = "test@test.com",
                Password = "hashed_password",
                CreatedAt = DateTime.UtcNow,
                Regio = "West",
                AuthorisatieCode = "AUTH999"
            };
            context.Veilingmeesters.Add(veilingmeester);
            context.SaveChanges();

            // Act
            var result = controller.GetVeilingmeesterAccount(veilingmeester.Id).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var details = Assert.IsType<VeilingKlokApp.Models.VeilingmeesterDetails>(okResult.Value);
            Assert.Equal("test@test.com", details.Email);
            Assert.Equal("West", details.Regio);
            Assert.Equal("AUTH999", details.AuthorisatieCode);
        }

        [Fact]
        public void GetVeilingmeesterAccount_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new VeilingmeesterController(context, new MockPasswordHasher(), new MockAuthService());

            // Act
            var result = controller.GetVeilingmeesterAccount(999).Result;

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var error = Assert.IsType<HtppError>(notFoundResult.Value);
            Assert.Equal(404, error.code);
        }

        [Fact]
        public void UpdateVeilingmeesterAccount_WithValidData_ReturnsOk()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new VeilingmeesterController(context, new MockPasswordHasher(), new MockAuthService());

            var veilingmeester = new Veilingmeester
            {
                Email = "old@test.com",
                Password = "hashed_password",
                CreatedAt = DateTime.UtcNow,
                Regio = "Noord",
                AuthorisatieCode = "OLD123"
            };
            context.Veilingmeesters.Add(veilingmeester);
            context.SaveChanges();

            var updateData = new UpdateVeilingMeester
            {
                Email = "new@test.com",
                Password = "NewPassword123!",
                Regio = "Zuid",
                AuthorisatieCode = "NEW456"
            };

            // Act
            var result = controller.UpdateVeilingmeesterAccount(veilingmeester.Id, updateData).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void UpdateVeilingmeesterAccount_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new VeilingmeesterController(context, new MockPasswordHasher(), new MockAuthService());

            var updateData = new UpdateVeilingMeester
            {
                Email = "test@test.com",
                Password = "Pass123!",
                Regio = "Oost",
                AuthorisatieCode = "AUTH789"
            };

            // Act
            var result = controller.UpdateVeilingmeesterAccount(999, updateData).Result;

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<HtppError>(notFoundResult.Value);
            Assert.Equal(404, error.code);
        }

        [Fact]
        public void GetVeilingmeesterVeilingKlokken_WithValidId_ReturnsVeilingKlokkenList()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new VeilingmeesterController(context, new MockPasswordHasher(), new MockAuthService());

            var veilingmeester = new Veilingmeester
            {
                Email = "test@test.com",
                Password = "hashed_password",
                CreatedAt = DateTime.UtcNow,
                Regio = "West",
                AuthorisatieCode = "AUTH123"
            };
            context.Veilingmeesters.Add(veilingmeester);
            context.SaveChanges();

            var veilingKlok = new VeilingKlok
            {
                Naam = "Test Clock",
                DurationInSeconds = 300,
                LiveViews = 0,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(2),
                VeilingmeesterId = veilingmeester.Id
            };
            context.Veilingklokken.Add(veilingKlok);
            context.SaveChanges();

            // Act
            var result = controller.GetVeilingmeesterVeilingKlokken(veilingmeester.Id).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void GetVeilingmeesterVeilingKlokken_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new VeilingmeesterController(context, new MockPasswordHasher(), new MockAuthService());

            // Act
            var result = controller.GetVeilingmeesterVeilingKlokken(999).Result;

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var error = Assert.IsType<HtppError>(notFoundResult.Value);
            Assert.Equal(404, error.code);
        }
    }
}
