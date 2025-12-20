using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VeilingKlokApp.Data;
using VeilingKlokApp.Declarations;
using VeilingKlokApp.Models.Domain;
using VeilingKlokApp.Models.OutputDTOs;
using VeilingKlokApp.Types;

namespace VeilingKlokApp.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> SignInAsync(Account account, HttpResponse response);
        Task<AuthResponse?> RefreshAsync(string refreshTokenString, HttpResponse response);
        Task LogoutAsync(string refreshTokenString, HttpResponse response);
    }

    public class AuthService : IAuthService
    {
        private readonly DatabaseContext _db;
        private readonly IJwtService _jwtService;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            DatabaseContext db,
            IJwtService jwtService,
            IOptions<JwtSettings> jwtSettings
        )
        {
            _db = db;
            _jwtService = jwtService;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthResponse> SignInAsync(Account account, HttpResponse response)
        {
            var accountType = await DetermineAccountType(account.Id);

            // Generate access and refresh tokens
            var accessToken = _jwtService.GenerateAccessToken(account, accountType);
            var refreshTokenString = _jwtService.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Id = 0,
                Token = refreshTokenString,
                AccountId = account.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            };

            _db.RefreshTokens.Add(refreshToken);
            await _db.SaveChangesAsync();

            SetRefreshTokenCookie(response, refreshTokenString, refreshToken.ExpiresAt);

            var authResponse = new AuthResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(
                    _jwtSettings.AccessTokenExpirationMinutes
                ),
                RefreshTokenExpiresAt = refreshToken.ExpiresAt,
                AccountId = account.Id,
                Email = account.Email,
                AccountType = accountType,
            };

            return authResponse;
        }

        public async Task<AuthResponse?> RefreshAsync(
            string refreshTokenString,
            HttpResponse response
        )
        {
            if (string.IsNullOrEmpty(refreshTokenString))
                return null;

            var refreshToken = await _db
                .RefreshTokens.Include(rt => rt.Account)
                .Where(rt => rt.Token == refreshTokenString)
                .FirstOrDefaultAsync();

            if (refreshToken == null || !refreshToken.IsActive)
            {
                return null;
            }

            // Determine account type
            var accountType = await DetermineAccountType(refreshToken.AccountId);

            // Generate new tokens (token rotation)
            var newAccessToken = _jwtService.GenerateAccessToken(refreshToken.Account, accountType);
            var newRefreshTokenString = _jwtService.GenerateRefreshToken();

            // Revoke old token and create new one
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.ReplacedByToken = newRefreshTokenString;

            var newRefreshToken = new RefreshToken
            {
                Id = 0,
                Token = newRefreshTokenString,
                AccountId = refreshToken.AccountId,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            };

            _db.RefreshTokens.Add(newRefreshToken);
            await _db.SaveChangesAsync();

            SetRefreshTokenCookie(response, newRefreshTokenString, newRefreshToken.ExpiresAt);

            var responseDto = new AuthResponse
            {
                AccessToken = newAccessToken,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(
                    _jwtSettings.AccessTokenExpirationMinutes
                ),
                RefreshTokenExpiresAt = newRefreshToken.ExpiresAt,
                AccountId = refreshToken.AccountId,
                Email = refreshToken.Account.Email,
                AccountType = accountType,
            };

            return responseDto;
        }

        public async Task LogoutAsync(string refreshTokenString, HttpResponse response)
        {
            if (!string.IsNullOrEmpty(refreshTokenString))
            {
                var refreshToken = await _db
                    .RefreshTokens.Where(rt => rt.Token == refreshTokenString)
                    .FirstOrDefaultAsync();

                if (refreshToken != null && refreshToken.IsActive)
                {
                    refreshToken.RevokedAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                }
            }

            response.Cookies.Delete("refreshToken");
        }

        private void SetRefreshTokenCookie(
            HttpResponse response,
            string refreshToken,
            DateTime expiresAt
        )
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expiresAt,
                Path = "/",
                IsEssential = true,
            };

            response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        private async Task<AccountType> DetermineAccountType(Guid accountId)
        {
            if (await _db.Kopers.AnyAsync(k => k.Id == accountId))
                return AccountType.Koper;
            if (await _db.Kwekers.AnyAsync(k => k.Id == accountId))
                return AccountType.Kweker;
            if (await _db.Veilingmeesters.AnyAsync(v => v.Id == accountId))
                return AccountType.Veilingmeester;
            throw new Exception("Account type could not be determined");
        }
    }
}
