using System.ComponentModel.DataAnnotations;
using VeilingKlokKlas1Groep2.Attributes;

namespace VeilingKlokApp.Models
{
    public class NewKwekerAccount
    {
        // Basic information layout of a new Kweker account
        [Required(ErrorMessage = "Company name is required")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }  = string.Empty;
        public DateTime CreatedAt { get; set; } 
        public string Telephone { get; set; }

        [MaxLength(255)]
        public string? Adress { get; set; }

        [MaxLength(100)]
        public string? Regio { get; set; }
        [Required(ErrorMessage = "Kweker number is required")]
        [MaxLength(100)]
        public string? KvkNumber { get; set; } = string.Empty;
    }
}
