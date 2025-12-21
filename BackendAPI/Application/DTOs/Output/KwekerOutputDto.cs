namespace Application.DTOs.Output;

public class KwekerOutputDto
{
    public Domain.Enums.AccountType AccountType { get; init; } = Domain.Enums.AccountType.Kweker;
    public required string Email { get; set; }
    public required string KvkNumber { get; set; }
    public required string CompanyName { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Telephone { get; set; }
    public required AddressOutputDto Address { get; set; }
}