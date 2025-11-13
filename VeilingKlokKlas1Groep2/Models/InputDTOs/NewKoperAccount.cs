using System.ComponentModel.DataAnnotations;
using VeilingKlokKlas1Groep2.Attributes;

namespace VeilingKlokApp.Models
{
    /// <summary>
    /// DTO for creating a new Koper (Buyer) account
    /// </summary>
    public class NewKoperAccount
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
         public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Adress { get; set; }

        [MaxLength(20)]
        public string? PostCode { get; set; }

        [MaxLength(100)]
        public string? Regio { get; set; }
    }
}
