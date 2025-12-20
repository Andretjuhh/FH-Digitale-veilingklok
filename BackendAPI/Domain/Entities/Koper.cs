using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities
{
    // Koper Model, represents the Koper table in the database
    [Table("Koper")]
    public class Koper : Account
    {
        [Column("first_name")]
        [Required, MaxLength(60)]
        public required string FirstName { get; set; }

        [Column("last_name")]
        [Required, MaxLength(60)]
        public required string LastName { get; set; }

        [Column("telephone")]
        [Required, MaxLength(20)]
        public required string Telephone { get; set; }

        // Koper Primary Adress  One-to-One Relationship
        [Column("primary_adress_id")]
        public int PrimaryAdressId { get; private set; }

        // Adress Relationshiop  User have many Delivery Adresses
        private readonly List<Address> IAdresses = new List<Address>();
        public IReadOnlyCollection<Address> Adresses => IAdresses;

        [NotMapped]
        public override AccountType AccountType => AccountType.Koper;

        private Koper()
            : base() { }

        public Koper(string email, Password password)
            : base(email, password) { }

        public void AddNewAdress(Address newAdress, bool makePrimary = false)
        {
            // User can have max 5 adresses
            if (IAdresses.Count >= 5)
                throw AccountValidationException.AdressMaximum();

            IAdresses.Add(newAdress);
            if (makePrimary)
                this.SetPrimaryAdress(newAdress);
        }

        public void UpdateAdress(Address updatedAdress)
        {
            var existingAdress =
                IAdresses.FirstOrDefault(a => a.Id == updatedAdress.Id)
                ?? throw AccountValidationException.InvalidAdress();

            // Update properties of the existing adress
            existingAdress.UpdateAddress(
                updatedAdress.Street,
                updatedAdress.City,
                updatedAdress.RegionOrState,
                updatedAdress.PostalCode,
                updatedAdress.Country
            );
        }

        public void RemoveAdress(Address adress)
        {
            if (!IAdresses.Contains(adress))
                throw AccountValidationException.InvalidAdress();
            if (PrimaryAdressId == adress.Id)
                throw AccountValidationException.CannotRemovePrimaryAdress();
            IAdresses.Remove(adress);
        }

        public void SetPrimaryAdress(Address adress)
        {
            if (!IAdresses.Contains(adress))
                throw AccountValidationException.InvalidAdress();
            PrimaryAdressId = adress.Id;
        }
    }
}
