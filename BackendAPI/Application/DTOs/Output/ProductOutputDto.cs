namespace Application.DTOs.Output;

public class ProductOutputDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required string Description { get; set; }
    public required string ImageUrl { get; set; }
    public required decimal? AuctionedPrice { get; set; }
    public required DateTimeOffset? AuctionedAt { get; set; }
    public required string? Region { get; set; }
    public required string Dimension { get; set; }
    public required int Stock { get; set; }
    public required string CompanyName { get; set; }
}