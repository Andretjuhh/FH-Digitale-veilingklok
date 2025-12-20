namespace VeilingKlokApp.Models
{
    public class KoperDetailsWithOrders
    {
        public Guid AccountId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Adress { get; set; }
        public string? Regio { get; set; }

        // The new property to hold the list of related orders
        public ICollection<OrderDetails> Orders { get; set; } = new List<OrderDetails>();
    }
}
