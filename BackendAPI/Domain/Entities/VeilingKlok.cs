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

    [Column("veiling_duration")] public required int VeilingDurationSeconds { get; init; } = 0;

    [Column("scheduled_at")] public required DateTimeOffset ScheduledAt { get; set; }
    [Column("started_at")] public DateTimeOffset? StartedAt { get; private set; }
    [Column("ended_at")] public DateTimeOffset? EndedAt { get; private set; }
    [Column("created_at")] public DateTimeOffset CreatedAt { get; init; }

    [Column("veiling_rounds")] public int VeilingRounds { get; set; } = 0;
    [Column("bidding_product_index")] public int BiddingProductIndex { get; private set; } = 0;

    [Column("total_products")] public int TotalProducts { get; private set; } = 0;

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

    // Navigation property for the many-to-many relationship with Product through VeilingKlokProduct
    private readonly List<VeilingKlokProduct> _veilingKlokProducts = new();
    public IReadOnlyCollection<VeilingKlokProduct> VeilingKlokProducts => _veilingKlokProducts;

    [NotMapped]
    public decimal LowestProductPrice =>
        _veilingKlokProducts.Count != 0 ? _veilingKlokProducts.Min(vkp => vkp.AuctionPrice) : 0;

    [NotMapped]
    public decimal HighestProductPrice =>
        _veilingKlokProducts.Count != 0 ? _veilingKlokProducts.Max(vkp => vkp.AuctionPrice) : 0;

    /// <summary>
    /// Gets the product IDs in the correct order (by position).
    /// </summary>
    public List<Guid> GetOrderedProductIds()
    {
        return _veilingKlokProducts
            .OrderBy(vkp => vkp.Position)
            .Select(vkp => vkp.ProductId)
            .ToList();
    }

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

    public void AddProduct(Guid productId, decimal auctionPrice)
    {
        // Check if product already exists in the collection
        var existingEntry = _veilingKlokProducts.FirstOrDefault(vkp => vkp.ProductId == productId);

        if (existingEntry != null)
            // Product is already in the klok, do nothing
            return;

        // Add new entry
        var position = _veilingKlokProducts.Any()
            ? _veilingKlokProducts.Max(vkp => vkp.Position) + 1
            : 0;

        _veilingKlokProducts.Add(new VeilingKlokProduct
        {
            VeilingKlokId = Id,
            ProductId = productId,
            AuctionPrice = auctionPrice,
            Position = position
        });
        TotalProducts++;
    }

    public void RemoveProductId(Guid productId)
    {
        if (Status >= VeilingKlokStatus.Started)
            throw KlokValidationException.KlokNotAvailableForUpdate();

        var entry = _veilingKlokProducts.FirstOrDefault(vkp => vkp.ProductId == productId);

        if (entry != null)
        {
            _veilingKlokProducts.Remove(entry);
            TotalProducts--;
        }
    }

    public void SetScheduledAt(DateTimeOffset scheduledAt)
    {
        ScheduledAt = scheduledAt;
    }

    public void SetBiddingProductIndex(int newIndex)
    {
        var activeProductsCount = _veilingKlokProducts.Count;
        if (activeProductsCount > 0 && (newIndex < 0 || newIndex >= activeProductsCount))
            throw KlokValidationException.InvalidProductIndex();
        BiddingProductIndex = newIndex;
    }

    public void UpdateProductPrice(Guid productId, decimal newPrice)
    {
        var entry = _veilingKlokProducts.FirstOrDefault(vkp => vkp.ProductId == productId);
        if (entry == null)
            throw KlokValidationException.ProductNotInVeilingKlok();

        entry.AuctionPrice = newPrice;
    }
}