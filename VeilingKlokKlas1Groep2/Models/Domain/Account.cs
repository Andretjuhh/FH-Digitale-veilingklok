using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeilingKlokApp.Models.Domain
{

    //Account Model, represents the Account table in the database
    // Its abstract because we never create an Account directly it is always a Koper or Kweker
    [Table("Account")]
    public abstract class Account
    {
        [Key]
        [Column("id")]
        public int Id { get; set; } // The PK

        [Column("email")]
        [EmailAddress]
        [Required, MaxLength(255)]
        public string Email { get; set; }

        [Column("password")]
        [Required, MaxLength(255)]
        public string Password { get; set; }

        [Column("created_at")]
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
