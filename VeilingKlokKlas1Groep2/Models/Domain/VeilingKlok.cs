using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VeilingKlokApp.Models.Domain;

namespace VeilingKlokApp.Models.Domain
{
    [Table("veilingklokken")]
    public class VeilingKlok
    {
        [Key]
        [Column("id")]
        public int Id { get; set; } // The PK

        [Column("naam")]
        [Required, MaxLength(100)]
        public string Naam { get; set; }

        // --- Seller (Veilingmeester) Relationship ---
        [Column("veilingmeester_id")]
        [Required]
         public int VeilingmeesterId { get; set; }
        public Veilingmeester Veilingmeester { get; set; } = default!;

    }
}
