using Domain.Enums;

namespace Application.DTOs.Output;

public class VeilingKlokDetailsOutputDto
{
    public required Guid Id { get; set; }
    public required VeilingKlokStatus Status { get; set; }
    public required int PeakedLiveViews { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required string RegionOrState { get; set; }
    public required string Country { get; set; }
    public required int CurrentBids { get; set; }
    public required int TotalProducts { get; set; }
    public required int VeilingDurationSeconds { get; set; }
    public DateTimeOffset? ScheduledAt { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }

    public int? VeilingRounds { get; set; }
    public int? CurrentProductIndex { get; set; }
    public decimal LowestProductPrice { get; set; }
    public decimal HighestProductPrice { get; set; }
    public required List<ProductDetailsOutputDto> Products { get; set; }
}