using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeilingKlokApp.Models.Domain
{
    [Table("Order")]
    public class Order
    {
        [Key]
        [Required, Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("bought_at")]
        [Required]
        public DateTime BoughtAt { get; set; }

        // --- Koper Relationship ---
        [Column("koper_id")]
        [Required]
        public Guid KoperId { get; set; }

        // Navigation property to the Koper (Buyer) object
        public Koper Koper { get; set; } = default!;

        // --- OrderItems Relationship (many products per order) ---
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
