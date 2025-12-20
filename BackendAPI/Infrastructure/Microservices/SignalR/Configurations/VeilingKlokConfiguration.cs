namespace Infrastructure.Microservices.SignalR.Configurations;

public class VeilingKlokConfiguration
{
    public int TickIntervalMs { get; set; } = 50; // 50ms = 20 ticks per second
    public decimal PriceDecreasePerTick { get; set; } = 0.10m; // €0.10 per tick
    public decimal BidPriceIncrease { get; set; } = 5.00m; // €5 increase after bid
    public int BidPauseMs { get; set; } = 2000; // 2 second pause after bid
    public int ProductTransitionMs { get; set; } = 3000; // 3 second transition
}