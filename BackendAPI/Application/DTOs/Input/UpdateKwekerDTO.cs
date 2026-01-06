using System.ComponentModel.DataAnnotations;
using Application.Common.Attributes;

namespace Application.DTOs.Input;

public class UpdateKwekerDTO
{
    public string? CompanyName { get; set; }

    [Name(ErrorMessage = "ACCOUNT.FIRSTNAME_INVALID")]
    public string? FirstName { get; set; }

    [Name(ErrorMessage = "ACCOUNT.LASTNAME_INVALID")]
    public string? LastName { get; set; }

    [EmailAddress(ErrorMessage = "ACCOUNT.EMAIL_INVALID")]
    public string? Email { get; set; }

    public string? Password { get; set; }
    [Phone] public string? Telephone { get; set; }
    public string? KvkNumber { get; set; }

    // Address fields
    public AddressInputDto? Address { get; set; }
}