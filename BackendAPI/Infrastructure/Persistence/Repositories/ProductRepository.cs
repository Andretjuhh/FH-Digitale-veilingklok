using Application.Common.Models;
using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _dbContext;

    public ProductRepository(AppDbContext context)
    {
        _dbContext = context;
    }

    public async Task AddAsync(Product product)
    {
        await _dbContext.Products.AddAsync(product);
    }

    public void Update(Product product)
    {
        _dbContext.Products.Update(product);
    }

    public async Task DeleteAsync(Guid productId)
    {
        var product =
            await _dbContext.Products.FindAsync(productId);
        if (product != null)
            _dbContext.Products.Remove(product);
    }

    public async Task<Product?> GetByIdAsync(Guid productId)
    {
        return await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
    }

    public async Task<Product?> GetByIdAsync(Guid productId, Guid kwekerId)
    {
        return await _dbContext.Products.FirstOrDefaultAsync(p =>
            p.Id == productId && p.KwekerId == kwekerId
        );
    }

    public async Task<IEnumerable<Product>> GetAllByIds(List<Guid> productIds)
    {
        return await _dbContext.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByKwekerIdAsync(Guid kwekerId)
    {
        return await _dbContext.Products.Where(p => p.KwekerId == kwekerId).ToListAsync();
    }

    #region Queries with Kweker Info

    // Queries that return Product along with partial Kweker info
    private IQueryable<(Product Product, KwekerInfo Kweker)> QueryProductWithKweker()
    {
        return _dbContext
            .Products.Join(
                _dbContext.Kwekers,
                product => product.KwekerId,
                kweker => kweker.Id,
                (product, kweker) =>
                    new { product, partialKweker = new KwekerInfo(kweker.Id, kweker.CompanyName) }
            )
            .Select(x => ValueTuple.Create(x.product, x.partialKweker));
    }

    public async Task<(Product Product, KwekerInfo Kweker)?> GetByIdWithKwekerIdAsync(Guid productId)
    {
        var result = await QueryProductWithKweker().FirstOrDefaultAsync(x => x.Product.Id == productId);
        if (result.Product == null)
            return null;
        return result;
    }

    public async Task<(Product Product, KwekerInfo Kweker)?> GetByIdWithKwekerIdAsync(Guid productId, Guid kwekerId)
    {
        var result = await QueryProductWithKweker()
            .FirstOrDefaultAsync(x => x.Product.Id == productId && x.Product.KwekerId == kwekerId);

        if (result.Product == null)
            return null;

        return result;
    }

    public async Task<IEnumerable<(Product Product, KwekerInfo Kweker)>> GetAllByIdsWithKwekerInfoAsync(List<Guid> ids)
    {
        return await QueryProductWithKweker().Where(x => ids.Contains(x.Product.Id)).ToListAsync();
    }

    public async Task<(
        IEnumerable<(Product Product, KwekerInfo Kweker)> Items,
        int TotalCount
        )> GetAllWithFilterAsync(
        string? nameFilter,
        decimal? maxPrice,
        Guid? kwekerId,
        int pageNumber,
        int pageSize
    )
    {
        var query = QueryProductWithKweker();

        if (!string.IsNullOrWhiteSpace(nameFilter))
            query = query.Where(x => x.Product.Name.Contains(nameFilter));

        if (maxPrice.HasValue)
            query = query.Where(x => x.Product.AuctionPrice <= maxPrice.Value);

        if (kwekerId.HasValue)
            query = query.Where(x => x.Product.KwekerId == kwekerId.Value);

        var totalCount = await query.CountAsync();

        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    #endregion
}