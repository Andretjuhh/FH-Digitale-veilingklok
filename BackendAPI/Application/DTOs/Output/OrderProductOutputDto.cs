namespace Application.DTOs.Output;

public class OrderProductOutputDto
{
    public required Guid ProductId { get; set; }
    public required string ProductName { get; set; }
    public required string ProductDescription { get; set; }
    public required string ProductImageUrl { get; set; }
    public required string CompanyName { get; set; }
    public required decimal? MinimalPrice { get; set; }
    public required int Quantity { get; set; }
    public required decimal PriceAtPurchase { get; set; }
    public required DateTimeOffset OrderedAt { get; set; }
}