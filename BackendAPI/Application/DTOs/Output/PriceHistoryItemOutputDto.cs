namespace Application.DTOs.Output;

public class PriceHistoryItemOutputDto
{
    public required Guid ProductId { get; set; }
    public required string ProductName { get; set; }
    public required Guid KwekerId { get; set; }
    public required string KwekerName { get; set; }
    public required decimal Price { get; set; }
    public required DateTimeOffset PurchasedAt { get; set; }
}
