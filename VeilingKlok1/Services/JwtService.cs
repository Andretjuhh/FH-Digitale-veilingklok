using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VeilingKlokApp.Declarations;
using VeilingKlokApp.Models.Domain;
using VeilingKlokApp.Types;

namespace VeilingKlokApp.Services
{
    /// <summary>
    /// Service for JWT token generation and validation
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generates an access token for an authenticated user
        /// </summary>
        string GenerateAccessToken(Account account, AccountType accountType);

        /// <summary>
        /// Generates a refresh token
        /// </summary>
        string GenerateRefreshToken();

        /// <summary>
        /// Validates an access token and returns the principal
        /// </summary>
        ClaimsPrincipal? ValidateAccessToken(string token);

        /// <summary>
        /// Gets the account ID from a token's claims
        /// </summary>
        int? GetAccountIdFromToken(string token);

        /// <summary>
        /// Gets the account type from a token's claims
        /// </summary>
        AccountType? GetAccountTypeFromToken(string token);
    }

    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// Generates a JWT access token with user claims
        /// </summary>
        public string GenerateAccessToken(Account account, AccountType accountType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            // Create credentials
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            );

            // Create claims for the token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, accountType.ToString()), // Custom claim for account type
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Token ID
            };

            // Create the token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = credentials,
            };

            // Create the token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Generates a cryptographically secure refresh token
        /// </summary>
        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        /// <summary>
        /// Validates a JWT access token and returns the claims principal
        /// </summary>
        public ClaimsPrincipal? ValidateAccessToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
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
                {
                    return principal;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Extracts the account ID from a token's claims
        /// </summary>
        public int? GetAccountIdFromToken(string token)
        {
            var principal = ValidateAccessToken(token);
            var accountIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(accountIdClaim, out int accountId))
            {
                return accountId;
            }

            return null;
        }

        /// <summary>
        /// Extracts the account type from a token's claims
        /// </summary>
        public AccountType? GetAccountTypeFromToken(string token)
        {
            var principal = ValidateAccessToken(token);
            var accountTypeString = principal?.FindFirst(ClaimTypes.Role)?.Value;

            if (Enum.TryParse<AccountType>(accountTypeString, out var accountType))
            {
                return accountType;
            }

            return null;
        }
    }
}
