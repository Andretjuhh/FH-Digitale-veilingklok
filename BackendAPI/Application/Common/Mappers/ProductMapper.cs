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
                Dimension = entity.Dimension,
                Auctioned = entity.Auctioned,
                AuctionedAt = entity.AuctionedAt,
                AuctionedCount = entity.AuctionedCount,
                CompanyName = kweker.CompanyName,
                KwekerId = kweker.Id,
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
            Dimension = entity.Dimension,
            Auctioned = entity.Auctioned,
            AuctionedAt = entity.AuctionedAt,
            AuctionedCount = entity.AuctionedCount,
            CompanyName = kweker.CompanyName,
            KwekerId = kweker.Id,
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
                    AuctionedAt = entity.AuctionedAt,
                    Stock = entity.Stock,
                    ImageUrl = entity.ImageUrl,
                    Dimension = entity.Dimension,
                    CompanyName = kweker.CompanyName,
                    KwekerId = kweker.Id,
                };

        public static ProductOutputDto ToOutputDto(Product entity, KwekerInfo kweker)
        {
            return new ProductOutputDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                AuctionedPrice = entity.AuctionPrice,
                AuctionedAt = entity.AuctionedAt,
                Stock = entity.Stock,
                ImageUrl = entity.ImageUrl,
                Dimension = entity.Dimension,
                CompanyName = kweker.CompanyName,
                KwekerId = kweker.Id,
            };
        }
    }
}
