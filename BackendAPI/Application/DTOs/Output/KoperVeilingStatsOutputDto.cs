namespace Application.DTOs.Output;

public class KoperVeilingStatsOutputDto
{
    public required int ActiveAuctions { get; set; }
    public required int ScheduledAuctions { get; set; }
    public required int AvailableProducts { get; set; }
    public required int YourPurchases { get; set; }
}
