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
        var product = await _dbContext.Products.FindAsync(productId);
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
        return await _dbContext
            .VeilingKlokProducts.Where(vkp => vkp.VeilingKlokId == veilingKlokId)
            .OrderBy(vkp => vkp.Position)
            .Join(
                _dbContext.Products,
                vkp => vkp.ProductId,
                product => product.Id,
                (vkp, product) => product
            )
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByKwekerIdAsync(Guid kwekerId)
    {
        return await _dbContext.Products.Where(p => p.KwekerId == kwekerId).ToListAsync();
    }

    #region Queries with Kweker Info

    public async Task<(Product Product, KwekerInfo Kweker)?> GetByIdWithKwekerIdAsync(
        Guid productId
    )
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

    public async Task<(Product Product, KwekerInfo Kweker)?> GetByIdWithKwekerIdAsync(
        Guid productId,
        Guid kwekerId
    )
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

    public async Task<
        IEnumerable<(Product Product, KwekerInfo Kweker)>
    > GetAllByIdsWithKwekerInfoAsync(List<Guid> ids)
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

    public async Task<IEnumerable<(Product Product, KwekerInfo Kweker)>> GetAllByVeilingKlokIdWithKwekerInfoAsync(Guid veilingKlokId)
    {
        var results = await _dbContext
            .Products.Join(
                _dbContext.Kwekers,
                product => product.KwekerId,
                kweker => kweker.Id,
                (product, kweker) => new { product, kweker }
            )
            .Where(x => x.product.VeilingKlokId == veilingKlokId)
            .ToListAsync();

        return results.Select(r => (r.product, new KwekerInfo(r.kweker.Id, r.kweker.CompanyName)));
    }

    public async Task<IEnumerable<(Product Product, KwekerInfo Kweker)>> GetAllByOrderItemsVeilingKlokIdWithKwekerInfoAsync(Guid veilingKlokId)
    {
        var results = await _dbContext
            .OrderItems.Where(oi => oi.VeilingKlokId == veilingKlokId)
            .Select(oi => oi.ProductId)
            .Distinct()
            .Join(_dbContext.Products, productId => productId, product => product.Id, (_, product) => product)
            .Join(
                _dbContext.Kwekers,
                product => product.KwekerId,
                kweker => kweker.Id,
                (product, kweker) => new { product, kweker }
            )
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
        // 1. Filter on Products table first (Efficiency: avoid joining full table for count)
        var query = _dbContext.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(nameFilter))
            query = query.Where(p => p.Name.Contains(nameFilter));

        if (!string.IsNullOrWhiteSpace(regionFilter))
            query = query.Where(p => p.Region == regionFilter);

        if (maxPrice.HasValue)
            query = query.Where(p => p.AuctionPrice <= maxPrice.Value);

        if (kwekerId.HasValue)
            query = query.Where(p => p.KwekerId == kwekerId.Value);

        if (klokId.HasValue)
            query = query.Where(p => p.VeilingKlokId == klokId.Value);

        // 2. Count filtered items
        var totalCount = await query.CountAsync();

        // 3. Apply Sorting and Paging
        var pagedProducts = query
            .OrderByDescending(p => p.CreatedAt)
            .ThenBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        // 4. Join Kweker info ONLY for the paged results
        var results = await pagedProducts
            .Join(
                _dbContext.Kwekers.AsNoTracking(),
                product => product.KwekerId,
                kweker => kweker.Id,
                (product, kweker) => new { product, kweker }
            )
            .ToListAsync();

        var items = results.Select(r =>
            (r.product, new KwekerInfo(r.kweker.Id, r.kweker.CompanyName))
        );

        return (items, totalCount);
    }

    #endregion
}
