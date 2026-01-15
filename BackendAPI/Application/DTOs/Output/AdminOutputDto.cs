using Domain.Enums;

namespace Application.DTOs.Output;

public class AdminOutputDto
{
    public AccountType AccountType { get; init; } = AccountType.Admin;
    public required string Email { get; set; } = string.Empty;
}
