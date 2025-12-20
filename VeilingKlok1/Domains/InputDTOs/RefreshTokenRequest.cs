using System.ComponentModel.DataAnnotations;

namespace VeilingKlokApp.Models.InputDTOs
{
    /// <summary>
    /// DTO for refresh token requests
    /// </summary>
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
