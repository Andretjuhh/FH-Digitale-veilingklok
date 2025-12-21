namespace Application.DTOs.Output;

public class KoperOutputDto
{
    public Domain.Enums.AccountType AccountType { get; init; } = Domain.Enums.AccountType.Koper;
    public required string Email { get; set; } = string.Empty;
    public required string FirstName { get; set; } = string.Empty;
    public required string LastName { get; set; } = string.Empty;
    public required string Telephone { get; set; } = string.Empty;

    public required int PrimaryAddressId { get; set; }
    public required List<AddressOutputDto> Addresses { get; set; } = new();
}