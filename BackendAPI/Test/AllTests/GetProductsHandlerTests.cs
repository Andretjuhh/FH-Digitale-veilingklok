using Application.Common.Models;
using Application.DTOs.Output;
using Application.Repositories;
using Application.UseCases.Product;
using Domain.Entities;
using Xunit;

namespace Test.Simple;

internal sealed class FakeProductRepository : IProductRepository
{
    private readonly List<(Product Product, KwekerInfo Kweker)> _items = new();

    public FakeProductRepository()
    {
        var kweker = new KwekerInfo(Guid.NewGuid(), "TestKweker", "123", "test@test.com");

        _items.Add(
            (
                new Product(10m, 100, null)
                {
                    Name = "Rozen",
                    Description = "Rode rozen",
                    ImageUrl = "image1",
                    Dimension = "M",
                    KwekerId = kweker.Id,
                },
                kweker
            )
        );

        _items.Add(
            (
                new Product(5m, 50, null)
                {
                    Name = "Tulpen",
                    Description = "Gele tulpen",
                    ImageUrl = "image2",
                    Dimension = "S",
                    KwekerId = kweker.Id,
                },
                kweker
            )
        );
    }

    public Task AddAsync(Product product)
    {
        return Task.CompletedTask;
    }

    public void Update(Product product) { }

    public Task DeleteAsync(Guid productId)
    {
        return Task.CompletedTask;
    }

    public Task<Product?> GetByIdAsync(Guid productId)
    {
        return Task.FromResult<Product?>(null);
    }

    public Task<Product?> GetByIdAsync(Guid productId, Guid kwekerId)
    {
        return Task.FromResult<Product?>(null);
    }

    public Task<(Product Product, KwekerInfo Kweker)?> GetByIdWithKwekerIdAsync(Guid productId)
    {
        return Task.FromResult<(Product, KwekerInfo)?>(null);
    }

    public Task<(Product Product, KwekerInfo Kweker)?> GetByIdWithKwekerIdAsync(
        Guid productId,
        Guid kwekerId
    )
    {
        return Task.FromResult<(Product, KwekerInfo)?>(null);
    }

    public Task<IEnumerable<Product>> GetAllByIds(List<Guid> productIds)
    {
        return Task.FromResult<IEnumerable<Product>>(Array.Empty<Product>());
    }

    public Task<IEnumerable<Product>> GetAllByVeilingKlokIdAsync(Guid veilingKlokId)
    {
        return Task.FromResult<IEnumerable<Product>>(Array.Empty<Product>());
    }

    public Task<IEnumerable<(Product Product, KwekerInfo Kweker)>> GetAllByIdsWithKwekerInfoAsync(
        List<Guid> ids
    )
    {
        return Task.FromResult<IEnumerable<(Product, KwekerInfo)>>(_items);
    }

    public Task<
        IEnumerable<(Product Product, KwekerInfo Kweker)>
    > GetAllByVeilingKlokIdWithKwekerInfoAsync(Guid veilingKlokId)
    {
        return Task.FromResult<IEnumerable<(Product, KwekerInfo)>>(_items);
    }

    public Task<
        IEnumerable<(Product Product, KwekerInfo Kweker)>
    > GetAllByOrderItemsVeilingKlokIdWithKwekerInfoAsync(Guid veilingKlokId)
    {
        return Task.FromResult<IEnumerable<(Product, KwekerInfo)>>(_items);
    }

    public Task<(
        IEnumerable<(Product Product, KwekerInfo Kweker)> Items,
        int TotalCount
    )> GetAllWithFilterAsync(
        string? nameFilter,
        string? regionFilter,
        decimal? maxPrice,
        Guid? kwekerId,
        Guid? klokId,
        int pageNumber,
        int pageSize
    )
    {
        IEnumerable<(Product Product, KwekerInfo Kweker)> query = _items;

        if (!string.IsNullOrWhiteSpace(nameFilter))
            query = query.Where(x =>
                x.Product.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase)
            );

        var list = query.ToList();
        return Task.FromResult<(IEnumerable<(Product Product, KwekerInfo Kweker)>, int)>(
            (list, list.Count)
        );
    }

    public Task<(decimal AveragePrice, int SampleCount)> GetAveragePriceAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<KwekerPriceAverage?> GetAveragePriceByKwekerAsync(Guid kwekerId, int? limit)
    {
        throw new NotImplementedException();
    }

    public Task<List<PriceHistoryItem>> GetLatestPricesAsync(int limit)
    {
        throw new NotImplementedException();
    }

    public Task<List<PriceHistoryItem>> GetLatestPricesByKwekerAsync(Guid kwekerId, int limit)
    {
        throw new NotImplementedException();
    }
}

public class GetProductsHandlerTests
{
    [Fact]
    public async Task Handle_WithFakeRepository_ReturnsMappedProducts()
    {
        var repo = new FakeProductRepository();
        var handler = new GetProductsHandler(repo);
        var query = new GetProductsQuery(null, null, null, null, null, 1, 10);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.Equal("Rozen", result.Data[0].Name);
        Assert.Equal("Tulpen", result.Data[1].Name);
    }

    [Fact]
    public async Task Handle_WithNameFilter_ReturnsOnlyMatchingProducts()
    {
        var repo = new FakeProductRepository();
        var handler = new GetProductsHandler(repo);
        var query = new GetProductsQuery("Rozen", null, null, null, null, 1, 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Data);
        Assert.Equal("Rozen", result.Data[0].Name);
    }

    [Fact]
    public async Task Handle_WithNameFilterWithoutMatches_ReturnsEmptyResult()
    {
        var repo = new FakeProductRepository();
        var handler = new GetProductsHandler(repo);
        var query = new GetProductsQuery("Onbekend", null, null, null, null, 1, 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Handle_WithDifferentPageAndSize_SetsPageAndLimitOnResult()
    {
        var repo = new FakeProductRepository();
        var handler = new GetProductsHandler(repo);
        var query = new GetProductsQuery(null, null, null, null, null, 2, 5);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.Limit);
        Assert.Equal(2, result.TotalCount);
    }
}
