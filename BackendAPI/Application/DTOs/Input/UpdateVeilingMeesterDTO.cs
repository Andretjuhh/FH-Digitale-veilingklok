using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Input;

public class UpdateVeilingMeesterDTO
{
    [EmailAddress(ErrorMessage = "ACCOUNT.EMAIL_INVALID")]
    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Regio { get; set; }
    public string? AuthorisatieCode { get; set; }
}