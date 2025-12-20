using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Input;

/// <summary>
/// DTO for user login requests
/// </summary>
public class RequestLoginDTO
{
    [Required(ErrorMessage = "ACCOUNT.EMAIL_REQUIRED")]
    [EmailAddress(ErrorMessage = "ACCOUNT.EMAIL_INVALID")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "ACCOUNT.PASSWORD_REQUIRED")]
    public required string Password { get; set; }
}