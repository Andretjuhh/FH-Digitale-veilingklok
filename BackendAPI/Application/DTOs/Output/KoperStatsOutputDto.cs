namespace Application.DTOs.Output;

public class KoperStatsOutputDto
{
    public required int TotalOrders { get; set; }
    public required int PendingOrders { get; set; }
    public required int CompletedOrders { get; set; }
    public required int CanceledOrders { get; set; }
}
