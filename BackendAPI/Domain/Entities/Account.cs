using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

//Account Model, represents the Account table in the database
// Its abstract because we never create an Account directly it is always a Koper or Kweker
[Table("Account")]
public class Account : IdentityUser<Guid>
{
    [Column("created_at")] public DateTimeOffset CreatedAt { get; init; }

    [Column("deleted_at")] public DateTimeOffset? DeletedAt { get; private set; }

    [Column("row_version")] [Timestamp] public ulong RowVersion { get; private set; }

    [Column("account_type")] public AccountType AccountType { get; init; } = AccountType.Koper;

#nullable disable
    public Account()
    {
    }

    protected Account(AccountType accountType)
    {
        AccountType = accountType;
    }

#nullable restore

    public Account(string email, AccountType accountType)
    {
        Email = email;
        UserName = email;
        AccountType = accountType;
    }

    public void SoftDelete()
    {
        DeletedAt = DateTimeOffset.UtcNow;
    }
}