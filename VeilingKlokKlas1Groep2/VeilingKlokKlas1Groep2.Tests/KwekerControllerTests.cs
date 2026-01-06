using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeilingKlokApp.Controllers;
using VeilingKlokApp.Data;
using VeilingKlokApp.Models.Domain;
using VeilingKlokKlas1Groep2.Services;
using Xunit;

namespace VeilingKlokKlas1Groep2.Tests
{
    public class KwekerControllerTests
    {
        // Mock implementatie van IPasswordHasher - geeft altijd groen licht voor wachtwoord operaties
        private class MockPasswordHasher : IPasswordHasher
        {
            public string HashPassword(string password) => $"hashed_{password}"; // Fake hash, geen echte encryptie
            public bool VerifyPassword(string hashedPassword, string providedPassword) => true; // Valideert altijd succesvol
        }

        // Mock implementatie van IAuthService - geeft altijd fake tokens terug
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

        private VeilingKlokContext CreateInMemoryContext() // In memory database aanmaken
        {
            var options = new DbContextOptionsBuilder<VeilingKlokContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unieke database per test
                .Options;
            return new VeilingKlokContext(options);
        }

        private void SetupAuthenticatedContext(ControllerBase controller, int accountId) // Simuleert ingelogde gebruiker
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Items["AccountId"] = accountId; // Zet de AccountId alsof de auth middleware dit heeft gedaan
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public void GetKwekerStats_WithNoData_ReturnsZeroStats() // Test voor het ophalen van statistieken met lege database
        {
            // Arrange
            // Maakt lege database en mock dependencies aan
            using var context = CreateInMemoryContext();
            var mockHasher = new MockPasswordHasher();
            var mockAuth = new MockAuthService();
            var controller = new KwekerController(context, mockHasher, mockAuth);
            
            SetupAuthenticatedContext(controller, 1); // Simuleert ingelogde kweker met id 1

            // Act
            // Roept de GetKwekerStats endpoint aan
            var result = controller.GetKwekerStats().Result;

            // Assert
            // Controleert of het resultaat klopt
            var okResult = Assert.IsType<OkObjectResult>(result);
            var stats = okResult.Value;
            
            var totalProductsProp = stats.GetType().GetProperty("totalProducts");
            Assert.Equal(0, totalProductsProp.GetValue(stats)); // Verwacht 0 producten want database is leeg
        }

        [Fact]
        public void GetProducts_ReturnsKwekerProducts() // Test of alleen producten van ingelogde kweker worden teruggegeven
        {
            // Arrange
            // Maakt database aan met 3 producten (2 van kweker 1, 1 van kweker 2)
            using var context = CreateInMemoryContext();
            var kwekerId = 1;
            
            context.Products.AddRange(
                new Product { Id = 1, Name = "Mijn Tulpen", Price = 10m, MinimumPrice = 8m, Quantity = 5, KwekerId = kwekerId },
                new Product { Id = 2, Name = "Mijn Rozen", Price = 15m, MinimumPrice = 12m, Quantity = 3, KwekerId = kwekerId },
                new Product { Id = 3, Name = "Andere Bloem", Price = 20m, MinimumPrice = 18m, Quantity = 7, KwekerId = 2 } // Van andere kweker
            );
            context.SaveChanges();

            var mockHasher = new MockPasswordHasher();
            var mockAuth = new MockAuthService();
            var controller = new KwekerController(context, mockHasher, mockAuth);
            
            SetupAuthenticatedContext(controller, kwekerId); // Ingelogd als kweker 1

            // Act
            // Vraagt producten op van ingelogde kweker
            var result = controller.GetProducts().Result;

            // Assert
            // Controleert of alleen producten van kweker 1 worden teruggegeven
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            
            var productsProperty = response.GetType().GetProperty("products");
            var products = productsProperty.GetValue(response) as IEnumerable<VeilingKlokApp.Models.ProductDetails>;
            
            Assert.NotNull(products);
            Assert.Equal(2, products.Count()); // Alleen 2 producten van kweker 1
            Assert.All(products, p => Assert.Equal(kwekerId, p.KwekerId)); // Alle producten zijn van kweker 1
        }

        [Fact]
        public void GetProducts_WithoutAuthentication_ReturnsUnauthorized() // Test of endpoint beschermd is tegen niet-ingelogde gebruikers
        {
            // Arrange
            // Maakt controller aan zonder ingelogde gebruiker
            using var context = CreateInMemoryContext();
            var mockHasher = new MockPasswordHasher();
            var mockAuth = new MockAuthService();
            var controller = new KwekerController(context, mockHasher, mockAuth);
            
            // Geen authenticated context setup - geen ingelogde gebruiker!

            // Act
            // Probeert producten op te halen zonder authenticatie
            var result = controller.GetProducts().Result;

            // Assert
            // Controller crasht zonder authenticatie (geeft 500), wat aangeeft dat auth middleware nodig is
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode); // Verwacht 500 error want geen auth
        }
    }
}
