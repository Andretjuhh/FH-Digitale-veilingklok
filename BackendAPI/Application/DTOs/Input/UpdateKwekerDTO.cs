namespace Application.DTOs.Input;

public class UpdateKwekerDTO
{
    public string? CompanyName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Telephone { get; set; }
    public string? KvkNumber { get; set; }

    // Address fields
    public AddressInputDto? Address { get; set; }
}