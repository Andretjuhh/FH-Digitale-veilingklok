namespace Application.DTOs.Output;

public class KwekerStatsOutputDto
{
    public required int TotalProducts { get; set; }
    public required int ActiveAuctions { get; set; }
    public required decimal TotalRevenue { get; set; }
    public required int OrdersReceived { get; set; }
    public required List<MonthlyRevenueDto> MonthlyRevenue { get; set; }
    public required List<DailyRevenueDto> DailyRevenue { get; set; }
}

public class MonthlyRevenueDto
{
    public required int Year { get; set; }
    public required int Month { get; set; }
    public required string MonthName { get; set; }
    public required decimal Revenue { get; set; }
}

public class DailyRevenueDto
{
    public required int Year { get; set; }
    public required int Month { get; set; }
    public required int Day { get; set; }
    public required string DateLabel { get; set; }
    public required decimal Revenue { get; set; }
}