using System.Linq.Expressions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTOs.Output;
using Domain.Entities;

namespace Application.Common.Mappers;

public class ProductMapper : IBaseMapper<Product, KwekerInfo, ProductDetailsOutputDto>
{
    public static Expression<Func<Product, KwekerInfo, ProductDetailsOutputDto>> EntityDto =>
        (entity, kweker) =>
            new ProductDetailsOutputDto
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                Name = entity.Name,
                Description = entity.Description,
                AuctionPrice = entity.AuctionPrice,
                MinimumPrice = entity.MinimumPrice,
                Stock = entity.Stock,
                ImageBase64 = entity.ImageUrl,
                Dimension = entity.Dimension ?? "",
                Region = entity.Region,
                Auctioned = entity.Auctioned,
                AuctionedAt = entity.AuctionedAt,
                AuctionedCount = entity.AuctionedCount,
                CompanyName = kweker.CompanyName,
                KwekerId = kweker.Id
            };

    public static ProductDetailsOutputDto ToOutputDto(Product entity, KwekerInfo kweker)
    {
        return new ProductDetailsOutputDto
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            Name = entity.Name,
            Description = entity.Description,
            AuctionPrice = entity.AuctionPrice,
            MinimumPrice = entity.MinimumPrice,
            Stock = entity.Stock,
            ImageBase64 = entity.ImageUrl,
            Dimension = entity.Dimension ?? "",
            Region = entity.Region,
            Auctioned = entity.Auctioned,
            AuctionedAt = entity.AuctionedAt,
            AuctionedCount = entity.AuctionedCount,
            CompanyName = kweker.CompanyName,
            KwekerId = kweker.Id
        };
    }

    public class Minimal : IBaseMapper<Product, KwekerInfo, ProductOutputDto>
    {
        public static Expression<Func<Product, KwekerInfo, ProductOutputDto>> EntityDto =>
            (entity, kweker) =>
                new ProductOutputDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    AuctionedPrice = entity.AuctionPrice,
                    MinimumPrice = entity.MinimumPrice, // Always null for Koper
                    AuctionedAt = entity.AuctionedAt,
                    Stock = entity.Stock,
                    ImageUrl = entity.ImageUrl,
                    Dimension = entity.Dimension ?? "",
                    Region = entity.Region,
                    CompanyName = kweker.CompanyName,
                    AuctionPlanned = entity.VeilingKlokId.HasValue
                };

        public static ProductOutputDto ToOutputDto(Product entity, KwekerInfo kweker)
        {
            return new ProductOutputDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                AuctionedPrice = entity.AuctionPrice,
                MinimumPrice = entity.MinimumPrice, // Always null for Koper
                AuctionedAt = entity.AuctionedAt,
                Stock = entity.Stock,
                ImageUrl = entity.ImageUrl,
                Dimension = entity.Dimension ?? "",
                Region = entity.Region,
                CompanyName = kweker.CompanyName,
                AuctionPlanned = entity.VeilingKlokId.HasValue
            };
        }
    }

    public class FromOrderInfo : IBaseMapper<OrderProductInfo, ProductOutputDto>
    {
        public static Expression<Func<OrderProductInfo, ProductOutputDto>> EntityDto =>
            entity => new ProductOutputDto
            {
                Id = entity.ProductId,
                Name = entity.ProductName,
                Description = entity.ProductDescription,
                AuctionedPrice = entity.PriceAtPurchase,
                MinimumPrice = entity.ProductMinimumPrice,
                ImageUrl = entity.ProductImageUrl,
                CompanyName = entity.CompanyName,
                AuctionPlanned = true,
                Region = null, // Not available in OrderProductInfo
                AuctionedAt = null, // Not available in OrderProductInfo
                Stock = 0, // Not available in OrderProductInfo
                Dimension = "" // Not available in OrderProductInfo
            };

        public static ProductOutputDto ToOutputDto(OrderProductInfo entity)
        {
            return new ProductOutputDto
            {
                Id = entity.ProductId,
                Name = entity.ProductName,
                Description = entity.ProductDescription,
                AuctionedPrice = entity.PriceAtPurchase,
                MinimumPrice = entity.ProductMinimumPrice,
                ImageUrl = entity.ProductImageUrl,
                CompanyName = entity.CompanyName,
                AuctionPlanned = true,
                Region = null, // Not available in OrderProductInfo
                AuctionedAt = null, // Not available in OrderProductInfo
                Stock = 0, // Not available in OrderProductInfo
                Dimension = "" // Not available in OrderProductInfo
            };
        }
    }
}