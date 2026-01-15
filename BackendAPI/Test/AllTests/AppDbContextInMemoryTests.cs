using Domain.Entities;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Test.Repositories;

public class AppDbContextInMemoryTests
{
    [Fact]
    public async Task CanSaveAndLoadProduct_InMemory()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "ProductsTestDb")
            .Options;

        var productId = Guid.NewGuid();

        using (var context = new AppDbContext(options))
        {
            var product = new Product(10m, 100, null)
            {
                Id = productId,
                Name = "Rozen",
                Description = "Rode rozen",
                ImageUrl = "image",
                Dimension = "M",
                KwekerId = Guid.NewGuid(),
            };

            context.Products.Add(product);
            await context.SaveChangesAsync();
        }

        // Act
        using var verifyContext = new AppDbContext(options);
        var loaded = await verifyContext.Products.SingleAsync(p => p.Id == productId);

        // Assert
        Assert.Equal("Rozen", loaded.Name);
        Assert.Equal(10m, loaded.MinimumPrice);
    }

    [Fact]
    public async Task CanSaveOrderWithItems_AndCascadeDeletesItems()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "OrdersWithItemsTestDb")
            .Options;

        Guid orderId;
        Guid productId;

        using (var context = new AppDbContext(options))
        {
            var kwekerId = Guid.NewGuid();
            var koperId = Guid.NewGuid();
            var veilingKlokId = Guid.NewGuid();

            var product = new Product(5m, 50, null)
            {
                Id = Guid.NewGuid(),
                Name = "Tulp",
                Description = "Gele tulp",
                ImageUrl = "image",
                Dimension = "S",
                KwekerId = kwekerId,
            };
            product.AddToVeilingKlok(veilingKlokId);

            var order = new Order(koperId) { Id = Guid.NewGuid(), VeilingKlokId = veilingKlokId };

            var orderItem = new OrderItem(7.5m, 3, product, order.Id)
            {
                VeilingKlokId = veilingKlokId,
            };

            order.AddItem(orderItem);

            context.Products.Add(product);
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            orderId = order.Id;
            productId = product.Id;
        }

        using (var verifyContext = new AppDbContext(options))
        {
            var loadedOrder = await verifyContext
                .Orders.Include(o => o.OrderItems)
                .SingleAsync(o => o.Id == orderId);

            Assert.Single(loadedOrder.OrderItems);
            var item = loadedOrder.OrderItems.First();
            Assert.Equal(3, item.Quantity);
            Assert.Equal(productId, item.ProductId);
        }

        using (var deleteContext = new AppDbContext(options))
        {
            var orderToDelete = await deleteContext
                .Orders.Include(o => o.OrderItems)
                .SingleAsync(o => o.Id == orderId);

            deleteContext.Orders.Remove(orderToDelete);
            await deleteContext.SaveChangesAsync();
        }

        using var finalContext = new AppDbContext(options);
        Assert.Empty(finalContext.OrderItems.Where(oi => oi.OrderId == orderId));
    }

    [Fact]
    public async Task CanSaveKwekerWithProducts_AndQueryByKweker()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "KwekerProductsTestDb")
            .Options;

        Guid kwekerId;

        using (var context = new AppDbContext(options))
        {
            kwekerId = Guid.NewGuid();

            var product1 = new Product(2m, 20, null)
            {
                Id = Guid.NewGuid(),
                Name = "Narcis",
                Description = "Gele narcis",
                ImageUrl = "img1",
                Dimension = "S",
                KwekerId = kwekerId,
            };

            var product2 = new Product(3m, 30, null)
            {
                Id = Guid.NewGuid(),
                Name = "Lelie",
                Description = "Witte lelie",
                ImageUrl = "img2",
                Dimension = "M",
                KwekerId = kwekerId,
            };

            context.Products.AddRange(product1, product2);
            await context.SaveChangesAsync();
        }

        using var verifyContext = new AppDbContext(options);
        var productsForKweker = await verifyContext
            .Products.Where(p => p.KwekerId == kwekerId)
            .OrderBy(p => p.Name)
            .ToListAsync();

        Assert.Equal(2, productsForKweker.Count);
        Assert.Collection(
            productsForKweker,
            p => Assert.Equal("Lelie", p.Name),
            p => Assert.Equal("Narcis", p.Name)
        );
    }
}
