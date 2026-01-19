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

    public async Task<(
        Order Order,
        List<OrderProductInfo> Products,
        KoperInfo Koper
    )?> GetOrderDetailsAsync(Guid orderId)
    {
        var query =
            from order in _dbContext.Orders.AsNoTracking()
            from orderItem in order.OrderItems
            join product in _dbContext.Products.AsNoTracking()
                on orderItem.ProductId equals product.Id
            join kweker in _dbContext.Kwekers.AsNoTracking() on product.KwekerId equals kweker.Id
            join koper in _dbContext.Kopers.AsNoTracking() on order.KoperId equals koper.Id
            where order.Id == orderId
            select new
            {
                Order = order,
                OrderItem = orderItem,
                Product = product,
                Kweker = kweker,
                Koper = koper,
                KoperAddress = koper.Adresses.FirstOrDefault(),
            };

        var results = await query
            .Select(x => new
            {
                x.Order,
                ProductInfo = new OrderProductInfo(
                    x.Product.Id,
                    x.Product.Name,
                    x.Product.Description,
                    x.Product.ImageUrl,
                    x.OrderItem.PriceAtPurchase,
                    x.OrderItem.ProductMinimumPrice,
                    x.Kweker.CompanyName,
                    x.Kweker.Id,
                    x.OrderItem.Quantity
                ),
                Koper = new KoperInfo(
                    x.Koper.Id,
                    x.Koper.Email ?? "",
                    x.Koper.FirstName,
                    x.Koper.LastName,
                    x.Koper.Telephone,
                    x.KoperAddress ?? new Address("", "", "", "", "")
                ),
            })
            .ToListAsync();

        if (!results.Any())
            return null;

        var first = results.First();
        var products = results.Select(x => x.ProductInfo).ToList();

        return (first.Order, products, first.Koper);
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

    public async Task<Order?> FindOrderAsync(Guid koperId, Guid veilingKlokId, Guid? productId)
    {
        return await _dbContext
            .Orders.Include(o => o.OrderItems)
            .Where(o =>
                o.KoperId == koperId
                && o.VeilingKlokId == veilingKlokId
                && (!productId.HasValue || o.OrderItems.Any(oi => oi.ProductId == productId))
            )
            .FirstOrDefaultAsync();
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
        var order = await _dbContext.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return null;

        var products = await _dbContext
            .OrderItems.AsNoTracking()
            .Where(oi => oi.OrderId == id)
            .Join(
                _dbContext.Products.AsNoTracking(),
                oi => oi.ProductId,
                p => p.Id,
                (oi, p) => new { OrderItem = oi, Product = p }
            )
            .Join(
                _dbContext.Kwekers.AsNoTracking(),
                x => x.Product.KwekerId,
                k => k.Id,
                (x, k) =>
                    new OrderProductInfo(
                        x.Product.Id,
                        x.Product.Name,
                        x.Product.Description,
                        x.Product.ImageUrl,
                        x.OrderItem.PriceAtPurchase,
                        x.OrderItem.ProductMinimumPrice,
                        k.CompanyName,
                        k.Id,
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
            .Orders.AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id && o.KoperId == koperId);

        if (order == null)
            return null;

        var products = await _dbContext
            .OrderItems.AsNoTracking()
            .Where(oi => oi.OrderId == id)
            .Join(
                _dbContext.Products.AsNoTracking(),
                oi => oi.ProductId,
                p => p.Id,
                (oi, p) => new { OrderItem = oi, Product = p }
            )
            .Join(
                _dbContext.Kwekers.AsNoTracking(),
                x => x.Product.KwekerId,
                k => k.Id,
                (x, k) =>
                    new OrderProductInfo(
                        x.Product.Id,
                        x.Product.Name,
                        x.Product.Description,
                        x.Product.ImageUrl,
                        x.OrderItem.PriceAtPurchase,
                        x.OrderItem.ProductMinimumPrice,
                        k.CompanyName,
                        k.Id,
                        x.OrderItem.Quantity
                    )
            )
            .ToListAsync();

        return (order, products);
    }

    public async Task<(
        IEnumerable<(Order order, List<OrderProductInfo> products)> Items,
        int TotalCount
    )> GetAllWithFilterAsync(
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
        var query = _dbContext.Orders.AsQueryable();

        if (koperId.HasValue)
            query = query.Where(o => o.KoperId == koperId.Value);

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
        var orders = await query
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var orderIds = orders.Select(o => o.Id).ToList();

        var productsFlat = await _dbContext
            .OrderItems.AsNoTracking()
            .Where(oi => orderIds.Contains(oi.OrderId))
            .Join(
                _dbContext.Products.AsNoTracking(),
                oi => oi.ProductId,
                p => p.Id,
                (oi, p) => new { OrderItem = oi, Product = p }
            )
            .Join(
                _dbContext.Kwekers.AsNoTracking(),
                x => x.Product.KwekerId,
                k => k.Id,
                (x, k) =>
                    new
                    {
                        x.OrderItem.OrderId,
                        ProductInfo = new OrderProductInfo(
                            x.Product.Id,
                            x.Product.Name,
                            x.Product.Description,
                            x.Product.ImageUrl,
                            x.OrderItem.PriceAtPurchase,
                            x.OrderItem.ProductMinimumPrice,
                            k.CompanyName,
                            k.Id,
                            x.OrderItem.Quantity
                        ),
                    }
            )
            .ToListAsync();

        var items = orders
            .Select(o =>
                (o, productsFlat.Where(p => p.OrderId == o.Id).Select(p => p.ProductInfo).ToList())
            )
            .ToList();

        return (items, totalCount);
    }

    public async Task<(
        Order Order,
        List<OrderProductInfo> Products,
        KoperInfo Koper
    )?> GetKwekerOrderAsync(Guid orderId, Guid kwekerId)
    {
        var query =
            from order in _dbContext.Orders
            from orderItem in order.OrderItems
            join product in _dbContext.Products on orderItem.ProductId equals product.Id
            join kweker in _dbContext.Kwekers on product.KwekerId equals kweker.Id
            join koper in _dbContext.Kopers on order.KoperId equals koper.Id
            where order.Id == orderId && kweker.Id == kwekerId
            select new
            {
                Order = order,
                OrderItem = orderItem,
                Product = product,
                Kweker = kweker,
                Koper = koper,
                KoperAddress = koper.Adresses.FirstOrDefault(),
            };

        var results = await query
            .Select(x => new
            {
                x.Order,
                ProductInfo = new OrderProductInfo(
                    x.Product.Id,
                    x.Product.Name,
                    x.Product.Description,
                    x.Product.ImageUrl,
                    x.OrderItem.PriceAtPurchase,
                    x.OrderItem.ProductMinimumPrice,
                    x.Kweker.CompanyName,
                    x.Kweker.Id,
                    x.OrderItem.Quantity
                ),
                Koper = new KoperInfo(
                    x.Koper.Id,
                    x.Koper.Email ?? "",
                    x.Koper.FirstName,
                    x.Koper.LastName,
                    x.Koper.Telephone,
                    x.KoperAddress ?? new Address("", "", "", "", "")
                ),
            })
            .ToListAsync();

        if (!results.Any())
            return null;

        var first = results.First();
        var products = results.Select(x => x.ProductInfo).ToList();

        return (first.Order, products, first.Koper);
    }

    public async Task<(
        Order Order,
        List<OrderProductInfo> Products,
        KwekerInfo Kweker,
        KoperInfo Koper
    )?> GetKoperOrderAsync(Guid orderId, Guid koperId)
    {
        var query =
            from order in _dbContext.Orders
            from orderItem in order.OrderItems
            join product in _dbContext.Products on orderItem.ProductId equals product.Id
            join kweker in _dbContext.Kwekers on product.KwekerId equals kweker.Id
            join koper in _dbContext.Kopers on order.KoperId equals koper.Id
            where order.Id == orderId && order.KoperId == koperId
            select new
            {
                Order = order,
                OrderItem = orderItem,
                Product = product,
                Kweker = kweker,
                Koper = koper,
                KoperAddress = koper.Adresses.FirstOrDefault(),
            };

        var results = await query
            .Select(x => new
            {
                x.Order,
                ProductInfo = new OrderProductInfo(
                    x.Product.Id,
                    x.Product.Name,
                    x.Product.Description,
                    x.Product.ImageUrl,
                    x.OrderItem.PriceAtPurchase,
                    x.OrderItem.ProductMinimumPrice,
                    x.Kweker.CompanyName,
                    x.Kweker.Id,
                    x.OrderItem.Quantity
                ),
                Kweker = new KwekerInfo(
                    x.Kweker.Id,
                    x.Kweker.CompanyName,
                    x.Kweker.PhoneNumber ?? "",
                    x.Kweker.Email ?? ""
                ),
                Koper = new KoperInfo(
                    x.Koper.Id,
                    x.Koper.Email ?? "",
                    x.Koper.FirstName,
                    x.Koper.LastName,
                    x.Koper.Telephone,
                    x.KoperAddress ?? new Address("", "", "", "", "")
                ),
            })
            .ToListAsync();

        if (!results.Any())
            return null;

        var first = results.First();
        var products = results.Select(x => x.ProductInfo).ToList();

        return (first.Order, products, first.Kweker, first.Koper);
    }

    public async Task<(
        IEnumerable<(Order Order, List<OrderProductInfo> Products, KoperInfo Koper)> Items,
        int TotalCount
    )> GetAllKwekerWithFilterAsync(
        string? ProductNameFilter,
        string? KoperNameFilter,
        OrderStatus? statusFilter,
        DateTime? beforeDate,
        DateTime? afterDate,
        Guid? productId,
        Guid kwekerId,
        int pageNumber,
        int pageSize
    )
    {
        // 1. Base Query of relevant items
        var query =
            from o in _dbContext.Orders.AsNoTracking()
            from oi in o.OrderItems
            join p in _dbContext.Products.AsNoTracking() on oi.ProductId equals p.Id
            join kw in _dbContext.Kwekers.AsNoTracking() on p.KwekerId equals kw.Id
            join k in _dbContext.Kopers.AsNoTracking() on o.KoperId equals k.Id
            where kw.Id == kwekerId
            select new
            {
                Order = o,
                Product = p,
                Koper = k,
                Kweker = kw,
            };

        // 2. Filters
        if (statusFilter.HasValue)
            query = query.Where(x => x.Order.Status == statusFilter.Value);
        if (!string.IsNullOrEmpty(ProductNameFilter))
            query = query.Where(x => x.Product.Name.Contains(ProductNameFilter));
        if (!string.IsNullOrEmpty(KoperNameFilter))
            query = query.Where(x =>
                (x.Koper.FirstName + " " + x.Koper.LastName).Contains(KoperNameFilter)
            );
        if (beforeDate.HasValue)
            query = query.Where(x => x.Order.CreatedAt <= beforeDate.Value);
        if (afterDate.HasValue)
            query = query.Where(x => x.Order.CreatedAt >= afterDate.Value);
        if (productId.HasValue)
            query = query.Where(x => x.Product.Id == productId.Value);

        // 3. Get Paged Order IDs
        var orderQuery = query.Select(x => x.Order).Distinct();

        var totalCount = await orderQuery.CountAsync();

        var pagedOrdersIds = await orderQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.Id)
            .ToListAsync();

        if (!pagedOrdersIds.Any())
            return (new List<(Order, List<OrderProductInfo>, KoperInfo)>(), totalCount);

        // 4. Fetch Data for these orders
        var resultQuery =
            from o in _dbContext.Orders.AsNoTracking()
            from oi in o.OrderItems
            join p in _dbContext.Products.AsNoTracking() on oi.ProductId equals p.Id
            join kw in _dbContext.Kwekers.AsNoTracking() on p.KwekerId equals kw.Id
            join k in _dbContext.Kopers.AsNoTracking() on o.KoperId equals k.Id
            where pagedOrdersIds.Contains(o.Id) && kw.Id == kwekerId
            select new
            {
                Order = o,
                Product = p,
                OrderItem = oi,
                Kweker = kw,
                Koper = k,
                KoperAddress = k.Adresses.FirstOrDefault(),
            };

        var flatResults = await resultQuery
            .Select(x => new
            {
                x.Order,
                ProductInfo = new OrderProductInfo(
                    x.Product.Id,
                    x.Product.Name,
                    x.Product.Description,
                    x.Product.ImageUrl,
                    x.OrderItem.PriceAtPurchase,
                    x.OrderItem.ProductMinimumPrice,
                    x.Kweker.CompanyName,
                    x.Kweker.Id,
                    x.OrderItem.Quantity
                ),
                Koper = new KoperInfo(
                    x.Koper.Id,
                    x.Koper.Email ?? "",
                    x.Koper.FirstName,
                    x.Koper.LastName,
                    x.Koper.Telephone,
                    x.KoperAddress ?? new Address("", "", "", "", "")
                ),
            })
            .ToListAsync();

        var groupedResults = flatResults
            .GroupBy(x => x.Order.Id)
            .Select(g =>
                (
                    g.First().Order,
                    g.Select(x => x.ProductInfo).ToList(),
                    g.First().Koper // Koper second
                )
            );

        return (groupedResults, totalCount);
    }

    #endregion
}
