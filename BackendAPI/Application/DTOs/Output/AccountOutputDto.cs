using Domain.Enums;

namespace Application.DTOs.Output;

public class AccountOutputDto
{
    public required AccountType AccountType { get; set; }
    public required string Email { get; set; }

    // Common fields
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Telephone { get; set; }

    // Kweker specific
    public string? CompanyName { get; set; }
    public string? KvkNumber { get; set; }

    // Veilingmeester specific
    public string? CountryCode { get; set; }
    public string? Region { get; set; }
    public string? AuthorisatieCode { get; set; }

    // Address (Primary for Koper, Main for Kweker)
    public AddressOutputDto? Address { get; set; }
}