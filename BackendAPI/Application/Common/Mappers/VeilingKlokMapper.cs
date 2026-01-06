using System.Linq.Expressions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTOs.Output;
using Domain.Entities;

namespace Application.Common.Mappers;

public class
    VeilingKlokMapper : IBaseMapper<VeilingKlok, VeilingKlokExtraInfo<ProductDetailsOutputDto>,
    VeilingKlokDetailsOutputDto>
{
    public static Expression<
            Func<VeilingKlok, VeilingKlokExtraInfo<ProductDetailsOutputDto>, VeilingKlokDetailsOutputDto>>
        EntityDto =>
        (veiling, info) =>
            new VeilingKlokDetailsOutputDto
            {
                Id = veiling.Id,
                Status = veiling.Status,
                PeakedLiveViews = veiling.PeakedLiveViews,
                ScheduledAt = veiling.ScheduledAt,
                StartedAt = veiling.StartedAt,
                EndedAt = veiling.EndedAt,
                CreatedAt = veiling.CreatedAt,
                RegionOrState = veiling.RegionOrState,
                Country = veiling.Country,
                TotalProducts = veiling.ProductsIds.Count,
                CurrentBids = info.TotalBids,
                Products = info.Products,
                HighestBidAmount = veiling.HighestPrice,
                LowestBidAmount = veiling.LowestPrice
            };

    public static VeilingKlokDetailsOutputDto ToOutputDto(VeilingKlok entity,
        VeilingKlokExtraInfo<ProductDetailsOutputDto> info)
    {
        return new VeilingKlokDetailsOutputDto
        {
            Id = entity.Id,
            Status = entity.Status,
            PeakedLiveViews = entity.PeakedLiveViews,
            ScheduledAt = entity.ScheduledAt,
            StartedAt = entity.StartedAt,
            EndedAt = entity.EndedAt,
            CreatedAt = entity.CreatedAt,
            RegionOrState = entity.RegionOrState,
            Country = entity.Country,
            TotalProducts = entity.ProductsIds.Count,
            CurrentBids = info.TotalBids,
            Products = info.Products,
            HighestBidAmount = entity.HighestPrice,
            LowestBidAmount = entity.LowestPrice
        };
    }


    public class Minimal : IBaseMapper<VeilingKlok, VeilingKlokExtraInfo<ProductOutputDto>, VeilingKlokOutputDto>
    {
        public static Expression<Func<VeilingKlok, VeilingKlokExtraInfo<ProductOutputDto>, VeilingKlokOutputDto>>
            EntityDto =>
            (entity, info) =>
                new VeilingKlokOutputDto
                {
                    Id = entity.Id,
                    Status = entity.Status,
                    PeakedLiveViews = entity.PeakedLiveViews,
                    ScheduledAt = entity.ScheduledAt,
                    StartedAt = entity.StartedAt,
                    EndedAt = entity.EndedAt,
                    CreatedAt = entity.CreatedAt,
                    RegionOrState = entity.RegionOrState,
                    Country = entity.Country,
                    TotalProducts = entity.ProductsIds.Count,
                    CurrentBids = info.TotalBids,
                    Products = info.Products,
                    HighestBidAmount = info.HighestPrice,
                    LowestBidAmount = info.LowestPrice
                };

        public static VeilingKlokOutputDto ToOutputDto(VeilingKlok entity,
            VeilingKlokExtraInfo<ProductOutputDto> info)
        {
            return new VeilingKlokOutputDto
            {
                Id = entity.Id,
                Status = entity.Status,
                PeakedLiveViews = entity.PeakedLiveViews,
                ScheduledAt = entity.ScheduledAt,
                StartedAt = entity.StartedAt,
                EndedAt = entity.EndedAt,
                CreatedAt = entity.CreatedAt,
                RegionOrState = entity.RegionOrState,
                Country = entity.Country,
                TotalProducts = entity.ProductsIds.Count,
                CurrentBids = info.TotalBids,
                Products = info.Products,
                HighestBidAmount = info.HighestPrice,
                LowestBidAmount = info.LowestPrice
            };
        }
    }
}