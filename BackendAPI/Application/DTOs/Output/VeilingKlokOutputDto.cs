using Domain.Enums;

namespace Application.DTOs.Output;

public class VeilingKlokOutputDto
{
    public required Guid Id { get; set; }
    public required VeilingKlokStatus Status { get; set; }
    public required int PeakedLiveViews { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required string RegionOrState { get; set; }
    public required string Country { get; set; }
    public required int CurrentBids { get; set; }
    public required int TotalProducts { get; set; }
    public DateTimeOffset? ScheduledAt { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }

    public decimal? HighestBidAmount { get; set; }
    public decimal? LowestBidAmount { get; set; }
    public required List<ProductOutputDto> Products { get; set; }
}