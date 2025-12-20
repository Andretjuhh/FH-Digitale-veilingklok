using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Input;

public class CreateVeilingMeesterDTO
{
    [Required(ErrorMessage = "ACCOUNT.EMAIL_REQUIRED")]
    [EmailAddress(ErrorMessage = "ACCOUNT.EMAIL_INVALID")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "ACCOUNT.PASSWORD_REQUIRED")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "ACCOUNT.REGION_REQUIRED")]
    [MaxLength(50)]
    public required string Region { get; set; }

    [Required(ErrorMessage = "ACCOUNT.COUNTRY_REQUIRED")]
    [MaxLength(2)]
    public required string CountryCode { get; set; }

    [Required(ErrorMessage = "ACCOUNT.AUTH_CODE_REQUIRED")]
    [MaxLength(20)]
    public required string AuthorisatieCode { get; set; }
}