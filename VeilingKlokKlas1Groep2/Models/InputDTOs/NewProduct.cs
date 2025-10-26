namespace VeilingKlokKlas1Groep2.Models.InputDTOs;

public class NewProduct
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal MinimumPrice { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
    public string? Size { get; set; }
    public int KwekerId { get; set; }
}
