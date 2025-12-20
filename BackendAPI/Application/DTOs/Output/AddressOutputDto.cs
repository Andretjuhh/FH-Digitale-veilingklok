namespace Application.DTOs.Output;

public class AddressOutputDto
{
    public required int Id { get; set; }
    public required string Street { get; set; }
    public required string City { get; set; }
    public required string RegionOrState { get; set; }
    public required string PostalCode { get; set; }
    public required string Country { get; set; }
}