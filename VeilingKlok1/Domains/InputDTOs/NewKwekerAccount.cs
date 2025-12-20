using System.ComponentModel.DataAnnotations;
using VeilingKlokApp.Attributes;

namespace VeilingKlokApp.Models
{
    public class NewKwekerAccount
    {
        // Basic information layout of a new Kweker account
        [Required(ErrorMessage = "Company name is required")]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Telephone is required")]
        [Phone]
        [MaxLength(20)]
        public required string Telephone { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(255)]
        public required string Adress { get; set; }

        [Required(ErrorMessage = "Postal code is required")]
        [MaxLength(8)]
        public required string PostCode { get; set; }

        [Required(ErrorMessage = "Region is required")]
        [MaxLength(100)]
        public required string Regio { get; set; }

        [Required(ErrorMessage = "KVK number is required")]
        [MaxLength(50)]
        public required string KvkNumber { get; set; }
    }
}
