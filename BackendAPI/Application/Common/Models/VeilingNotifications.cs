namespace Application.Common.Models;

public static class VeilingNotifications
{
    public record RegionVeilingStartedNotification
    {
        public required Guid ClockId { get; init; }
        public required string Country { get; init; }
        public required string Region { get; init; }
        public required DateTimeOffset StartTime { get; init; }
    }

    public record VeilingKlokStateNotification
    {
        public required Guid ClockId { get; init; }
        public required Guid CurrentProductId { get; init; }
        public required decimal CurrentPrice { get; init; }
        public required decimal StartingPrice { get; init; }
        public required decimal LowestPrice { get; init; }
        public required int RemainingQuantity { get; init; }
        public required int LiveViewerCount { get; init; }
        public required DateTimeOffset EndTime { get; init; }
    }

    public record VeilingBodNotification
    {
        public required Guid ProductId { get; init; }
        public required int Quantity { get; init; }
        public required decimal Price { get; init; }
        public required int RemainingQuantity { get; init; }
    }

    public record VeilingProductChangedNotification
    {
        public required Guid ProductId { get; init; }
        public required decimal StartingPrice { get; init; }
        public required int Quantity { get; init; }
    }

    public record VeilingPriceTickNotification
    {
        public required Guid ClockId { get; init; }
        public required Guid ProductId { get; init; }
        public required decimal CurrentPrice { get; init; }
        public required DateTimeOffset TickTime { get; init; }
    }

    public record VeilingProductWaitingNotification
    {
        public required Guid ClockId { get; init; }
        public required Guid CompletedProductId { get; init; }
    }
}
