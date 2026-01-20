using Domain.Enums;

namespace Application.DTOs.Output;

public class OrderKwekerOutputDto
{
    public required Guid Id { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required OrderStatus Status { get; set; }
    public required DateTimeOffset? ClosedAt { get; set; }
    public required int Quantity { get; set; }
    public required decimal TotalPrice { get; set; }
    public required List<OrderProductOutputDto> Products { get; set; }
    public required KoperInfoOutputDto KoperInfo { get; set; }
}
