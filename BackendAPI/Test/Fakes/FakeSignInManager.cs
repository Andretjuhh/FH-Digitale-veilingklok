using System.Security.Claims;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Test.Fakes;

public class FakeSignInManager : SignInManager<Account>
{
    public FakeSignInManager()
        : base(
            new FakeUserManager(),
            new FakeHttpContextAccessor(),
            new FakeUserClaimsPrincipalFactory(),
            Microsoft.Extensions.Options.Options.Create(new IdentityOptions()),
            new FakeLogger<SignInManager<Account>>(),
            new FakeAuthenticationSchemeProvider(),
            new FakeUserConfirmation()
        ) { }

    public override Task SignInAsync(
        Account user,
        bool isPersistent,
        string? authenticationMethod = null
    )
    {
        // Mock implementation - do nothing
        return Task.CompletedTask;
    }

    public override Task SignOutAsync()
    {
        // Mock implementation - do nothing
        return Task.CompletedTask;
    }
}

public class FakeUserManager : UserManager<Account>
{
    public FakeUserManager()
        : base(
            new FakeUserStore(),
            Microsoft.Extensions.Options.Options.Create(new IdentityOptions()),
            new FakePasswordHasher(),
            Array.Empty<IUserValidator<Account>>(),
            Array.Empty<IPasswordValidator<Account>>(),
            new FakeLookupNormalizer(),
            new IdentityErrorDescriber(),
            new FakeServiceProvider(),
            new FakeLogger<UserManager<Account>>()
        ) { }

    public override Task<Account?> GetUserAsync(ClaimsPrincipal principal)
    {
        // Extract user ID and role from claims
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roleClaim = principal.FindFirst(ClaimTypes.Role)?.Value;

        if (
            string.IsNullOrEmpty(userIdClaim)
            || !Guid.TryParse(userIdClaim, out var userId)
            || string.IsNullOrEmpty(roleClaim)
            || !Enum.TryParse<AccountType>(roleClaim, out var accountType)
        )
        {
            return Task.FromResult<Account?>(null);
        }

        // Create a fake account based on the role
        Account account = accountType switch
        {
            AccountType.Koper => new global::Domain.Entities.Koper("fake@example.com")
            {
                Id = userId,
                FirstName = "Test",
                LastName = "User",
                Telephone = "123456789",
            },
            AccountType.Kweker => new global::Domain.Entities.Kweker("fake@example.com")
            {
                Id = userId,
                KvkNumber = "12345678",
                CompanyName = "Test Company",
                FirstName = "Test",
                LastName = "User",
                Telephone = "123456789",
            },
            AccountType.Veilingmeester => new global::Domain.Entities.Veilingmeester(
                "fake@example.com"
            )
            {
                Id = userId,
                CountryCode = "NL",
                Region = "Test",
                AuthorisatieCode = "AUTH123",
            },
            _ => null!,
        };

        return Task.FromResult<Account?>(account);
    }

    public override Task<IdentityResult> UpdateSecurityStampAsync(Account user)
    {
        // Mock implementation
        return Task.FromResult(IdentityResult.Success);
    }
}

// Supporting fake classes
public class FakeUserStore : IUserStore<Account>
{
    public void Dispose() { }

    public Task<string> GetUserIdAsync(Account user, CancellationToken cancellationToken) =>
        Task.FromResult(user.Id.ToString());

    public Task<string?> GetUserNameAsync(Account user, CancellationToken cancellationToken) =>
        Task.FromResult<string?>(user.Email);

    public Task SetUserNameAsync(
        Account user,
        string? userName,
        CancellationToken cancellationToken
    ) => Task.CompletedTask;

    public Task<string?> GetNormalizedUserNameAsync(
        Account user,
        CancellationToken cancellationToken
    ) => Task.FromResult<string?>(user.Email);

    public Task SetNormalizedUserNameAsync(
        Account user,
        string? normalizedName,
        CancellationToken cancellationToken
    ) => Task.CompletedTask;

    public Task<IdentityResult> CreateAsync(Account user, CancellationToken cancellationToken) =>
        Task.FromResult(IdentityResult.Success);

    public Task<IdentityResult> UpdateAsync(Account user, CancellationToken cancellationToken) =>
        Task.FromResult(IdentityResult.Success);

    public Task<IdentityResult> DeleteAsync(Account user, CancellationToken cancellationToken) =>
        Task.FromResult(IdentityResult.Success);

    public Task<Account?> FindByIdAsync(string userId, CancellationToken cancellationToken) =>
        Task.FromResult<Account?>(null);

    public Task<Account?> FindByNameAsync(
        string normalizedUserName,
        CancellationToken cancellationToken
    ) => Task.FromResult<Account?>(null);
}

public class FakeHttpContextAccessor : IHttpContextAccessor
{
    public HttpContext? HttpContext { get; set; }
}

public class FakeUserClaimsPrincipalFactory : IUserClaimsPrincipalFactory<Account>
{
    public Task<ClaimsPrincipal> CreateAsync(Account user) =>
        Task.FromResult(new ClaimsPrincipal());
}

public class FakeLogger<T> : ILogger<T>
{
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => false;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    ) { }
}

public class FakeAuthenticationSchemeProvider : IAuthenticationSchemeProvider
{
    public Task<IEnumerable<AuthenticationScheme>> GetAllSchemesAsync() =>
        Task.FromResult(Enumerable.Empty<AuthenticationScheme>());

    public Task<AuthenticationScheme?> GetDefaultAuthenticateSchemeAsync() =>
        Task.FromResult<AuthenticationScheme?>(null);

    public Task<AuthenticationScheme?> GetDefaultChallengeSchemeAsync() =>
        Task.FromResult<AuthenticationScheme?>(null);

    public Task<AuthenticationScheme?> GetDefaultForbidSchemeAsync() =>
        Task.FromResult<AuthenticationScheme?>(null);

    public Task<AuthenticationScheme?> GetDefaultSignInSchemeAsync() =>
        Task.FromResult<AuthenticationScheme?>(null);

    public Task<AuthenticationScheme?> GetDefaultSignOutSchemeAsync() =>
        Task.FromResult<AuthenticationScheme?>(null);

    public Task<IEnumerable<AuthenticationScheme>> GetRequestHandlerSchemesAsync() =>
        Task.FromResult(Enumerable.Empty<AuthenticationScheme>());

    public Task<AuthenticationScheme?> GetSchemeAsync(string name) =>
        Task.FromResult<AuthenticationScheme?>(null);

    public void AddScheme(AuthenticationScheme scheme) { }

    public bool TryAddScheme(AuthenticationScheme scheme) => true;

    public void RemoveScheme(string name) { }
}

public class FakeUserConfirmation : IUserConfirmation<Account>
{
    public Task<bool> IsConfirmedAsync(UserManager<Account> manager, Account user) =>
        Task.FromResult(true);
}

public class FakePasswordHasher : IPasswordHasher<Account>
{
    public string HashPassword(Account user, string password) => password;

    public PasswordVerificationResult VerifyHashedPassword(
        Account user,
        string hashedPassword,
        string providedPassword
    ) => PasswordVerificationResult.Success;
}

public class FakeLookupNormalizer : ILookupNormalizer
{
    public string NormalizeName(string? name) => name ?? "";

    public string NormalizeEmail(string? email) => email ?? "";
}

public class FakeServiceProvider : IServiceProvider
{
    public object? GetService(Type serviceType) => null;
}
