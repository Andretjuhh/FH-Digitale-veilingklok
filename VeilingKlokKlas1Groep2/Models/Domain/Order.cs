using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeilingKlokKlas1Groep2.Models.Domain
{
    public class Order
    {
        [Key]
        [Column("id")]
        public int Id { get; set; } // The PK

        [Column("koper_id")]
        [Required]
        public int KoperId { get; set; } // FK to Koper

        [Column("quantity")]
        [Required]
        public int Quantity { get; set; }

        [Column("bought_at")]
        [Required]
        public DateTime BoughtAt { get; set; }

        public Koper Koper { get; set; } // Navigation property to Koper
    }
}