using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Exceptions;

namespace Domain.Entities;

[Table("Product")]
public class Product
{
    [Key] [Column("id")] public Guid Id { get; init; } = Guid.Empty;

    [Column("created_at")] public DateTimeOffset CreatedAt { get; private set; }

    [Column("image_url")] [Required] public required string ImageUrl { get; set; }

    [Column("dimension")] public string? Dimension { get; set; }

    [Column("name")] [Required] public required string Name { get; set; }

    [Column("description")] [Required] public required string Description { get; set; }

    [Column("auction_price")] public decimal? AuctionPrice { get; private set; }

    [Column("minimum_price")]
    [Required]
    [Range(0.00, double.MaxValue)]
    public decimal MinimumPrice { get; private set; }

    [Column("stock")]
    [Required]
    [Range(0, int.MaxValue)]
    public int Stock { get; private set; }

    [Column("region")] public string? Region { get; private set; }

    [Column("auctioned_at")] public DateTimeOffset? AuctionedAt { get; private set; }

    [Column("auctioned_count")] public int AuctionedCount { get; private set; } = 0;

    // --- Kweker Relationship ---
    [Column("kweker_id")] [Required] public required Guid KwekerId { get; init; } // FK to Kweker

    // --- Veilingklok Relationship (optional) ---
    [Column("veilingklok_id")] public Guid? VeilingKlokId { get; private set; } // FK to VeilingKlok (nullable)

    [Column("row_version")] [Timestamp] public ulong RowVersion { get; private set; }

    // --- OrderItems Relationship (reverse navigation) ---
    // Style 1: ✅ The Rich Model (Stronger Protection)
    private readonly List<int> IOrderIds = new();
    public IReadOnlyCollection<int> OrderIds => IOrderIds;

    [NotMapped] public bool IsBeingAuctioned => VeilingKlokId.HasValue;

    [NotMapped] public bool Auctioned => AuctionedCount > 0;

    private Product()
    {
    }

    public Product(decimal minimumPrice, int stock, string? region)
    {
        if (minimumPrice < 0)
            throw ProductValidationException.InvalidMinimumPrice();
        if (stock < 0)
            throw ProductValidationException.NegativeStockValue();

        Region = region;
        AuctionPrice = minimumPrice;
        MinimumPrice = minimumPrice;
        Stock = stock;
    }

    public void AddToVeilingKlok(Guid veilingKlokId)
    {
        AuctionedAt = DateTimeOffset.UtcNow;
        VeilingKlokId = veilingKlokId;
        AuctionedCount++;
    }

    public void RemoveVeilingKlok()
    {
        AuctionedAt = null;
        VeilingKlokId = null;
    }

    public void IncreaseStock(int amount)
    {
        if (amount < 0)
            throw ProductValidationException.NegativeStockValue();
        Stock += amount;
    }

    public void DecreaseStock(int amount)
    {
        if (amount < 0)
            throw ProductValidationException.NegativeStockValue();
        if (Stock - amount < 0)
            throw ProductValidationException.InsufficientProductStock();
        Stock -= amount;
    }

    public void UpdateAuctionPrice(decimal newPrice)
    {
        if (newPrice < MinimumPrice || newPrice < 0)
            throw ProductValidationException.InvalidProductPrice();
        AuctionPrice = newPrice;
    }

    public void Update(
        string? name,
        string? description,
        string? imageBase64,
        string? dimension,
        string? region,
        decimal? minimumPrice,
        int? stock
    )
    {
        if (minimumPrice < 0)
            throw ProductValidationException.InvalidMinimumPrice();
        if (stock.HasValue && stock.Value < 0)
            throw ProductValidationException.NegativeStockValue();
        if (minimumPrice.HasValue && minimumPrice.Value > AuctionPrice)
            throw ProductValidationException.MinimumPriceExceedsPrice();

        if (stock.HasValue)
            Stock = stock.Value;
        if (!string.IsNullOrEmpty(name))
            Name = name;
        if (!string.IsNullOrEmpty(description))
            Description = description;
        if (!string.IsNullOrEmpty(imageBase64))
            ImageUrl = imageBase64;
        if (!string.IsNullOrEmpty(dimension))
            Dimension = dimension;
        if (minimumPrice.HasValue)
            MinimumPrice = minimumPrice.Value;
        if (!string.IsNullOrEmpty(region))
            Region = region;
    }
}