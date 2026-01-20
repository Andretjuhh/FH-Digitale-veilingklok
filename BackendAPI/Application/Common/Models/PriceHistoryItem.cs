namespace Application.Common.Models;

public sealed record PriceHistoryItem(
    Guid ProductId,
    string ProductName,
    Guid KwekerId,
    string KwekerName,
    decimal Price,
    DateTimeOffset PurchasedAt
);
