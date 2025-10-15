using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VeilingKlokApp.Models.Domain;

namespace VeilingKlokApp.Models.Domain
{
    [Table("Veilingmeester")]
    public class Veilingmeester : Account
    {
        [Column("soort_veiling")]
        [Required, MaxLength(20)]
        public string SoortVeiling { get; set; }

        // [Column("regio")]
        // [MaxLength(100)]
        // public string Regio { get; set; }

        // Navigation property for the Veilingklokken managed by this Veilingmeester
        public ICollection<VeilingKlok> Veilingklokken { get; set; } = new List<VeilingKlok>();
    }
}
