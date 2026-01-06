using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeilingKlokApp.Controllers;
using VeilingKlokApp.Data;
using VeilingKlokApp.Models.Domain;
using VeilingKlokApp.Models.OutputDTOs;
using VeilingKlokKlas1Groep2.Declarations;
using VeilingKlokKlas1Groep2.Models.InputDTOs;
using Xunit;

namespace VeilingKlokKlas1Groep2.Tests
{
    public class VeilingKlokControllerTests
    {
        private VeilingKlokContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<VeilingKlokContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            return new VeilingKlokContext(options);
        }

        [Fact]
        public void CreateVeilingKlok_WithValidData_ReturnsCreated()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new VeilingKlokController(context);

            // Create a veilingmeester first
            var veilingmeester = new Veilingmeester
            {
                Email = "test@test.com",
                Password = "hashed_password",
                CreatedAt = DateTime.UtcNow,
                Regio = "Noord",
                AuthorisatieCode = "AUTH123"
            };
            context.Veilingmeesters.Add(veilingmeester);
            context.SaveChanges();

            var newVeilingKlok = new NewVeilingKlok
            {
                Naam = "Test Auction Clock",
                DurationInSeconds = 300,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(2),
                VeilingmeesterId = veilingmeester.Id
            };

            // Act
            var result = controller.CreateVeilingKlok(newVeilingKlok).Result;

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.NotNull(createdResult.Value);
        }

        [Fact]
        public void CreateVeilingKlok_WithNullData_ReturnsBadRequest()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new VeilingKlokController(context);

            // Act
            var result = controller.CreateVeilingKlok(null).Result;

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<HtppError>(badRequestResult.Value);
            Assert.Equal(400, error.code);
        }

        [Fact]
        public void CreateVeilingKlok_WithInvalidDuration_ReturnsBadRequest()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new VeilingKlokController(context);

            var newVeilingKlok = new NewVeilingKlok
            {
                Naam = "Test Clock",
                DurationInSeconds = -10, // Invalid
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(2),
                VeilingmeesterId = 1
            };

            // Act
            var result = controller.CreateVeilingKlok(newVeilingKlok).Result;

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<HtppError>(badRequestResult.Value);
            Assert.Equal(400, error.code);
        }

        [Fact]
        public void CreateVeilingKlok_WithInvalidTimeRange_ReturnsBadRequest()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new VeilingKlokController(context);

            var newVeilingKlok = new NewVeilingKlok
            {
                Naam = "Test Clock",
                DurationInSeconds = 300,
                StartTime = DateTime.UtcNow.AddHours(2),
                EndTime = DateTime.UtcNow, // End before start
                VeilingmeesterId = 1
            };

            // Act
            var result = controller.CreateVeilingKlok(newVeilingKlok).Result;

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<HtppError>(badRequestResult.Value);
            Assert.Equal(400, error.code);
        }

        [Fact]
        public void CreateVeilingKlok_WithNonExistentVeilingmeester_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new VeilingKlokController(context);

            var newVeilingKlok = new NewVeilingKlok
            {
                Naam = "Test Clock",
                DurationInSeconds = 300,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(2),
                VeilingmeesterId = 999 // Non-existent
            };

            // Act
            var result = controller.CreateVeilingKlok(newVeilingKlok).Result;

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<HtppError>(notFoundResult.Value);
            Assert.Equal(404, error.code);
        }

        [Fact]
        public void GetVeilingKlok_WithValidId_ReturnsVeilingKlokDetails()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new VeilingKlokController(context);

            var veilingKlok = new VeilingKlok
            {
                Naam = "Test Clock",
                DurationInSeconds = 300,
                LiveViews = 0,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(2),
                VeilingmeesterId = 1
            };
            context.Veilingklokken.Add(veilingKlok);
            context.SaveChanges();

            // Act
            var result = controller.GetVeilingKlok(veilingKlok.Id).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var details = Assert.IsType<VeilingKlokDetails>(okResult.Value);
            Assert.Equal("Test Clock", details.Naam);
            Assert.Equal(300, details.DurationInSeconds);
        }

        [Fact]
        public void GetVeilingKlok_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new VeilingKlokController(context);

            // Act
            var result = controller.GetVeilingKlok(999).Result;

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var error = Assert.IsType<HtppError>(notFoundResult.Value);
            Assert.Equal(404, error.code);
        }

        [Fact]
        public void GetAllVeilingKlokken_ReturnsAllClocks()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new VeilingKlokController(context);

            var clock1 = new VeilingKlok
            {
                Naam = "Clock 1",
                DurationInSeconds = 300,
                LiveViews = 0,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(2),
                VeilingmeesterId = 1
            };
            var clock2 = new VeilingKlok
            {
                Naam = "Clock 2",
                DurationInSeconds = 400,
                LiveViews = 0,
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(3),
                VeilingmeesterId = 1
            };
            context.Veilingklokken.AddRange(clock1, clock2);
            context.SaveChanges();

            // Act
            var result = controller.GetAllVeilingKlokken(null, false).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void GetAllVeilingKlokken_FilteredByVeilingmeester_ReturnsFilteredClocks()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new VeilingKlokController(context);

            var clock1 = new VeilingKlok
            {
                Naam = "Clock 1",
                DurationInSeconds = 300,
                LiveViews = 0,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(2),
                VeilingmeesterId = 1
            };
            var clock2 = new VeilingKlok
            {
                Naam = "Clock 2",
                DurationInSeconds = 400,
                LiveViews = 0,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(3),
                VeilingmeesterId = 2
            };
            context.Veilingklokken.AddRange(clock1, clock2);
            context.SaveChanges();

            // Act
            var result = controller.GetAllVeilingKlokken(1, false).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void UpdateVeilingKlok_WithValidData_ReturnsOk()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new VeilingKlokController(context);

            var veilingKlok = new VeilingKlok
            {
                Naam = "Old Name",
                DurationInSeconds = 300,
                LiveViews = 0,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(2),
                VeilingmeesterId = 1
            };
            context.Veilingklokken.Add(veilingKlok);
            context.SaveChanges();

            var updateData = new UpdateVeilingKlok
            {
                Naam = "New Name",
                DurationInSeconds = 400,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(3)
            };

            // Act
            var result = controller.UpdateVeilingKlok(veilingKlok.Id, updateData).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void UpdateVeilingKlok_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryContext();
            var controller = new VeilingKlokController(context);

            var updateData = new UpdateVeilingKlok
            {
                Naam = "Test",
                DurationInSeconds = 300,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(2)
            };

            // Act
            var result = controller.UpdateVeilingKlok(999, updateData).Result;

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<HtppError>(notFoundResult.Value);
            Assert.Equal(404, error.code);
        }
    }
}
