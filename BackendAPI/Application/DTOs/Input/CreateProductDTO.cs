using System.ComponentModel.DataAnnotations;
using Application.Common.Attributes;

namespace Application.DTOs.Input;

public class CreateProductDTO
{
    [Required(ErrorMessage = "PRODUCT.NAME_REQUIRED")]
    [Name(ErrorMessage = "PRODUCT.NAME_INVALID")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "PRODUCT.DESCRIPTION_REQUIRED")]
    public required string Description { get; set; }

    [Required(ErrorMessage = "PRODUCT.MIN_PRICE_REQUIRED")]
    [Range(1, double.MaxValue, ErrorMessage = "PRODUCT.MIN_PRICE_INVALID")]
    public decimal MinimumPrice { get; set; }

    [Required(ErrorMessage = "PRODUCT.STOCK_REQUIRED")]
    [Range(0, int.MaxValue, ErrorMessage = "PRODUCT.STOCK_INVALID")]
    public int Stock { get; set; }

    public string? ImageBase64 { get; set; }

    public string? Dimension { get; set; }
}
