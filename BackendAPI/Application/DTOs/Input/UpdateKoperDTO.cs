using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Input;

public class UpdateKoperDTO
{
    [Required(ErrorMessage = "ACCOUNT.EMAIL_REQUIRED")]
    [EmailAddress(ErrorMessage = "ACCOUNT.EMAIL_INVALID")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "ACCOUNT.PASSWORD_REQUIRED")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "ACCOUNT.FIRSTNAME_REQUIRED")]
    [MaxLength(100)]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "ACCOUNT.LASTNAME_REQUIRED")]
    [MaxLength(100)]
    public required string LastName { get; set; }

    [Required(ErrorMessage = "ACCOUNT.TELEPHONE_REQUIRED")]
    [Phone]
    [MaxLength(20)]
    public required string Telephone { get; set; }

    [Required(ErrorMessage = "ACCOUNT.ADDRESS_REQUIRED")]
    public required AddressInputDto Address { get; set; }
}