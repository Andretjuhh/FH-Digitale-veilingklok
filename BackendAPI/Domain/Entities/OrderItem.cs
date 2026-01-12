using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Domain.Exceptions;

namespace Domain.Entities;

/// <summary>
/// Represents the junction/payload entity between Order and Product.
/// This allows an Order to have multiple Products with different quantities.
/// </summary>
[Table("OrderItem")]
public class OrderItem
{
    [Key] [Column("id")] public int Id { get; init; }

    // --- Quantity for this specific product in this order ---
    [Column("quantity")] [Required] public int Quantity { get; private set; }

    // Store the price at the time of purchase (good for audit trail)
    [Column("price_at_purchase")]
    [Required]
    [Range(0.00, double.MaxValue)]
    public decimal PriceAtPurchase { get; private set; }

    // --- Product Relationship ---
    [Column("veilingklok_id")] [Required] public required Guid VeilingKlokId { get; init; }

    [Column("product_id")] [Required] public Guid ProductId { get; }

    [Column("order_id")] [Required] public Guid OrderId { get; }

    [Column("created_at")] public DateTimeOffset CreatedAt { get; init; }

    [Column("row_version")] [Timestamp] public ulong RowVersion { get; private set; }

    private OrderItem()
    {
    }

    public OrderItem(decimal purchasedPrice, int quantity, Product product, Guid orderId)
    {
        if (purchasedPrice < product.MinimumPrice)
            throw OrderValidationException.InvalidProductPrice();
        if (quantity < 1)
            throw OrderValidationException.MinOrderQuantityOne();

        PriceAtPurchase = purchasedPrice;
        Quantity = quantity;
        OrderId = orderId;
        ProductId = product.Id;
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity < 1)
            throw OrderValidationException.MinOrderQuantityOne();
        Quantity = newQuantity;
    }
}