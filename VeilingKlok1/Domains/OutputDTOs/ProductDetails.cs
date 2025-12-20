namespace VeilingKlokApp.Models;

public class ProductDetails
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal MinimumPrice { get; set; }
    public int Quantity { get; set; }
    public string? ImageBase64 { get; set; }
    public string? Size { get; set; }
}
