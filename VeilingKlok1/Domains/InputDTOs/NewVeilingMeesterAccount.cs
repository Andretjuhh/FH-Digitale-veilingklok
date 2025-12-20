using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace VeilingKlokApp.Models
{
    public class NewVeilingMeesterAccount
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Region is required")]
        [MaxLength(100)]
        public required string Regio { get; set; }

        [Required(ErrorMessage = "Authorisatie code is required")]
        [MaxLength(20)]
        public required string AuthorisatieCode { get; set; }
    }
}
