using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeilingKlok.Models.Domain
{
    // Kweker Model, represents the Kweker table in the database
    public class Kweker
    {
        //Since this is a specialization of Account, AccountId is both PK and FK
        [Column("account_id")]
        public int AccountId { get; set; }

        [Column("name")]
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Column("telephone")]
        [Required, MaxLength(20)]
        public string Telephone { get; set; }

        [Column("adress")]
        [MaxLength(255)]
        public string? Adress { get; set; }

        [Column("regio")]
        [MaxLength(100)]
        public string? Regio { get; set; }

        [Column("kvk_nmr")]
        [MaxLength(50)]
        public string? KvkNumber { get; set; }

        public Account Account { get; set; }
    }
}