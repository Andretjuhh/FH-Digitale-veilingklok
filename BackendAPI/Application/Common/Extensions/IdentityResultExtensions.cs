using Application.Common.Exceptions;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Application.Common.Extensions;

public static class IdentityResultExtensions
{
    public static void ThrowIfFailed(this IdentityResult result)
    {
        if (result.Succeeded)
            return;

        var errors = result.Errors.ToList();

        // 1. Check for duplicates (highest priority as it's a conflict)
        if (errors.Any(e => e.Code == "DuplicateUserName" || e.Code == "DuplicateEmail"))
            throw RepositoryException.ExistingAccount();

        // 2. Check for specific password errors
        if (errors.Any(e => e.Code == "PasswordTooShort"))
            throw AccountValidationException.PasswordTooShort();

        if (errors.Any(e => e.Code == "PasswordRequiresDigit"))
            throw AccountValidationException.PasswordMissingDigit();

        if (errors.Any(e => e.Code == "PasswordRequiresUpper"))
            throw AccountValidationException.PasswordMissingUppercase();

        // Catch-all for other password policy errors (e.g., Lower, NonAlphanumeric, UniqueChars)
        if (errors.Any(e => e.Code.StartsWith("Password")))
            throw AccountValidationException.PasswordTooWeak();

        // 3. Email errors
        if (errors.Any(e => e.Code == "InvalidEmail"))
            throw AccountValidationException.EmailInvalid();

        // 4. Default fallback: concatenate all error descriptions
        throw RepositoryException.AccountCreationFailed(
            string.Join(", ", errors.Select(e => e.Description))
        );
    }
}
