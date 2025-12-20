using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeilingKlokApp.Models.Domain
{
    // Koper Model, represents the Koper table in the database
    [Table("Koper")]
    public class Koper : Account
    {
        [Column("first_name")]
        [Required, MaxLength(60)]
        public required string FirstName { get; set; }

        [Column("last_name")]
        [Required, MaxLength(60)]
        public required string LastName { get; set; }

        [Column("telephone")]
        [Phone]
        [Required, MaxLength(20)]
        public required string Telephone { get; set; }

        [Column("adress")]
        [Required, MaxLength(100)]
        public required string Adress { get; set; }

        [Column("post_code")]
        [Required, MaxLength(10)]
        public required string PostCode { get; set; }

        // Navigation property for the one-to-many relationship with Order
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
