using System.ComponentModel.DataAnnotations;

namespace VeilingKlokApp.Models.InputDTOs;

public class NewProduct
{
    [Required]
    public required string Name { get; set; }

    public string? Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public decimal MinimumPrice { get; set; }

    [Required]
    public int Quantity { get; set; }

    public string? ImageBase64 { get; set; }

    public string? Size { get; set; }
}
