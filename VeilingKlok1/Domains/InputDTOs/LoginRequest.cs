using System.ComponentModel.DataAnnotations;

namespace VeilingKlokApp.Models.InputDTOs
{
    /// <summary>
    /// DTO for user login requests
    /// </summary>
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }
    }
}
