// Models/Domain/Product.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeilingKlokApp.Models.Domain
{
    [Table("Product")]
    public class Product
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("name")]
        [Required]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        [Required]
        public required string Description { get; set; }

        [Column("price")]
        [Required, Range(0.00, double.MaxValue)]
        public decimal Price { get; set; }

        [Column("minimum_price")]
        [Required, Range(0.00, double.MaxValue)]
        public decimal MinimumPrice { get; set; }

        [Column("stock")]
        [Required, Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Column("image_base64")]
        public string? ImageBase64 { get; set; }

        [Column("dimension")]
        public string? Dimension { get; set; }

        [Column("in_auction")]
        public bool InAuction { get; set; } = false;

        // --- Kweker Relationship ---
        [Column("kweker_id")]
        [Required]
        public Guid KwekerId { get; set; } // FK to Kweker
        public Kweker Kweker { get; set; } = default!; // Navigation property to Kweker

        // --- Veilingklok Relationship (optional) ---
        [Column("veilingklok_id")]
        public Guid? VeilingKlokId { get; set; } // FK to VeilingKlok (nullable)
        public VeilingKlok? VeilingKlok { get; set; } // Navigation property (optional)

        // --- OrderItems Relationship (reverse navigation) ---
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
