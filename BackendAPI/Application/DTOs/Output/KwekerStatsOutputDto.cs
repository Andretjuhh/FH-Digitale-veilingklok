namespace Application.DTOs.Output;

public class KwekerStatsOutputDto
{
    public required int TotalProducts { get; set; }
    public required int ActiveAuctions { get; set; }
    public required decimal TotalRevenue { get; set; }
    public required int StemsSold { get; set; }
}