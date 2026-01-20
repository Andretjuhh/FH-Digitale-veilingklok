using Domain.Enums;

namespace Application.DTOs.Output;

public class OrderOutputDto
{
    public required Guid Id { get; set; }
    public required Guid ProductId { get; set; }
    public required string ProductName { get; set; }
    public required string ProductDescription { get; set; }
    public required string ProductImageUrl { get; set; }
    public required string CompanyName { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required OrderStatus Status { get; set; }
    public required DateTimeOffset? ClosedAt { get; set; }
    public required decimal TotalAmount { get; set; }
    public required int TotalItems { get; set; }
}
