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

    public class OrderKwekerDetails : IBaseMapper<Order, (ProductOutputDto Product, KoperInfoOutputDto KoperInfo),
        OrderKwekerOutput>
    {
        public static
            Expression<Func<Order, (ProductOutputDto Product, KoperInfoOutputDto KoperInfo), OrderKwekerOutput>>
            EntityDto =>
            (entity, data) =>
                new OrderKwekerOutput
                {
                    Id = entity.Id,
                    CreatedAt = entity.CreatedAt,
                    Status = entity.Status,
                    ClosedAt = entity.ClosedAt,
                    Quantity = entity.OrderItems.Sum(oi => oi.Quantity),
                    TotalPrice = entity.OrderItems.Sum(oi => oi.Quantity * oi.PriceAtPurchase),
                    Product = data.Product,
                    KoperInfo = data.KoperInfo
                };

        public static OrderKwekerOutput ToOutputDto(Order entity,
            (ProductOutputDto Product, KoperInfoOutputDto KoperInfo) data)
        {
            return new OrderKwekerOutput
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                Status = entity.Status,
                ClosedAt = entity.ClosedAt,
                Quantity = entity.OrderItems.Sum(oi => oi.Quantity),
                TotalPrice = entity.OrderItems.Sum(oi => oi.Quantity * oi.PriceAtPurchase),
                Product = data.Product,
                KoperInfo = data.KoperInfo
            };
        }
    }

    public class KwekerOrders : IBaseMapper<(Order Order, OrderProductInfo OProductInfo, KoperInfo Koper),
        OrderKwekerOutput>
    {
        public static Expression<Func<(Order Order, OrderProductInfo OProductInfo, KoperInfo Koper), OrderKwekerOutput>>
            EntityDto =>
            data => new OrderKwekerOutput
            {
                Id = data.Order.Id,
                CreatedAt = data.Order.CreatedAt,
                Status = data.Order.Status,
                ClosedAt = data.Order.ClosedAt,
                Quantity = data.OProductInfo.Quantity,
                TotalPrice = data.OProductInfo.PriceAtPurchase * data.OProductInfo.Quantity,
                Product = ProductMapper.FromOrderInfo.EntityDto.Compile()(data.OProductInfo),
                KoperInfo = KoperMapper.Info.EntityDto.Compile()(data.Koper)
            };

        public static OrderKwekerOutput ToOutputDto((Order Order, OrderProductInfo OProductInfo, KoperInfo Koper) data)
        {
            return new OrderKwekerOutput
            {
                Id = data.Order.Id,
                CreatedAt = data.Order.CreatedAt,
                Status = data.Order.Status,
                ClosedAt = data.Order.ClosedAt,
                Quantity = data.OProductInfo.Quantity,
                TotalPrice = data.OProductInfo.PriceAtPurchase * data.OProductInfo.Quantity,
                Product = ProductMapper.FromOrderInfo.ToOutputDto(data.OProductInfo),
                KoperInfo = KoperMapper.Info.ToOutputDto(data.Koper)
            };
        }
    }
}