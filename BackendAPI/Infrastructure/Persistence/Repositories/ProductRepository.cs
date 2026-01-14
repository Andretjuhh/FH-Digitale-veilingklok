using Application.Common.Models;
using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

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

    public async Task<List<PriceHistoryItem>> GetLatestPricesByKwekerAsync(Guid kwekerId, int limit)
    {
        const string sql = @"
SELECT TOP (@limit)
    p.id AS product_id,
    p.name AS product_name,
    k.id AS kweker_id,
    k.company_name AS kweker_name,
    oi.price_at_purchase AS price_at_purchase,
    oi.created_at AS purchased_at
FROM OrderItem oi
JOIN Product p ON oi.product_id = p.id
JOIN Kweker k ON p.kweker_id = k.id
WHERE p.kweker_id = @kwekerId
ORDER BY oi.created_at DESC;";

        return await ExecutePriceHistoryQueryAsync(sql, new Dictionary<string, object>
        {
            ["@limit"] = limit,
            ["@kwekerId"] = kwekerId
        });
    }

    public async Task<List<PriceHistoryItem>> GetLatestPricesAsync(int limit)
    {
        const string sql = @"
SELECT TOP (@limit)
    p.id AS product_id,
    p.name AS product_name,
    k.id AS kweker_id,
    k.company_name AS kweker_name,
    oi.price_at_purchase AS price_at_purchase,
    oi.created_at AS purchased_at
FROM OrderItem oi
JOIN Product p ON oi.product_id = p.id
JOIN Kweker k ON p.kweker_id = k.id
ORDER BY oi.created_at DESC;";

        return await ExecutePriceHistoryQueryAsync(sql, new Dictionary<string, object>
        {
            ["@limit"] = limit
        });
    }

    public async Task<KwekerPriceAverage?> GetAveragePriceByKwekerAsync(Guid kwekerId, int? limit)
    {
        var sql = @"
SELECT
    k.id AS kweker_id,
    k.company_name AS kweker_name,
    AVG(CAST(oi.price_at_purchase AS decimal(18,2))) AS avg_price,
    COUNT(1) AS sample_count
FROM OrderItem oi
JOIN Product p ON oi.product_id = p.id
JOIN Kweker k ON p.kweker_id = k.id
WHERE p.kweker_id = @kwekerId
GROUP BY k.id, k.company_name;";

        var parameters = new Dictionary<string, object>
        {
            ["@kwekerId"] = kwekerId
        };

        if (limit.HasValue && limit.Value > 0)
        {
            sql = @"
WITH Recent AS (
    SELECT TOP (@limit)
        oi.price_at_purchase AS price_at_purchase
    FROM OrderItem oi
    JOIN Product p ON oi.product_id = p.id
    WHERE p.kweker_id = @kwekerId
    ORDER BY oi.created_at DESC
)
SELECT
    k.id AS kweker_id,
    k.company_name AS kweker_name,
    AVG(CAST(r.price_at_purchase AS decimal(18,2))) AS avg_price,
    COUNT(1) AS sample_count
FROM Recent r
JOIN Kweker k ON k.id = @kwekerId
GROUP BY k.id, k.company_name;";
            parameters["@limit"] = limit.Value;
        }

        return await ExecuteKwekerAverageQueryAsync(sql, parameters);
    }

    public async Task<(decimal AveragePrice, int SampleCount)> GetAveragePriceAllAsync()
    {
        const string sql = @"
SELECT
    AVG(CAST(oi.price_at_purchase AS decimal(18,2))) AS avg_price,
    COUNT(1) AS sample_count
FROM OrderItem oi;";

        var connection = _dbContext.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;
        if (shouldClose)
            await connection.OpenAsync();

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return (0, 0);

            var avg = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
            var count = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
            return (avg, count);
        }
        finally
        {
            if (shouldClose)
                await connection.CloseAsync();
        }
    }

    private async Task<List<PriceHistoryItem>> ExecutePriceHistoryQueryAsync(
        string sql,
        Dictionary<string, object> parameters
    )
    {
        var connection = _dbContext.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;
        if (shouldClose)
            await connection.OpenAsync();

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            foreach (var (key, value) in parameters)
            {
                var p = command.CreateParameter();
                p.ParameterName = key;
                p.Value = value;
                command.Parameters.Add(p);
            }

            var results = new List<PriceHistoryItem>();
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new PriceHistoryItem(
                    reader.GetGuid(0),
                    reader.GetString(1),
                    reader.GetGuid(2),
                    reader.GetString(3),
                    reader.GetDecimal(4),
                    reader.GetFieldValue<DateTimeOffset>(5)
                ));
            }

            return results;
        }
        finally
        {
            if (shouldClose)
                await connection.CloseAsync();
        }
    }

    private async Task<KwekerPriceAverage?> ExecuteKwekerAverageQueryAsync(
        string sql,
        Dictionary<string, object> parameters
    )
    {
        var connection = _dbContext.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;
        if (shouldClose)
            await connection.OpenAsync();

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            foreach (var (key, value) in parameters)
            {
                var p = command.CreateParameter();
                p.ParameterName = key;
                p.Value = value;
                command.Parameters.Add(p);
            }

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            var kwekerId = reader.GetGuid(0);
            var kwekerName = reader.GetString(1);
            var avg = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2);
            var count = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);

            return new KwekerPriceAverage(kwekerId, kwekerName, avg, count);
        }
        finally
        {
            if (shouldClose)
                await connection.CloseAsync();
        }
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
        var query = _dbContext
            .Products.Join(
                _dbContext.Kwekers,
                product => product.KwekerId,
                kweker => kweker.Id,
                (product, kweker) => new { product, kweker }
            );

        if (!string.IsNullOrWhiteSpace(nameFilter))
            query = query.Where(x => x.product.Name.Contains(nameFilter));

        if (maxPrice.HasValue)
            query = query.Where(x => x.product.AuctionPrice <= maxPrice.Value);

        if (kwekerId.HasValue)
            query = query.Where(x => x.product.KwekerId == kwekerId.Value);

        var totalCount = await query.CountAsync();

        var results = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        var items = results.Select(r => (r.product, new KwekerInfo(r.kweker.Id, r.kweker.CompanyName)));

        return (items, totalCount);
    }

    #endregion
}
