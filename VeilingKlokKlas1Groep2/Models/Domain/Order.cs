using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeilingKlokApp.Models.Domain
{
    //TODO: Once we have products, uncomment the ProductId and Product navigation property
    [Table("Order")]
    public class Order
    {
        [Key]
        [Column("id")]
        public int Id { get; set; } // The PK

        [Column("quantity")]
        [Required]
        public int Quantity { get; set; }

        [Column("bought_at")]
        [Required]
        public DateTime BoughtAt { get; set; }


        // --- Buyer (Koper) Relationship ---
        [Column("koper_id")]
        [Required]
        // Foreign Key to the Koper (Buyer) table
        public int KoperId { get; set; }

        // Navigation property to the Koper (Buyer) object
        public Koper Koper { get; set; } = default!;

        // --- Seller (Kweker) Relationship ---
        [Column("kweker_id")]
        [Required]
        // Foreign Key to the Kweker (Seller) table
        public int KwekerId { get; set; }

        // Navigation property to the Kweker (Seller) object
        public Kweker Kweker { get; set; } = default!;

        // --- Product Relationship ---
        // [Column("product_id")]
        // [Required]
        // public int ProductId { get; set; } // FK to Product
        //public Product Product { get; set; } // Navigation property to Product
    }
}