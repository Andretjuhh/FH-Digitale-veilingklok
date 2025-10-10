using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject_Klas1_Groep2.Models
{

    //Account Model, represents the Account table in the database
    [Table("account")]
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
