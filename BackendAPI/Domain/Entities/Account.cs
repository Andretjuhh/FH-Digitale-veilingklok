using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Domain.Interfaces;
using Domain.ValueObjects;

namespace Domain.Entities;

//Account Model, represents the Account table in the database
// Its abstract because we never create an Account directly it is always a Koper or Kweker
[Table("Account")]
public abstract class Account
{
    [Key]
    [Column("id")]
    public Guid Id { get; init; } = Guid.Empty;

    [Column("email")]
    [EmailAddress]
    [Required]
    [MaxLength(255)]
    public string Email { get; private set; }

    [Column("password")]
    [Required]
    [MaxLength(255)]
    public Password Password { get; private set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    [Column("deleted_at")]
    public DateTimeOffset? DeletedAt { get; private set; }

    [Column("row_version")]
    [Timestamp]
    public ulong RowVersion { get; private set; }

    [NotMapped]
    public abstract AccountType AccountType { get; }

    // RefreshTokens Relationshiop  User have many RefreshTokens
    private readonly List<RefreshToken> IRTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => IRTokens;

#nullable disable
    protected Account() { }

#nullable restore

    protected Account(string email, Password password)
    {
        Email = email;
        Password = password;
    }

    public void ChangeEmail(string newEmail)
    {
        Email = newEmail;
    }

    public void ChangePassword(string raw, IPasswordHasher hasher)
    {
        Password = Password.Create(raw, hasher);
    }

    public void AddRefreshToken(RefreshToken token)
    {
        IRTokens.Add(token);
    }

    public void UpdateRefreshToken(RefreshToken oldToken, RefreshToken newToken)
    {
        // Replace old refresh token with new one
        IRTokens.Remove(oldToken);
        IRTokens.Add(newToken);
    }

    public void RemoveRefreshToken(string token)
    {
        var existingToken = IRTokens.FirstOrDefault(rt => rt.Token == token);
        if (existingToken != null)
            IRTokens.Remove(existingToken);
    }

    public void RevokeAllRefreshTokens()
    {
        // Delete all existing refresh tokens
        IRTokens.Clear();
    }

    public void SoftDelete()
    {
        DeletedAt = DateTimeOffset.UtcNow;
    }
}
