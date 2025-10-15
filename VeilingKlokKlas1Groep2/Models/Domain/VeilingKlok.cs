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

        [Column("duration_in_seconds")]
        [Required]
        public int DurationInSeconds { get; set; }

        [Column("live_views")]
        public int LiveViews { get; set; } = 0;

        [Column("start_time")]
        [Required]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        [Required]
        public DateTime EndTime { get; set; }

        // --- Veilingmeester Relationship ---
        [Column("veilingmeester_id")]
        [Required]
         public int VeilingmeesterId { get; set; }
        public Veilingmeester Veilingmeester { get; set; } = default!;

    }
}
