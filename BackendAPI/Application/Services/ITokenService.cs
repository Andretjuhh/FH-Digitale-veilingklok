using System.Security.Claims;
using Application.DTOs.Output;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public interface ITokenService
{
    /// <summary>
    /// Generates an access token for an authenticated user
    /// </summary>
    public (string token, Guid jti, DateTimeOffset expireAt) GenerateAccessToken(Account account);

    /// <summary>
    /// Generates a refresh token
    /// </summary>
    public RefreshToken GenerateRefreshToken(Guid accountId, Guid jti);

    /// <summary>
    /// Validates an access token and returns the principal
    /// </summary>
    public ClaimsPrincipal? ValidateAccessToken(string token);

    public void ClearCookies();

    public string? GetRefreshTokenFromCookies();

    public AuthOutputDto GenerateAuthenticationTokens(Account account);
}
