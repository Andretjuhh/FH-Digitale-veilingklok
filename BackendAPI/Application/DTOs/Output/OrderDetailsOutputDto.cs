using Domain.Enums;

namespace Application.DTOs.Output;

public class OrderDetailsOutputDto
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }
    public required decimal TotalAmount { get; set; }
    public required int TotalItems { get; set; }
    public required List<OrderItemOutputDto> Products { get; set; }
}