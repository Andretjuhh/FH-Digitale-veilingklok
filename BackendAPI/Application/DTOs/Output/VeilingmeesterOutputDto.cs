namespace Application.DTOs.Output;

public class VeilingmeesterOutputDto
{
    public Domain.Enums.AccountType AccountType { get; init; } = Domain.Enums.AccountType.Veilingmeester;
    public required string Email { get; set; } = string.Empty;
    public required string CountryCode { get; set; } = string.Empty;
    public required string Region { get; set; } = string.Empty;
}