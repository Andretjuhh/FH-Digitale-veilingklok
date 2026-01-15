namespace Application.DTOs.Output;

public class KwekerOrderStatsOutputDto
{
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int CanceledOrders { get; set; }
}
