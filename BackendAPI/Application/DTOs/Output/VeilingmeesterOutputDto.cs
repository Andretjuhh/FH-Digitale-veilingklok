namespace Application.DTOs.Output;

public class VeilingmeesterOutputDto
{
    public required string Email { get; set; } = string.Empty;
    public required string CountryCode { get; set; } = string.Empty;
    public required string Region { get; set; } = string.Empty;
}