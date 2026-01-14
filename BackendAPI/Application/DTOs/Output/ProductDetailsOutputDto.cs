namespace Application.DTOs.Output;

public class ProductDetailsOutputDto
{
    public required Guid Id { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
    public required decimal? AuctionPrice { get; set; }
    public required decimal MinimumPrice { get; set; }
    public required int Stock { get; set; }
    public required string? Region { get; set; }
    public required string ImageBase64 { get; set; }
    public required string Dimension { get; set; }
    public required bool Auctioned { get; set; }
    public required int AuctionedCount { get; set; }
    public required DateTimeOffset? AuctionedAt { get; set; }
    public required Guid KwekerId { get; set; }
    public required string CompanyName { get; set; }
}