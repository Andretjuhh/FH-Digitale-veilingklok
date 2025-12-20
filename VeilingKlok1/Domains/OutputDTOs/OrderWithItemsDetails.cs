namespace VeilingKlokApp.Models.OutputDTOs
{
    /// <summary>
    /// DTO for displaying product details within an order
    /// </summary>
    public class OrderItemDetails
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuantityBought { get; set; }
        public string? ImageUrl { get; set; }
    }

    /// <summary>
    /// DTO for displaying a single order with all its product details
    /// </summary>
    public class OrderWithItemsDetails
    {
        public Guid OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDetails> Products { get; set; } = new List<OrderItemDetails>();
    }
}
