using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject_Klas1_Groep2.Models
{
    public class Kweker
    {
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