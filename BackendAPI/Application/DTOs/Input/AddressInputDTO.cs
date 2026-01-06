using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Input;

public class AddressInputDto
{
    [Required(ErrorMessage = "ADDRESS.STREET_REQUIRED")]
    [MaxLength(255)]
    public required string Street { get; set; }

    [Required(ErrorMessage = "ADDRESS.CITY_REQUIRED")]
    [MaxLength(50)]
    public required string City { get; set; }

    [Required(ErrorMessage = "ADDRESS.REGION_REQUIRED")]
    [MaxLength(50)]
    public required string RegionOrState { get; set; }

    [Required(ErrorMessage = "ADDRESS.POSTCODE_REQUIRED")]
    [MaxLength(10)]
    public required string PostalCode { get; set; }

    [Required(ErrorMessage = "ADDRESS.COUNTRY_REQUIRED")]
    [MaxLength(2)]
    public required string Country { get; set; }
}