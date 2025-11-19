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
        public int Id { get; set; }

        [Column("name")]
        [Required]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("price")]
        [Required]
        public decimal Price { get; set; }
        
        [Column("minimum_price")]
        [Required]
        public decimal MinimumPrice { get; set; }
        
        [Column("quantity")]
        [Required]
        public int Quantity { get; set; }
        
        [Column("image_url")]
        public string? ImageUrl { get; set; }
        
        [Column("size")]
        public string? Size { get; set; }

        // --- Kweker Relationship ---
        [Column("kweker_id")]
        [Required]
        public int KwekerId { get; set; } // FK to Kweker
        public Kweker Kweker { get; set; } = default!; // Navigation property to Kweker

        // --- Veilingklok Relationship ---
        [Column("veilingklok_id")]
        [Required]
        public int VeilingKlokId { get; set; } // FK to VeilingKlok
        public VeilingKlok VeilingKlok { get; set; } = default!; // Navigation property
    }
}
