using Domain.Entities;

namespace Infrastructure.Microservices.SignalR.Models;

public class VeilingProductState
{
    public required Guid ProductId { get; init; }
    public required int InitialStock { get; init; }
    public required decimal StartingPrice { get; init; }
    public required decimal CurrentPrice { get; set; }
    public required decimal LowestPrice { get; init; }

    // This will be updated as bids are placed
    public int RemainingStock { get; private set; }
    public decimal LastBidPrice { get; private set; }

    public static VeilingProductState Create(Product product)
    {
        return new VeilingProductState
        {
            ProductId = product.Id,
            InitialStock = product.Stock,
            RemainingStock = product.Stock,
            StartingPrice = product.AuctionPrice!.Value,
            CurrentPrice = product.AuctionPrice!.Value,
            LowestPrice = product.MinimumPrice
        };
    }

    public void PlaceBid(decimal bidPrice, int quantity)
    {
        LastBidPrice = Math.Max(bidPrice, LowestPrice);
        RemainingStock = Math.Max(RemainingStock - quantity, 0);
    }
}