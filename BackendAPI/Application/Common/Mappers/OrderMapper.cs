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
            TotalItems = entity.OrderItems.Sum(oi => oi.Quantity),
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
            TotalItems = entity.OrderItems.Sum(oi => oi.Quantity),
        };
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
            };
        }
    }
}
