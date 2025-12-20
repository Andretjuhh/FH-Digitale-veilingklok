using System.ComponentModel.DataAnnotations;

namespace VeilingKlokApp.Models.InputDTOs;

/// <summary>
/// DTO for updating an existing Product
/// </summary>
public class UpdateProduct
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Product price must be greater than zero")]
    public decimal? Price { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Minimum price cannot be negative")]
    public decimal? MinimumPrice { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int? Quantity { get; set; }

    public string? ImageBase64 { get; set; }
    public string? Size { get; set; }
}
