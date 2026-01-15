using System;
using Domain.Entities;
using Domain.Exceptions;
using Xunit;

namespace Test.Domain;

public class OrderItemDomainTests
{
    private static Product CreateValidProduct()
    {
        var product = new Product(10m, 100, null)
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Description = "Desc",
            ImageUrl = "img",
            KwekerId = Guid.NewGuid(),
        };
        product.AddToVeilingKlok(Guid.NewGuid());
        return product;
    }

    [Fact]
    public void Ctor_WithQuantityLessThanOne_ThrowsMinOrderQuantityOne()
    {
        var product = CreateValidProduct();

        Assert.Throws<OrderValidationException>(() =>
            new OrderItem(10m, 0, product, Guid.NewGuid()) { VeilingKlokId = Guid.NewGuid() }
        );
    }

    [Fact]
    public void Ctor_WithPriceBelowMinimum_ThrowsInvalidProductPrice()
    {
        var product = CreateValidProduct();

        Assert.Throws<OrderValidationException>(() =>
            new OrderItem(5m, 1, product, Guid.NewGuid()) { VeilingKlokId = Guid.NewGuid() }
        );
    }

    [Fact]
    public void UpdateQuantity_WithLessThanOne_ThrowsMinOrderQuantityOne()
    {
        var product = CreateValidProduct();
        var item = new OrderItem(10m, 1, product, Guid.NewGuid())
        {
            VeilingKlokId = Guid.NewGuid(),
        };

        Assert.Throws<OrderValidationException>(() => item.UpdateQuantity(0));
    }
}
