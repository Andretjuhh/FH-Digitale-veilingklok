using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("RefreshToken")]
public class RefreshToken
{
    [Key]
    [Column("token_id")]
    public required string Token { get; init; }

    [Column("jti")]
    [Required]
    public required Guid Jti { get; init; }

    [Column("account_id")]
    [Required]
    public required Guid AccountId { get; init; }

    [Column("expires_at")]
    [Required]
    public required DateTimeOffset ExpiresAt { get; init; }

    [Column("created_at")]
    [Required]
    public DateTimeOffset CreatedAt { get; init; }

    [Column("revoked_at")]
    public DateTimeOffset? RevokedAt { get; set; }

    [NotMapped]
    public bool IsActive => RevokedAt == null && DateTimeOffset.UtcNow < ExpiresAt;

    [NotMapped]
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
}
