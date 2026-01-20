namespace Application.DTOs.Output;

public class ProductOutputDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required string Description { get; set; }
    public required string ImageUrl { get; set; }
    public required decimal? AuctionedPrice { get; set; } // Always returned
    public required decimal? MinimumPrice { get; set; } // If koper requests it return null
    public required DateTimeOffset? AuctionedAt { get; set; }
    public required string? Region { get; set; }
    public required string Dimension { get; set; }
    public required int Stock { get; set; }
    public required string CompanyName { get; set; }
    public required bool AuctionPlanned { get; set; }
    public required string KwekerId { get; set; }
}
