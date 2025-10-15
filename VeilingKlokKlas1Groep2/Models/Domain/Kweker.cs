using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeilingKlokApp.Models.Domain
{
    // Kweker Model, represents the Kweker table in the database
    [Table("Kweker")]
    public class Kweker: Account
    {
        [Column("name")]
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Column("telephone")]
        [Phone]
        [Required, MaxLength(20)]
        public string Telephone { get; set; }

        [Column("adress")]
        [MaxLength(255)]
        public string? Adress { get; set; }

        // [Column("regio")]
        // [MaxLength(100)]
        // public string? Regio { get; set; }

        [Column("kvk_nmr")]
        [MaxLength(50)]
        public string? KvkNumber { get; set; }

    }
}