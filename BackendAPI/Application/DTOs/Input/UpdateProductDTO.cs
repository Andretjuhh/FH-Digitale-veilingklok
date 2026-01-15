using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Input;

/// <summary>
/// DTO for updating an existing Product
/// </summary>
public class UpdateProductDTO
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "PRODUCT.MIN_PRICE_MIN_ZERO")]
    public decimal? MinimumPrice { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "PRODUCT.STOCK_MIN_ZERO")]
    public int? Stock { get; set; }

    public string? ImageBase64 { get; set; }
    public string? Dimension { get; set; }
    public string? Region { get; set; }

}