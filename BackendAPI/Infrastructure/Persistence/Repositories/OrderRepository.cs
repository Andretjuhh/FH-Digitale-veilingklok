using Application.Common.Exceptions;
using Application.Common.Models;
using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _dbContext;

    public OrderRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Order order)
    {
        await _dbContext.Orders.AddAsync(order);
    }

    public void Update(Order order)
    {
        _dbContext.Orders.Update(order);
    }

    public async Task DeleteAsync(Order order)
    {
        _dbContext.Orders.Remove(order);
        await Task.CompletedTask;
    }

    public async Task<Order?> GetByIdAsync(Guid orderId)
    {
        return await _dbContext
            .Orders.Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<Order?> GetByIdAsync(Guid orderId, Guid koperId)
    {
        return await _dbContext
            .Orders.Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.KoperId == koperId);
    }

    public async Task<IEnumerable<Order>> GetAllByIdsAsync(List<Guid> orderIds)
    {
        return await _dbContext
            .Orders.Where(o => orderIds.Contains(o.Id))
            .Include(o => o.OrderItems)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllByKoperIdAsync(Guid koperId)
    {
        return await _dbContext
            .Orders.Where(o => o.KoperId == koperId)
            .Include(o => o.OrderItems)
            .ToListAsync();
    }

    #region Complex Queries

    public async Task<(Order order, VeilingKlokStatus klokStatus)?> GetWithKlokStatusByIdAsync(
        Guid id,
        Guid koperId
    )
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(o =>
            o.Id == id && o.KoperId == koperId
        );

        if (order == null)
            return null;

        var klokStatus = await _dbContext
            .Veilingklokken.Where(vk => vk.Id == order.VeilingKlokId)
            .Select(vk => vk.Status)
            .FirstOrDefaultAsync();

        return (order, klokStatus);
    }

    public async Task<(Order order, List<OrderProductInfo> products)?> GetWithProductsByIdAsync(
        Guid id
    )
    {
        var order = await _dbContext
            .Orders.Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return null;

        var products = await _dbContext
            .OrderItems.Where(oi => oi.OrderId == id)
            .Join(
                _dbContext.Products,
                oi => oi.ProductId,
                p => p.Id,
                (oi, p) => new { OrderItem = oi, Product = p }
            )
            .Join(
                _dbContext.Kwekers,
                x => x.Product.KwekerId,
                k => k.Id,
                (x, k) =>
                    new OrderProductInfo(
                        x.Product.Id,
                        x.Product.Name,
                        x.Product.Description,
                        x.Product.ImageUrl,
                        x.OrderItem.PriceAtPurchase,
                        k.CompanyName,
                        x.OrderItem.Quantity
                    )
            )
            .ToListAsync();

        return (order, products);
    }

    public async Task<(Order order, List<OrderProductInfo> products)?> GetWithProductsByIdAsync(
        Guid id,
        Guid koperId
    )
    {
        var order = await _dbContext
            .Orders.Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id && o.KoperId == koperId);

        if (order == null)
            return null;

        var products = await _dbContext
            .OrderItems.Where(oi => oi.OrderId == id)
            .Join(
                _dbContext.Products,
                oi => oi.ProductId,
                p => p.Id,
                (oi, p) => new { OrderItem = oi, Product = p }
            )
            .Join(
                _dbContext.Kwekers,
                x => x.Product.KwekerId,
                k => k.Id,
                (x, k) =>
                    new OrderProductInfo(
                        x.Product.Id,
                        x.Product.Name,
                        x.Product.Description,
                        x.Product.ImageUrl,
                        x.OrderItem.PriceAtPurchase,
                        k.CompanyName,
                        x.OrderItem.Quantity
                    )
            )
            .ToListAsync();

        return (order, products);
    }

    public async Task<(IEnumerable<Order> Items, int TotalCount)> GetAllWithFilterAsync(
        OrderStatus? statusFilter,
        DateTime? beforeDate,
        DateTime? afterDate,
        Guid? productId,
        Guid? koperId,
        Guid? klokId,
        int pageNumber,
        int pageSize
    )
    {
        var query = _dbContext.Orders.Where(o => o.KoperId == koperId);

        if (statusFilter.HasValue)
            query = query.Where(o => o.Status == statusFilter.Value);

        if (beforeDate.HasValue)
            query = query.Where(o => o.CreatedAt <= beforeDate.Value);

        if (afterDate.HasValue)
            query = query.Where(o => o.CreatedAt >= afterDate.Value);

        if (klokId.HasValue)
            query = query.Where(o => o.VeilingKlokId == klokId.Value);

        if (productId.HasValue)
            query = query.Where(o => o.OrderItems.Any(oi => oi.ProductId == productId.Value));

        var totalCount = await query.CountAsync();
        var items = await query
            .Include(o => o.OrderItems)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    #endregion
}
