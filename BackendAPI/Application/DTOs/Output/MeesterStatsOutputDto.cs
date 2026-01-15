namespace Application.DTOs.Output;

public class MeesterStatsOutputDto
{
    public required int TotalVeilingKlokken { get; set; }
    public required int ActiveVeilingKlokken { get; set; }
    public required int TotalProducts { get; set; }
    public required int TotalOrders { get; set; }
}
