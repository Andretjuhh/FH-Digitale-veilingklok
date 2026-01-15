namespace Application.Common.Models;

public sealed record KwekerPriceAverage(
    Guid KwekerId,
    string KwekerName,
    decimal AveragePrice,
    int SampleCount
);
