using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeilingKlokApp.Controllers;
using VeilingKlokApp.Data;
using VeilingKlokApp.Models.Domain;
using VeilingKlokApp.Models.OutputDTOs;
using VeilingKlokKlas1Groep2.Declarations;
using Xunit;

namespace VeilingKlokKlas1Groep2.Tests
{
    public class ProductControllerTests
    {
        private VeilingKlokContext CreateInMemoryContext() //in memory database aanmaken
        {
            var options = new DbContextOptionsBuilder<VeilingKlokContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            return new VeilingKlokContext(options);
        }

        [Fact]
        public void GetProduct_ReturnsProduct_WhenProductExists() //Test voor het ophalen van een bestaand product
        {
            // Arrange
            using var context = CreateInMemoryContext();
            context.Products.Add(new Product
            {
                Id = 1,
                Name = "Test Tulpen",
                Description = "Mooie rode tulpen",
                Price = 15.50m,
                MinimumPrice = 12.00m,
                Quantity = 10,
                KwekerId = 1
            });
            context.SaveChanges();

            var controller = new ProductController(context); // Controller met nieuwe database

            // Act
            var actionResult = controller.GetProduct(1).Result;
            var result = actionResult.Result;

            // Assert
            //Check of het resultaat klopt
            var okResult = Assert.IsType<OkObjectResult>(result);
            var product = Assert.IsType<VeilingKlokApp.Models.ProductDetails>(okResult.Value);
            Assert.Equal("Test Tulpen", product.Name);
            Assert.Equal(15.50m, product.Price);
        }

        [Fact]
        public void GetProduct_ReturnsNotFound_WhenProductDoesNotExist() //Test voor het ophalen van een niet-bestaand product
        {
            // Arrange
            // Maakt een lege database aan
            using var context = CreateInMemoryContext();
            var controller = new ProductController(context);

            // Act
            // Vraagt een product op dat niet bestaat
            var actionResult = controller.GetProduct(999).Result;
            var result = actionResult.Result;

            // Assert
            // Controleert of het resultaat een NotFound is
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetAllProducts_ReturnsAllProducts()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var kweker = new Kweker
            {
                Email = "kweker@test.com",
                Password = "hashed_password",
                CreatedAt = DateTime.UtcNow,
                Name = "Test Kweker",
                Telephone = "0612345678"
            };
            context.Kwekers.Add(kweker);
            context.SaveChanges();

            context.Products.AddRange(
                new Product { Name = "Tulpen", Price = 10m, MinimumPrice = 8m, Quantity = 100, KwekerId = kweker.Id },
                new Product { Name = "Rozen", Price = 15m, MinimumPrice = 12m, Quantity = 50, KwekerId = kweker.Id }
            );
            context.SaveChanges();

            var controller = new ProductController(context);

            // Act
            var actionResult = controller.GetAllProducts(null).Result;
            var result = actionResult.Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void GetAllProducts_FilteredByKweker_ReturnsFilteredProducts()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var kweker1 = new Kweker { Email = "k1@test.com", Password = "pass", CreatedAt = DateTime.UtcNow, Name = "Kweker 1", Telephone = "0611111111" };
            var kweker2 = new Kweker { Email = "k2@test.com", Password = "pass", CreatedAt = DateTime.UtcNow, Name = "Kweker 2", Telephone = "0622222222" };
            context.Kwekers.AddRange(kweker1, kweker2);
            context.SaveChanges();

            context.Products.AddRange(
                new Product { Name = "Product1", Price = 10m, MinimumPrice = 8m, Quantity = 10, KwekerId = kweker1.Id },
                new Product { Name = "Product2", Price = 20m, MinimumPrice = 15m, Quantity = 20, KwekerId = kweker2.Id }
            );
            context.SaveChanges();

            var controller = new ProductController(context);

            // Act
            var actionResult = controller.GetAllProducts(kweker1.Id).Result;
            var result = actionResult.Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}
