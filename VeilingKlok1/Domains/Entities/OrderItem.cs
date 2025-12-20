using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeilingKlokApp.Models.Domain
{
    /// <summary>
    /// Represents the junction/payload entity between Order and Product.
    /// This allows an Order to have multiple Products with different quantities.
    /// </summary>
    [Table("OrderItem")]
    public class OrderItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // --- Quantity for this specific product in this order ---
        [Column("quantity")]
        [Required]
        public int Quantity { get; set; }

        // Optional: Store the price at the time of purchase (good for audit trail)
        [Column("price_at_purchase")]
        [Required, Range(0.00, double.MaxValue)]
        public decimal PriceAtPurchase { get; set; }

        // --- Order Relationship ---
        [Column("order_id")]
        [Required]
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = default!;

        // --- Product Relationship ---
        [Column("product_id")]
        [Required]
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;
    }
}
