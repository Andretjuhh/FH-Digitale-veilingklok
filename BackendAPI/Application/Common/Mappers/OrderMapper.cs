using System.Linq.Expressions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTOs.Output;
using Domain.Entities;

namespace Application.Common.Mappers;

public class OrderMapper : IBaseMapper<Order, List<OrderProductInfo>, OrderOutputDto>
{
    public static Expression<Func<Order, List<OrderProductInfo>, OrderOutputDto>> EntityDto =>
        (entity, products) =>
            new OrderOutputDto
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                Status = entity.Status,
                ClosedAt = entity.ClosedAt,
                TotalAmount = products.Sum(oi => oi.Quantity * oi.PriceAtPurchase),
                TotalItems = products.Sum(oi => oi.Quantity),
                ProductId = products.Select(oi => oi.ProductId).FirstOrDefault(),
                ProductName =
                    products.Select(oi => oi.ProductName).FirstOrDefault() ?? string.Empty,
                ProductDescription =
                    products.Select(oi => oi.ProductDescription).FirstOrDefault() ?? string.Empty,
                ProductImageUrl =
                    products.Select(oi => oi.ProductImageUrl).FirstOrDefault() ?? string.Empty,
                CompanyName =
                    products.Select(oi => oi.CompanyName).FirstOrDefault() ?? string.Empty,
            };

    public static OrderOutputDto ToOutputDto(Order entity, List<OrderProductInfo> products)
    {
        var firstItem = products.FirstOrDefault();

        return new OrderOutputDto
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            Status = entity.Status,
            ClosedAt = entity.ClosedAt,
            TotalAmount = products.Sum(oi => oi.Quantity * oi.PriceAtPurchase),
            TotalItems = products.Sum(oi => oi.Quantity),
            ProductId = firstItem?.ProductId ?? Guid.Empty,
            ProductName = firstItem?.ProductName ?? string.Empty,
            ProductDescription = firstItem?.ProductDescription ?? string.Empty,
            ProductImageUrl = firstItem?.ProductImageUrl ?? string.Empty,
            CompanyName = firstItem?.CompanyName ?? string.Empty,
        };
    }

    public class ItemOrder : IBaseMapper<OrderItem, OrderItemProduct, OrderProductOutputDto>
    {
        public static Expression<
            Func<OrderItem, OrderItemProduct, OrderProductOutputDto>
        > EntityDto =>
            (entity, product) =>
                new OrderProductOutputDto
                {
                    ProductId = entity.ProductId,
                    ProductName = product.ProductName,
                    ProductImageUrl = product.ProductImageUrl,
                    ProductDescription = product.ProductDescription,
                    Quantity = entity.Quantity,
                    PriceAtPurchase = entity.PriceAtPurchase,
                    OrderedAt = entity.CreatedAt,
                    CompanyName = product.CompanyName,
                    MinimalPrice = null,
                };

        public static OrderProductOutputDto ToOutputDto(OrderItem entity, OrderItemProduct product)
        {
            return new OrderProductOutputDto
            {
                ProductId = entity.ProductId,
                ProductName = product.ProductName,
                ProductImageUrl = product.ProductImageUrl,
                ProductDescription = product.ProductDescription,
                Quantity = entity.Quantity,
                PriceAtPurchase = entity.PriceAtPurchase,
                OrderedAt = entity.CreatedAt,
                CompanyName = product.CompanyName,
                MinimalPrice = null,
            };
        }
    }

    public class KwekerOrders
        : IBaseMapper<
            (Order Order, List<OrderProductInfo> Products, KoperInfo Koper),
            OrderKwekerOutputDto
        >
    {
        public static Expression<
            Func<
                (Order Order, List<OrderProductInfo> Products, KoperInfo Koper),
                OrderKwekerOutputDto
            >
        > EntityDto => data => ToOutputDto(data);

        public static OrderKwekerOutputDto ToOutputDto(
            (Order Order, List<OrderProductInfo> Products, KoperInfo Koper) data
        )
        {
            return new OrderKwekerOutputDto
            {
                Id = data.Order.Id,
                CreatedAt = data.Order.CreatedAt,
                Status = data.Order.Status,
                ClosedAt = data.Order.ClosedAt,
                Quantity = data.Products.Sum(l => l.Quantity),
                TotalPrice = data.Products.Sum(l => l.PriceAtPurchase * l.Quantity),
                Products = data
                    .Products.Select(l => new OrderProductOutputDto
                    {
                        ProductId = l.ProductId,
                        ProductName = l.ProductName,
                        ProductDescription = l.ProductDescription,
                        ProductImageUrl = l.ProductImageUrl,
                        CompanyName = l.CompanyName,
                        MinimalPrice = l.ProductMinimumPrice,
                        Quantity = l.Quantity,
                        PriceAtPurchase = l.PriceAtPurchase,
                        OrderedAt = data.Order.CreatedAt,
                    })
                    .ToList(),
                KoperInfo = KoperMapper.Info.ToOutputDto(data.Koper),
            };
        }
    }

    public class KoperOrders
        : IBaseMapper<
            (Order Order, List<OrderProductInfo> Products, KwekerInfo Kweker, KoperInfo Koper),
            OrderKoperOutputDto
        >
    {
        public static Expression<
            Func<
                (Order Order, List<OrderProductInfo> Products, KwekerInfo Kweker, KoperInfo Koper),
                OrderKoperOutputDto
            >
        > EntityDto =>
            data => new OrderKoperOutputDto
            {
                Id = data.Order.Id,
                CreatedAt = data.Order.CreatedAt,
                Status = data.Order.Status,
                ClosedAt = data.Order.ClosedAt,
                Quantity = data.Products.Sum(l => l.Quantity),
                TotalPrice = data.Products.Sum(l => l.Quantity * l.PriceAtPurchase),
                Products = data
                    .Products.Select(l => new OrderProductOutputDto
                    {
                        ProductId = l.ProductId,
                        ProductName = l.ProductName,
                        ProductDescription = l.ProductDescription,
                        ProductImageUrl = l.ProductImageUrl,
                        CompanyName = l.CompanyName,
                        MinimalPrice = null, // Hidden for Koper
                        Quantity = l.Quantity,
                        PriceAtPurchase = l.PriceAtPurchase,
                        OrderedAt = data.Order.CreatedAt,
                    })
                    .ToList(),
                KwekerInfo = data.Kweker,
                KoperInfo = data.Koper,
            };

        public static OrderKoperOutputDto ToOutputDto(
            (Order Order, List<OrderProductInfo> Products, KwekerInfo Kweker, KoperInfo Koper) data
        )
        {
            return new OrderKoperOutputDto
            {
                Id = data.Order.Id,
                CreatedAt = data.Order.CreatedAt,
                Status = data.Order.Status,
                ClosedAt = data.Order.ClosedAt,
                Quantity = data.Products.Sum(l => l.Quantity),
                TotalPrice = data.Products.Sum(l => l.Quantity * l.PriceAtPurchase),
                Products = data
                    .Products.Select(l => new OrderProductOutputDto
                    {
                        ProductId = l.ProductId,
                        ProductName = l.ProductName,
                        ProductDescription = l.ProductDescription,
                        ProductImageUrl = l.ProductImageUrl,
                        CompanyName = l.CompanyName,
                        MinimalPrice = null, // Hidden for Koper
                        Quantity = l.Quantity,
                        PriceAtPurchase = l.PriceAtPurchase,
                        OrderedAt = data.Order.CreatedAt,
                    })
                    .ToList(),
                KwekerInfo = data.Kweker,
                KoperInfo = data.Koper,
            };
        }
    }
}
