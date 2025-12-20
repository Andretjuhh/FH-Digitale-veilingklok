using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VeilingKlokApp.Attributes;
using VeilingKlokApp.Data;
using VeilingKlokApp.Declarations;
using VeilingKlokApp.Mappers;
using VeilingKlokApp.Models.InputDTOs;
using VeilingKlokApp.Models.OutputDTOs;
using VeilingKlokApp.Services;
using VeilingKlokApp.Types;
using VeilingKlokApp.Utils;
using VeilingKlokKlas1Groep2.Declarations;
using VeilingKlokKlas1Groep2.Models.OutputDTOs;

namespace VeilingKlokApp.Controllers
{
    /// <summary>
    /// Controller for authentication operations
    /// Handles login, token refresh, and logout
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly DatabaseContext db;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly JwtSettings _jwtSettings;
        private readonly IAuthService _authService;

        public AuthController(
            DatabaseContext db,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IOptions<JwtSettings> jwtSettings,
            IAuthService authService
        )
        {
            this.db = db;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _jwtSettings = jwtSettings.Value;
            _authService = authService;
        }

        #region Login

        /// <summary>
        /// Authenticates a user and returns access and refresh tokens
        /// </summary>
        /// <param name="loginRequest">User credentials</param>
        /// <returns>JWT tokens and user information</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            // Validate input
            var validationError = ControllerHelper.ValidateModelState(ModelState);
            if (validationError != null)
                return validationError;

            try
            {
                // Find account by email using LINQ
                var account = await db
                    .Accounts.Where(a => a.Email == loginRequest.Email)
                    .FirstOrDefaultAsync();

                // Check if account exists
                if (
                    account == null
                    || account != null
                        && !_passwordHasher.VerifyPassword(account.Password, loginRequest.Password)
                )
                {
                    return HtppError.Unauthorized("Invalid email or password");
                }

                // Use AuthService to perform sign in (generate tokens, persist refresh token, set cookie)
                var authResponse = await _authService.SignInAsync(account!, Response);
                return HttpSuccess<AuthResponse>.Ok(authResponse, "Login successful");
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex);
            }
        }

        #endregion

        #region Account Info

        /// <summary>
        /// Returns the authenticated user's account info based on their account type
        /// </summary>
        [HttpGet("account")]
        [Authorize]
        public async Task<IActionResult> GetAccountInfo()
        {
            try
            {
                // Get account ID and account type from token claims
                var accountContext = ControllerHelper.GetAccountContextInfo(HttpContext);

                // Check if account ID and account type are valid
                if (!accountContext.HasValue)
                {
                    return HtppError.Unauthorized("Account is not authenticated");
                }

                // Get account details based on account type
                AccountDetails? accountDetails = null;

                // Retrieve account details based on account type
                switch (accountContext.Value.AccountType)
                {
                    case AccountType.Koper:
                        var koper = await db.Kopers.FindAsync(accountContext.Value.AccountId);
                        if (koper != null)
                        {
                            accountDetails = KoperMapper.ToAccountDetails(koper);
                        }
                        break;

                    case AccountType.Kweker:
                        var kweker = await db.Kwekers.FindAsync(accountContext.Value.AccountId);
                        if (kweker != null)
                        {
                            accountDetails = KwekerMapper.ToAccountDetails(kweker);
                        }
                        break;

                    case AccountType.Veilingmeester:
                        var veilingmeester = await db.Veilingmeesters.FindAsync(
                            accountContext.Value.AccountId
                        );
                        if (veilingmeester != null)
                        {
                            accountDetails = VeilingmeesterMapper.ToAccountDetails(veilingmeester);
                        }
                        break;

                    default:
                        return HtppError.BadRequest("Unknown account type");
                }

                // Check if account details are found
                if (accountDetails == null)
                {
                    return HtppError.NotFound("Account details not found");
                }

                return HttpSuccess<AccountDetails>.Ok(
                    accountDetails,
                    "Account details retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex);
            }
        }

        #endregion

        #region Logout

        /// <summary>
        /// Logs out a user by revoking their refresh token
        /// </summary>
        /// <returns>Success message</returns>
        [HttpGet("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Get refresh token from cookie
                Request.Cookies.TryGetValue("refreshToken", out var refreshTokenString);
                await _authService.LogoutAsync(refreshTokenString ?? string.Empty, Response);
                return HttpSuccess<object>.Ok(new { }, "Logout successful");
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex);
            }
        }

        #endregion

        #region Refresh Token

        /// <summary>
        /// Refreshes an access token using a valid refresh token
        /// </summary>
        /// <returns>New access token</returns>
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                // Get refresh token from cookie
                if (
                    !Request.Cookies.TryGetValue("refreshToken", out var refreshTokenString)
                    || string.IsNullOrEmpty(refreshTokenString)
                )
                {
                    return HtppError.Unauthorized("Refresh token not found");
                }

                var authResponse = await _authService.RefreshAsync(refreshTokenString, Response);
                if (authResponse == null)
                {
                    return HtppError.Unauthorized("Invalid or expired refresh token");
                }

                return HttpSuccess<AuthResponse>.Ok(authResponse, "Token refreshed successfully");
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex);
            }
        }

        #endregion

        #region Revoke All Tokens

        /// <summary>
        /// Revokes all refresh tokens for a specific account (useful for security)
        /// Requires authentication
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("revoke-all")]
        [VeilingKlokApp.Attributes.Authorize]
        public async Task<IActionResult> RevokeAllTokens()
        {
            try
            {
                // Get authenticated user's account ID
                var accountContext = ControllerHelper.GetAccountContextInfo(HttpContext);

                if (!accountContext.HasValue)
                {
                    return HtppError.Unauthorized("Invalid token claims");
                }

                var accountId = accountContext.Value.AccountId;

                // Revoke all active refresh tokens for this account using LINQ
                var activeTokens = await db
                    .RefreshTokens.Where(rt => rt.AccountId == accountId && rt.RevokedAt == null)
                    .ToListAsync();

                foreach (var token in activeTokens)
                {
                    token.RevokedAt = DateTime.UtcNow;
                }

                await db.SaveChangesAsync();

                // Clear all refresh token cookies (in case user has multiple sessions)
                Response.Cookies.Delete("refreshToken");

                var result = new { count = activeTokens.Count };
                return HttpSuccess<object>.Ok(
                    result,
                    $"Revoked {activeTokens.Count} refresh token(s)"
                );
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex);
            }
        }

        #endregion
    }
}
