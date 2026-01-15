using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Junction/join table entity to track the many-to-many relationship between VeilingKlok and Product.
/// This maintains the history of which products were in each VeilingKlok session.
/// </summary>
[Table("VeilingKlokProduct")]
public class VeilingKlokProduct
{
    [Key] [Column("id")] public int Id { get; init; }

    [Column("veilingklok_id")] [Required] public required Guid VeilingKlokId { get; init; }

    [Column("product_id")] [Required] public required Guid ProductId { get; init; }

    [Column("auction_price")] public required decimal AuctionPrice { get; set; }

    /// <summary>
    /// The position/index of this product in the VeilingKlok queue.
    /// This allows maintaining the order of products in the auction.
    /// </summary>
    [Column("position")]
    [Required]
    public int Position { get; set; }

    [Column("added_at")] public DateTimeOffset AddedAt { get; init; } = DateTimeOffset.UtcNow;
}