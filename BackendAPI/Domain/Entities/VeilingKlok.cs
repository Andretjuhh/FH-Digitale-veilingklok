using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

[Table("Veilingklok")]
public class VeilingKlok
{
    [Key] [Column("id")] public Guid Id { get; init; } = Guid.Empty;

    [Column("status")] public VeilingKlokStatus Status { get; private set; } = VeilingKlokStatus.Scheduled;

    [Column("peaked_live_views")] public int PeakedLiveViews { get; private set; } = 0;

    [Column("bidding_product_index")] public int BiddingProductIndex { get; private set; } = 0;

    [Column("veiling_duration")] public required int VeilingDurationMinutes { get; init; } = 0;

    [Column("scheduled_at")] public required DateTimeOffset ScheduledAt { get; set; }

    [Column("started_at")] public DateTimeOffset? StartedAt { get; private set; }

    [Column("ended_at")] public DateTimeOffset? EndedAt { get; private set; }

    [Column("created_at")] public DateTimeOffset CreatedAt { get; init; }

    [Column("highest_price")] public decimal HighestPrice { get; set; } = 0;
    [Column("lowest_price")] public decimal LowestPrice { get; set; } = 0;

    [Column("state_or_province")]
    [Required]
    [MaxLength(50)]
    public required string RegionOrState { get; init; }

    [Column("country")]
    [Required]
    [MaxLength(2)]
    public required string Country { get; init; }

    [Column("veilingmeester_id")]
    [Required]
    public Guid? VeilingmeesterId { get; private set; }

    [Column("row_version")] [Timestamp] public ulong RowVersion { get; private set; }

    // Style 1: ✅ The Rich Model (Stronger Protection)
    // Navigation property for the one-to-many relationship with Product
    private readonly List<Guid> IProductsIds = new();
    public IReadOnlyCollection<Guid> ProductsIds => IProductsIds;

    // Style 1: ✅ The Rich Model (Stronger Protection)
    private readonly List<Guid> IOrdersIds = new();
    public IReadOnlyCollection<Guid> OrdersIds => IOrdersIds;

    public void UpdatePeakedLiveViews(int newPeakedLiveViews)
    {
        if (newPeakedLiveViews < PeakedLiveViews)
            throw KlokValidationException.InvalidLivePeakedView();
        PeakedLiveViews = newPeakedLiveViews;
    }

    public void UpdateStatus(VeilingKlokStatus newStatus)
    {
        if (newStatus > Status)
            Status = newStatus;

        if (newStatus == VeilingKlokStatus.Started)
            StartedAt = DateTimeOffset.UtcNow;

        if (newStatus == VeilingKlokStatus.Ended)
            EndedAt = DateTimeOffset.UtcNow;

        if (newStatus == VeilingKlokStatus.Scheduled)
            ScheduledAt = DateTimeOffset.UtcNow;
    }

    public void AssignVeilingmeester(Guid veilingmeesterId)
    {
        if (Status >= VeilingKlokStatus.Started)
            throw KlokValidationException.KlokNotAvailableForUpdate();
        VeilingmeesterId = veilingmeesterId;
    }

    public void AddProductId(Guid productId)
    {
        if (!IProductsIds.Contains(productId))
            IProductsIds.Add(productId);
    }

    public void SetScheduledAt(DateTimeOffset scheduledAt)
    {
        ScheduledAt = scheduledAt;
    }

    public void SetBiddingProductIndex(int newIndex)
    {
        if (newIndex < 0 || newIndex >= IProductsIds.Count)
            throw KlokValidationException.InvalidProductIndex();
        BiddingProductIndex = newIndex;
    }

    public void UpdatePriceRange(decimal addedProductPrice)
    {
        if (addedProductPrice > HighestPrice)
            HighestPrice = addedProductPrice;
        else if (LowestPrice == 0 || addedProductPrice < LowestPrice)
            LowestPrice = addedProductPrice;
    }
}
