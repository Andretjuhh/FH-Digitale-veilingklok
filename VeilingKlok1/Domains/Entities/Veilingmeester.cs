using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeilingKlokApp.Models.Domain
{
    [Table("Veilingmeester")]
    public class Veilingmeester : Account
    {
        [Column("authorisatie_code")]
        [Required, MaxLength(20)]
        public required string AuthorisatieCode { get; set; }

        // Navigation property for the Veilingklokken managed by this Veilingmeester
        public ICollection<VeilingKlok> Veilingklokken { get; set; } = new List<VeilingKlok>();
    }
}
