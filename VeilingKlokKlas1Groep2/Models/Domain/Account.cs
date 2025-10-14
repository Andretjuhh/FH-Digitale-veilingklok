using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeilingKlok.Models.Domain
{

    //Account Model, represents the Account table in the database
    public class Account
    {
        [Key]
        [Column("id")]
        public int Id { get; set; } // The PK

        [Column("email")]
        [Required, MaxLength(255)]
        public string Email { get; set; }

        [Column("password")]
        [Required, MaxLength(255)]
        public string Password { get; set; }

        public Koper Koper { get; set; } // Navigation property to Kweker

        public Kweker Kweker { get; set; } // Navigation property to Kweker
    }
}
