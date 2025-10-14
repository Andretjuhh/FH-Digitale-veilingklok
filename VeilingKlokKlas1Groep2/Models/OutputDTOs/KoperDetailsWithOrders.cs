namespace VeilingKlok.Models
{
    public class KoperDetailsWithOrders
{
    public int AccountId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Adress { get; set; }
    public string Regio { get; set; }
    
    // The new property to hold the list of related orders
    public ICollection<OrderDetails> Orders { get; set; }
}
}
