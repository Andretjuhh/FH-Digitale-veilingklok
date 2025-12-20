using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

[Table("Order")]
public class Order
{
    [Key]
    [Required]
    [Column("id")]
    public Guid Id { get; init; } = Guid.Empty;

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    [Column("status")]
    public OrderStatus Status { get; private set; } = OrderStatus.Open;

    [Column("closed_at")]
    public DateTimeOffset? ClosedAt { get; private set; }

    // --- Koper Relationship ---
    [Column("koper_id")]
    [Required]
    public Guid KoperId { get; init; }

    [Column("veilingklok_id")]
    [Required]
    public required Guid VeilingKlokId { get; init; }

    [Column("row_version")]
    [Timestamp]
    public ulong RowVersion { get; private set; }

    // --- OrderItems Relationship (many products per order) ---
    // Style 1: ✅ The Rich Model (Stronger Protection)
    private readonly List<OrderItem> IOrderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => IOrderItems;

    public IReadOnlyCollection<Guid> ProductsIds =>
        OrderItems.Select(oi => oi.ProductId).Distinct().ToList().AsReadOnly();

    private Order() { }

    public Order(Guid koperId)
    {
        KoperId = koperId;
        Status = OrderStatus.Open;
    }

    public void UpdateOrderStatus(OrderStatus newStatus)
    {
        // Validate status transition
        if (newStatus < Status)
            throw OrderValidationException.OrderAlreadyClosed();

        // Update status
        Status = newStatus;

        // Set ClosedAt if the order is now closed
        if (Status >= OrderStatus.Processing)
            ClosedAt = DateTimeOffset.UtcNow;
    }

    public void AddItem(OrderItem item)
    {
        // Check if the order is still open
        if (Status != OrderStatus.Open)
            throw OrderValidationException.OrderIsNotOpen();

        // Check if the quantity is valid
        if (item.Quantity < 1)
            throw OrderValidationException.MinOrderQuantityOne();

        IOrderItems.Add(item);
    }

    public void RemoveItem(OrderItem item)
    {
        // Check if the order is still open
        if (Status != OrderStatus.Open)
            throw OrderValidationException.OrderIsNotOpen();

        var isFromOrder = IOrderItems.Contains(item);
        if (!isFromOrder)
            throw OrderValidationException.InvalidOrderedItem();
        IOrderItems.Remove(item);
    }
}
