using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities
{
    [Table("Veilingmeester")]
    public class Veilingmeester : Account
    {
        [Column("country_code")]
        [Required, MaxLength(2)]
        public required string CountryCode { get; set; }

        [Column("state_or_province")]
        [Required, MaxLength(50)]
        public required string Region { get; set; }

        [Column("authorisatie_code")]
        [Required, MaxLength(20)]
        public required string AuthorisatieCode { get; set; }

        private Veilingmeester()
            : base(AccountType.Veilingmeester) { }

        public Veilingmeester(string email)
            : base(email, AccountType.Veilingmeester) { }
    }
}
