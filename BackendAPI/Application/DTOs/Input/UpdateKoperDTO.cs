using System.ComponentModel.DataAnnotations;
using Application.Common.Attributes;

namespace Application.DTOs.Input;

public class UpdateKoperDTO
{
    [Required(ErrorMessage = "ACCOUNT.EMAIL_REQUIRED")]
    [EmailAddress(ErrorMessage = "ACCOUNT.EMAIL_INVALID")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "ACCOUNT.PASSWORD_REQUIRED")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "ACCOUNT.FIRSTNAME_REQUIRED")]
    [Name(ErrorMessage = "ACCOUNT.FIRSTNAME_INVALID")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "ACCOUNT.LASTNAME_REQUIRED")]
    [Name(ErrorMessage = "ACCOUNT.LASTNAME_INVALID")]
    public required string LastName { get; set; }

    [Required(ErrorMessage = "ACCOUNT.TELEPHONE_REQUIRED")]
    [Phone]
    [MaxLength(20)]
    public required string Telephone { get; set; }

    [Required(ErrorMessage = "ACCOUNT.ADDRESS_REQUIRED")]
    public required AddressInputDto Address { get; set; }
}