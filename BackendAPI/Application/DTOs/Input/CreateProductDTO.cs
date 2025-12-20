using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Input;

public class CreateProductDTO
{
    [Required(ErrorMessage = "PRODUCT.NAME_REQUIRED")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "PRODUCT.DESCRIPTION_REQUIRED")]
    public required string Description { get; set; }

    [Required(ErrorMessage = "PRODUCT.PRICE_REQUIRED")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "PRODUCT.MIN_PRICE_REQUIRED")]
    public decimal MinimumPrice { get; set; }

    [Required(ErrorMessage = "PRODUCT.STOCK_REQUIRED")]
    public int Stock { get; set; }

    [Base64String(ErrorMessage = "PRODUCT.IMAGE_INVALID_BASE64")]
    public required string ImageBase64 { get; set; }

    public string? Dimension { get; set; }
}