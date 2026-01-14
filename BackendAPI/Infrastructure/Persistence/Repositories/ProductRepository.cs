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

    public async Task<IEnumerable<Product>> GetAllByVeilingKlokIdAsync(Guid veilingKlokId)
    {
        return await _dbContext.Products.Where(p => p.VeilingKlokId == veilingKlokId).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByKwekerIdAsync(Guid kwekerId)
    {
        return await _dbContext.Products.Where(p => p.KwekerId == kwekerId).ToListAsync();
    }

    #region Queries with Kweker Info

    public async Task<(Product Product, KwekerInfo Kweker)?> GetByIdWithKwekerIdAsync(Guid productId)
    {
        var result = await _dbContext
            .Products.Join(
                _dbContext.Kwekers,
                product => product.KwekerId,
                kweker => kweker.Id,
                (product, kweker) => new { product, kweker }
            )
            .Where(x => x.product.Id == productId)
            .FirstOrDefaultAsync();

        if (result == null)
            return null;

        return (result.product, new KwekerInfo(result.kweker.Id, result.kweker.CompanyName));
    }

    public async Task<(Product Product, KwekerInfo Kweker)?> GetByIdWithKwekerIdAsync(Guid productId, Guid kwekerId)
    {
        var result = await _dbContext
            .Products.Join(
                _dbContext.Kwekers,
                product => product.KwekerId,
                kweker => kweker.Id,
                (product, kweker) => new { product, kweker }
            )
            .Where(x => x.product.Id == productId && x.product.KwekerId == kwekerId)
            .FirstOrDefaultAsync();

        if (result == null)
            return null;

        return (result.product, new KwekerInfo(result.kweker.Id, result.kweker.CompanyName));
    }

    public async Task<IEnumerable<(Product Product, KwekerInfo Kweker)>> GetAllByIdsWithKwekerInfoAsync(List<Guid> ids)
    {
        var results = await _dbContext
            .Products.Join(
                _dbContext.Kwekers,
                product => product.KwekerId,
                kweker => kweker.Id,
                (product, kweker) => new { product, kweker }
            )
            .Where(x => ids.Contains(x.product.Id))
            .ToListAsync();

        return results.Select(r => (r.product, new KwekerInfo(r.kweker.Id, r.kweker.CompanyName)));
    }

    public async Task<(
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
        var query = _dbContext
            .Products.Join(
                _dbContext.Kwekers,
                product => product.KwekerId,
                kweker => kweker.Id,
                (product, kweker) => new { product, kweker }
            );

        if (!string.IsNullOrWhiteSpace(nameFilter))
            query = query.Where(x => x.product.Name.Contains(nameFilter));

        if (!string.IsNullOrWhiteSpace(regionFilter))
            query = query.Where(x => x.product.Region == regionFilter);

        if (maxPrice.HasValue)
            query = query.Where(x => x.product.AuctionPrice <= maxPrice.Value);

        if (kwekerId.HasValue)
            query = query.Where(x => x.product.KwekerId == kwekerId.Value);

        if (klokId.HasValue)
            query = query.Where(x => x.product.VeilingKlokId == klokId.Value);

        var totalCount = await query.CountAsync();
        var results = await query
            .OrderByDescending(x => x.product.CreatedAt)
            .ThenBy(x => x.product.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var items = results.Select(r => (r.product, new KwekerInfo(r.kweker.Id, r.kweker.CompanyName)));

        return (items, totalCount);
    }

    #endregion
}