namespace Application.Common.Models;

public record OrderProductInfo(
    Guid ProductId,
    string ProductName,
    string ProductDescription,
    string ProductImageUrl,
    decimal PriceAtPurchase,
    decimal ProductMinimumPrice,
    string CompanyName,
    Guid KwekerId,
    int Quantity
);
