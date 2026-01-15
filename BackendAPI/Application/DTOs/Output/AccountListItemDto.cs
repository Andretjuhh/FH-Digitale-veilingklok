using Domain.Enums;

namespace Application.DTOs.Output;

public class AccountListItemDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public bool IsDeleted => DeletedAt.HasValue;
}
