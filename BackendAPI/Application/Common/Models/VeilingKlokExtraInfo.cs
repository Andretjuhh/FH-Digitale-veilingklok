using Domain.Entities;

namespace Application.Common.Models;

public record VeilingKlokExtraInfo<T>(
    int TotalBids,
    List<T> Products,
    decimal? HighestPrice = null,
    decimal? LowestPrice = null
);