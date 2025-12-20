using Domain.Exceptions;
using Domain.Interfaces;

namespace Domain.ValueObjects;

public sealed class Password
{
    private string HashedPassword { get; }

    private Password(string hash)
    {
        HashedPassword = hash;
    }

    public static Password FromHash(string hash)
    {
        return new Password(hash);
    }

    public static Password Create(string raw, IPasswordHasher hasher)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw AccountValidationException.PasswordEmpty();

        if (raw.Length < 8)
            throw AccountValidationException.PasswordTooShort();

        if (!raw.Any(char.IsUpper))
            throw AccountValidationException.PasswordMissingUppercase();

        if (!raw.Any(char.IsDigit))
            throw AccountValidationException.PasswordMissingDigit();

        return new Password(hasher.Hash(raw));
    }

    public bool Verify(string providedPassword, IPasswordHasher hasher)
    {
        return hasher.Verify(providedPassword, HashedPassword);
    }
}