using System.ComponentModel.DataAnnotations;

namespace VeilingKlokKlas1Groep2.Models.InputDTOs
{
    /// <summary>
    /// DTO for user login requests
    /// </summary>
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
