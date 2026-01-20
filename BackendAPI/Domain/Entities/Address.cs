using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("Adresses")]
public class Address
{
    [Key] [Column("id")] public int Id { get; init; }

    [Column("street")]
    [Required]
    [MaxLength(255)]
    public string Street { get; private set; }

    [Column("city")]
    [Required]
    [MaxLength(50)]
    public string City { get; private set; }

    [Column("state_or_province")]
    [Required]
    [MaxLength(50)]
    public string RegionOrState { get; private set; }

    [Column("postal_code")]
    [Required]
    [MaxLength(10)]
    public string PostalCode { get; private set; }

    [Column("country")]
    [Required]
    [MaxLength(2)]
    public string Country { get; private set; }

    [Column("account_id")] [Required] public Guid AccountId { get; private set; }

#nullable disable
    // Parameterless constructor for EF Core
    private Address()
    {
    }

#nullable restore

    public Address(
        string street,
        string city,
        string regionOrState,
        string postalCode,
        string country
    )
    {
        Street = street;
        City = city;
        RegionOrState = regionOrState;
        PostalCode = postalCode;
        Country = country;
    }

    public Address(
        string street,
        string city,
        string regionOrState,
        string postalCode,
        string country,
        Guid accountId
    )
    {
        Street = street;
        City = city;
        RegionOrState = regionOrState;
        PostalCode = postalCode;
        Country = country;
        AccountId = accountId;
    }

    public void UpdateAddress(
        string street,
        string city,
        string regionOrState,
        string postalCode,
        string country
    )
    {
        Street = street;
        City = city;
        RegionOrState = regionOrState;
        PostalCode = postalCode;
        Country = country;
    }
}