using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VeilingKlokKlas1Groep2.Models.Domain;
namespace VeilingKlokKlas1Groep2
{
    public class Veilingmeester
    {
        //Since this is a specialization of Account, AccountId is both PK and FK
        [Column("account_id")]
        public int AccountId { get; set; }

        [Column("soort_veiling")]
        public string? SoortVeiling { get; set; }

        public Account Account { get; set; }
    }
}