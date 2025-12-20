using Domain.Enums;

namespace Application.DTOs.Output;

public class AuthOutputDto
{
    public required string AccessToken { get; set; } = string.Empty;
    public required DateTimeOffset AccessTokenExpiresAt { get; set; }
    public required AccountType AccountType { get; set; }
}
