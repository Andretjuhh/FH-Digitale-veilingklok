namespace Application.Common.Models;

public record OrderItemProduct(
    string ProductName,
    string ProductDescription,
    string ProductImageUrl,
    string CompanyName
);