using System.Linq.Expressions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTOs.Output;
using Domain.Entities;

namespace Application.Common.Mappers;

public class OrderMapper : IBaseMapper<Order, OrderOutputDto>
{
    public static Expression<Func<Order, OrderOutputDto>> EntityDto =>
        entity => new OrderOutputDto
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            Status = entity.Status,
            ClosedAt = entity.ClosedAt,
            TotalAmount = entity.OrderItems.Sum(oi => oi.Quantity * oi.PriceAtPurchase),
            TotalItems = entity.OrderItems.Sum(oi => oi.Quantity)
        };

    public static OrderOutputDto ToOutputDto(Order entity)
    {
        return new OrderOutputDto
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            Status = entity.Status,
            ClosedAt = entity.ClosedAt,
            TotalAmount = entity.OrderItems.Sum(oi => oi.Quantity * oi.PriceAtPurchase),
            TotalItems = entity.OrderItems.Sum(oi => oi.Quantity)
        };
    }

    public class ExtraDetails : IBaseMapper<Order, List<OrderProductInfo>, OrderDetailsOutputDto>
    {
        public static Expression<Func<Order, List<OrderProductInfo>, OrderDetailsOutputDto>> EntityDto =>
            (entity, products) =>
                new OrderDetailsOutputDto
                {
                    Id = entity.Id,
                    CreatedAt = entity.CreatedAt,
                    Status = entity.Status,
                    ClosedAt = entity.ClosedAt,
                    TotalAmount = entity.OrderItems.Sum(oi => oi.Quantity * oi.PriceAtPurchase),
                    TotalItems = entity.OrderItems.Sum(oi => oi.Quantity),
                    Products = products
                        .Select(p => new OrderItemOutputDto
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            ProductDescription = p.ProductDescription,
                            ProductImageUrl = p.ProductImageUrl,
                            CompanyName = p.CompanyName,
                            Quantity = p.Quantity,
                            PriceAtPurchase = p.PriceAtPurchase,
                            OrderedAt = entity.CreatedAt
                        })
                        .ToList()
                };

        public static OrderDetailsOutputDto ToOutputDto(Order entity, List<OrderProductInfo> products)
        {
            return new OrderDetailsOutputDto
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                Status = entity.Status,
                ClosedAt = entity.ClosedAt,
                TotalAmount = entity.OrderItems.Sum(oi => oi.Quantity * oi.PriceAtPurchase),
                TotalItems = entity.OrderItems.Sum(oi => oi.Quantity),
                Products = products
                    .Select(p => new OrderItemOutputDto
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        ProductDescription = p.ProductDescription,
                        ProductImageUrl = p.ProductImageUrl,
                        CompanyName = p.CompanyName,
                        Quantity = p.Quantity,
                        PriceAtPurchase = p.PriceAtPurchase,
                        OrderedAt = entity.CreatedAt
                    })
                    .ToList()
            };
        }
    }

    public class ItemOrder : IBaseMapper<OrderItem, OrderItemProduct, OrderItemOutputDto>
    {
        public static Expression<Func<OrderItem, OrderItemProduct, OrderItemOutputDto>> EntityDto =>
            (entity, product) =>
                new OrderItemOutputDto
                {
                    ProductId = entity.ProductId,
                    ProductName = product.ProductName,
                    ProductImageUrl = product.ProductImageUrl,
                    ProductDescription = product.ProductDescription,
                    Quantity = entity.Quantity,
                    PriceAtPurchase = entity.PriceAtPurchase,
                    OrderedAt = entity.CreatedAt,
                    CompanyName = product.CompanyName
                };

        public static OrderItemOutputDto ToOutputDto(OrderItem entity, OrderItemProduct product)
        {
            return new OrderItemOutputDto
            {
                ProductId = entity.ProductId,
                ProductName = product.ProductName,
                ProductImageUrl = product.ProductImageUrl,
                ProductDescription = product.ProductDescription,
                Quantity = entity.Quantity,
                PriceAtPurchase = entity.PriceAtPurchase,
                OrderedAt = entity.CreatedAt,
                CompanyName = product.CompanyName
            };
        }
    }
}