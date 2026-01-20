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
                TotalProducts = veiling.TotalProducts,
                CurrentBids = info.TotalBids,
                Products = info.Products,
                VeilingDurationSeconds = veiling.VeilingDurationSeconds,
                LowestProductPrice = veiling.LowestProductPrice,
                HighestProductPrice = veiling.HighestProductPrice,
                VeilingRounds = veiling.VeilingRounds,
                CurrentProductIndex = veiling.BiddingProductIndex
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
            TotalProducts = entity.TotalProducts,
            CurrentBids = info.TotalBids,
            VeilingDurationSeconds = entity.VeilingDurationSeconds,
            Products = info.Products,
            LowestProductPrice = entity.LowestProductPrice,
            HighestProductPrice = entity.HighestProductPrice,
            VeilingRounds = entity.VeilingRounds,
            CurrentProductIndex = entity.BiddingProductIndex
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
                    TotalProducts = entity.TotalProducts,
                    VeilingDurationSeconds = entity.VeilingDurationSeconds,
                    CurrentBids = info.TotalBids,
                    Products = info.Products,
                    LowestProductPrice = info.LowestPrice,
                    HighestProductPrice = info.HighestPrice,
                    VeilingRounds = entity.VeilingRounds,
                    CurrentProductIndex = entity.BiddingProductIndex
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
                TotalProducts = entity.TotalProducts,
                VeilingDurationSeconds = entity.VeilingDurationSeconds,
                CurrentBids = info.TotalBids,
                Products = info.Products,
                LowestProductPrice = info.LowestPrice,
                HighestProductPrice = info.HighestPrice,
                VeilingRounds = entity.VeilingRounds,
                CurrentProductIndex = entity.BiddingProductIndex
            };
        }
    }
}