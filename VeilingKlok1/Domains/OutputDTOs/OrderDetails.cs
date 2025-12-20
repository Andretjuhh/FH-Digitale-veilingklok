namespace VeilingKlokApp.Models
{
    public class OrderDetails
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public DateTime BoughtAt { get; set; }
        // Add other properties from the Order model as needed
    }
}
