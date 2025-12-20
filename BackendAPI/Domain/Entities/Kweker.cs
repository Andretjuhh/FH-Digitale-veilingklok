using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities
{
    // Kweker Model, represents the Kweker table in the database
    [Table("Kweker")]
    public class Kweker : Account
    {
        [Column("kvk_nmr")]
        [Required, MaxLength(50)]
        public required string KvkNumber { get; init; }

        [Column("company_name")]
        [Required, MaxLength(100)]
        public required string CompanyName { get; set; }

        [Column("first_name")]
        [Required, MaxLength(60)]
        public required string FirstName { get; set; }

        [Column("last_name")]
        [Required, MaxLength(60)]
        public required string LastName { get; set; }

        [Column("telephone")]
        [Required, MaxLength(20)]
        public required string Telephone { get; set; }

        // Adress Relationshiop Company have one  Adresses
        [Column("adress_id")]
        [Required]
        public int AdressId { get; init; }
        public Address Adress { get; init; } = default!;

        [NotMapped]
        public override AccountType AccountType => AccountType.Kweker;

        private Kweker()
            : base() { }

        public Kweker(string email, Password password)
            : base(email, password) { }

        public void UpdateAdress(Address updatedAdress)
        {
            if (Adress.Id != updatedAdress.Id)
                throw AccountValidationException.InvalidAdress();
            Adress.UpdateAddress(
                updatedAdress.Street,
                updatedAdress.City,
                updatedAdress.RegionOrState,
                updatedAdress.PostalCode,
                updatedAdress.Country
            );
        }
    }
}
