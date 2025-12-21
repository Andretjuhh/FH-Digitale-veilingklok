using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.DTOs.Output;
using Application.Services;
using Domain.Entities;
using Infrastructure.MicroServices.Security.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.MicroServices.Security;

public class TokenService : ITokenService
{
    private readonly JwtConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenService(
        IOptions<JwtConfiguration> configuration,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _configuration = configuration.Value;
        _httpContextAccessor = httpContextAccessor;

        // If a any property is missing, throw an exception
        if (
            string.IsNullOrEmpty(_configuration.Secret)
            || string.IsNullOrEmpty(_configuration.Issuer)
            || string.IsNullOrEmpty(_configuration.Audience)
            || _configuration.AccessTokenExpirationMinutes <= 0
            || _configuration.RefreshTokenExpirationDays <= 0
        )
            throw new ArgumentException("Invalid JWT configuration. Please check the settings.");
    }

    public Guid GetUserTokenJti()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            throw new InvalidOperationException("No HTTP context available.");

        var user = httpContext.User;
        var jtiClaim = user.FindFirst(JwtRegisteredClaimNames.Jti);
        if (jtiClaim == null || !Guid.TryParse(jtiClaim.Value, out var jti))
            throw new InvalidOperationException("JTI claim is missing or invalid.");

        return jti;
    }

    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration.Secret);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration.Issuer,
                ValidateAudience = true,
                ValidAudience = _configuration.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero, // No tolerance for expiration
            };

            var principal = tokenHandler.ValidateToken(
                token,
                validationParameters,
                out var validatedToken
            );

            // Ensure the token is a JWT and uses the correct algorithm
            if (
                validatedToken is JwtSecurityToken jwtToken
                && jwtToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase
                )
            )
                return principal;

            return null;
        }
        catch
        {
            return null;
        }
    }

    public (string token, Guid jti, DateTimeOffset expireAt) GenerateAccessToken(Account account)
    {
        var key = Encoding.ASCII.GetBytes(_configuration.Secret);
        var expires = DateTimeOffset.UtcNow.AddMinutes(_configuration.AccessTokenExpirationMinutes);

        // Create claims for the token
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new(ClaimTypes.Email, account.Email),
            new(ClaimTypes.Role, account.AccountType.ToString()), // Custom claim for account type
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Token ID
        };

        // Extract the JTI claim value
        var jti = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

        if (jti == null)
            throw new InvalidOperationException("JTI claim could not be created.");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires.UtcDateTime, // Use UtcDateTime to avoid timezone issues
            NotBefore = DateTime.UtcNow, // Explicitly set NotBefore to current UTC time
            Issuer = _configuration.Issuer,
            Audience = _configuration.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
        };

        // Token creation
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), Guid.Parse(jti), expires);
    }

    public RefreshToken GenerateRefreshToken(Guid accountId, Guid jti)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var token = Convert.ToBase64String(randomBytes);

        return new RefreshToken
        {
            Token = token,
            Jti = jti,
            AccountId = accountId,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(_configuration.RefreshTokenExpirationDays),
        };
    }

    public (AuthOutputDto auth, RefreshToken refreshToken) GenerateAuthenticationTokens(
        Account account
    )
    {
        var (accessToken, jti, expires) = GenerateAccessToken(account);
        var refreshToken = GenerateRefreshToken(account.Id, jti);
        account.AddRefreshToken(refreshToken);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, // Crucial for security: prevents client-side JavaScript access
            Secure = _httpContextAccessor.HttpContext?.Request.IsHttps ?? true, // Only send over HTTPS in production
            IsEssential = true, // Necessary for the site to function
            SameSite = SameSiteMode.Strict, // Prevent CSRF attacks
            Expires = refreshToken.ExpiresAt, // Set the cookie expiry to match the refresh token expiry
        };

        SetCookie("refreshToken", refreshToken.Token, cookieOptions); // Assuming RefreshToken entity has a 'Token' property

        return (
            new AuthOutputDto
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = expires,
                AccountType = account.AccountType,
            },
            refreshToken
        );
    }

    public string? GetContextRefreshToken()
    {
        return GetCookie("refreshToken");
    }

    public void ClearCookies()
    {
        DeleteCookie("refreshToken");
    }

    private string? GetCookie(string key)
    {
        // Returns null if HttpContext is unavailable or cookie not found
        return _httpContextAccessor.HttpContext?.Request.Cookies[key];
    }

    private void SetCookie(string key, string value, CookieOptions options)
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(key, value, options);
    }

    private void DeleteCookie(string key)
    {
        // To delete, set the cookie with an immediate past expiration date.
        var options = new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(-1),
            HttpOnly = true,
        };
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(key, "", options);
    }
}
