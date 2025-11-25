using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace VeilingKlokKlas1Groep2.Models
{
    public class NewVeilingMeesterAccount
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Regio { get; set; }

        [Required(ErrorMessage = "Authorisatie code is required")]
        [MaxLength(20)]
        public string AuthorisatieCode { get; set; } = string.Empty;
    }
}