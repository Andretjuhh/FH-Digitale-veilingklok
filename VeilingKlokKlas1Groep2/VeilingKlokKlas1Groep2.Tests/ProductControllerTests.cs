using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeilingKlokApp.Controllers;
using VeilingKlokApp.Data;
using VeilingKlokApp.Models.Domain;
using VeilingKlokApp.Models.OutputDTOs;
using Xunit;

namespace VeilingKlokKlas1Groep2.Tests
{
    public class ProductControllerTests
    {
        private VeilingKlokContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<VeilingKlokContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new VeilingKlokContext(options);
        }

        [Fact]
        public void GetProduct_ReturnsProduct_WhenProductExists()
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

            var controller = new ProductController(context);

            // Act
            var actionResult = controller.GetProduct(1).Result;
            var result = actionResult.Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var product = Assert.IsType<VeilingKlokApp.Models.ProductDetails>(okResult.Value);
            Assert.Equal("Test Tulpen", product.Name);
            Assert.Equal(15.50m, product.Price);
        }

        [Fact]
        public void GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new ProductController(context);

            // Act
            var actionResult = controller.GetProduct(999).Result;
            var result = actionResult.Result;

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
