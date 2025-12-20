using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeilingKlokApp.Models.Domain
{
    // Kweker Model, represents the Kweker table in the database
    [Table("Kweker")]
    public class Kweker : Account
    {
        [Column("kvk_nmr")]
        [Required, MaxLength(50)]
        public required string KvkNumber { get; set; }

        [Column("company_name")]
        [Required, MaxLength(100)]
        public required string CompanyName { get; set; }

        [Column("first_name")]
        [MaxLength(60)]
        public string? FirstName { get; set; }

        [Column("last_name")]
        [MaxLength(60)]
        public string? LastName { get; set; }

        [Column("telephone")]
        [Phone]
        [Required, MaxLength(20)]
        public required string Telephone { get; set; }

        [Column("adress")]
        [Required, MaxLength(100)]
        public required string Adress { get; set; }

        [Column("post_code")]
        [Required, MaxLength(8)]
        public required string PostCode { get; set; }

        // Navigation property for the one-to-many relationship with Product
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
