using System;
using Domain.Entities;
using Domain.Exceptions;
using Xunit;

namespace Test.Domain;

public class ProductDomainTests
{
    [Fact]
    public void Ctor_WithNegativeMinimumPrice_ThrowsInvalidMinimumPrice()
    {
        Assert.Throws<ProductValidationException>(() =>
            new Product(-1m, 10, null)
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                Description = "Desc",
                ImageUrl = "img",
                KwekerId = Guid.NewGuid(),
            }
        );
    }

    [Fact]
    public void Ctor_WithNegativeStock_ThrowsNegativeStockValue()
    {
        Assert.Throws<ProductValidationException>(() =>
            new Product(10m, -1, null)
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                Description = "Desc",
                ImageUrl = "img",
                KwekerId = Guid.NewGuid(),
            }
        );
    }

    [Fact]
    public void DecreaseStock_BelowZero_ThrowsInsufficientProductStock()
    {
        var product = new Product(10m, 1, null)
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Description = "Desc",
            ImageUrl = "img",
            KwekerId = Guid.NewGuid(),
        };

        Assert.Throws<ProductValidationException>(() => product.DecreaseStock(2));
    }

    [Fact]
    public void UpdateAuctionPrice_BelowMinimum_ThrowsInvalidProductPrice()
    {
        var product = new Product(10m, 10, null)
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Description = "Desc",
            ImageUrl = "img",
            KwekerId = Guid.NewGuid(),
        };

        Assert.Throws<ProductValidationException>(() => product.UpdateAuctionPrice(5m));
    }
}
