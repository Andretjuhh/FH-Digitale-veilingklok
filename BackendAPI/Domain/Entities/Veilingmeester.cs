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

        [NotMapped]
        public override AccountType AccountType => AccountType.Veilingmeester;

        private Veilingmeester()
            : base() { }

        public Veilingmeester(string email, Password password)
            : base(email, password) { }
    }
}
