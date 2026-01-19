using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Microservices.SignalR.Models;

public class VeilingKlokState
{
    public Guid ClockId { get; init; }
    public VeilingKlokStatus Status { get; set; }
    public string Country { get; init; }
    public string RegionOrState { get; init; }
    public List<VeilingProductState> Products { get; init; } = new();
    public int VeilingDurationSeconds { get; init; }
    public int TotalRounds { get; private set; }

    // Price range for the klok based on all products
    public int HighestPriceRange { get; init; }
    public int LowestPriceRange { get; init; }

    // We will tick the current price based on time
    public decimal CurrentPrice { get; private set; }
    public int CurrentProductIndex { get; set; }
    public DateTimeOffset VeilingStartTime { get; private set; }
    public DateTimeOffset VeilingEndTime { get; private set; }

    public VeilingKlokState(VeilingKlok veilingKlok, List<Product> products)
    {
        ClockId = veilingKlok.Id;
        Status = veilingKlok.Status;
        CurrentProductIndex = veilingKlok.BiddingProductIndex;
        VeilingDurationSeconds = veilingKlok.VeilingDurationSeconds;
        Country = veilingKlok.Country;
        RegionOrState = veilingKlok.RegionOrState;
        TotalRounds = veilingKlok.VeilingRounds;
        LowestPriceRange = 0;

        if (products.Count == 0)
        {
            CurrentProductIndex = 0;
            CurrentPrice = 0;
            HighestPriceRange = 0;
            return;
        }

        if (CurrentProductIndex < 0 || CurrentProductIndex >= products.Count)
            CurrentProductIndex = 0;

        var auctionPrices = products
            .Select(p => p.AuctionPrice ?? 0)
            .ToList();

        CurrentPrice = auctionPrices[CurrentProductIndex];
        HighestPriceRange = decimal.ToInt32(auctionPrices.Max());
        foreach (var product in products)
            Products.Add(VeilingProductState.Create(product));
    }

    // Start the veiling clock
    public void StartVeiling()
    {
        Status = VeilingKlokStatus.Started;
        VeilingStartTime = DateTimeOffset.UtcNow;
    }

    //  Check if the current veiling round has ended
    public bool CurrentProductVeilingEnded()
    {
        return DateTimeOffset.UtcNow >= VeilingEndTime;
    }

    // Start veiling for a specific product
    public void StartProductVeiling(Guid productId)
    {
        var productIndex = Products.FindIndex(p => p.ProductId == productId);
        CurrentProductIndex = productIndex;
        VeilingStartTime = DateTimeOffset.UtcNow;
        VeilingEndTime = VeilingStartTime.AddSeconds(VeilingDurationSeconds);
        CurrentPrice = GetTickedProductPrice(VeilingStartTime);
        TotalRounds++;
    }

    // Get the current product being auctioned
    public VeilingProductState GetCurrentProduct()
    {
        return Products[CurrentProductIndex];
    }

    // Calculate the current price based on the elapsed time
    public decimal GetTickedProductPrice(DateTimeOffset date)
    {
        if (date >= VeilingEndTime)
            return GetCurrentProduct().LowestPrice;

        var totalTicks = (VeilingEndTime - VeilingStartTime).Ticks;
        var elapsedTicks = (date - VeilingStartTime).Ticks;

        // Calculate the percentage of time passed (0.0 to 1.0)
        var progress = (decimal)elapsedTicks / totalTicks;

        var product = GetCurrentProduct();
        var priceRange = product.StartingPrice - product.LowestPrice;

        // Linear price drop: StartPrice - (Range * Progress)
        var newPrice = product.StartingPrice - priceRange * progress;

        return Math.Max(newPrice, product.LowestPrice);
    }

    // This method is being called by a ticker hosted server service
    public void Tick()
    {
        CurrentPrice = GetTickedProductPrice(DateTimeOffset.UtcNow);
        GetCurrentProduct().CurrentPrice = CurrentPrice;
    }
}