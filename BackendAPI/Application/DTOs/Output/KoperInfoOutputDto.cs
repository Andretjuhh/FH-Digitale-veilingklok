namespace Application.DTOs.Output;

public class KoperInfoOutputDto
{
    public required string Email { get; set; } = string.Empty;
    public required string FirstName { get; set; } = string.Empty;
    public required string LastName { get; set; } = string.Empty;
    public required string Telephone { get; set; } = string.Empty;
    public required AddressOutputDto Address { get; set; }
}