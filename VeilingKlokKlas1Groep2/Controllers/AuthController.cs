using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeilingKlokApp.Data;
using VeilingKlokApp.Models.Domain;
using VeilingKlokApp.Models.OutputDTOs;
using VeilingKlokKlas1Groep2.Declarations;
using VeilingKlokKlas1Groep2.Models.InputDTOs;
using VeilingKlokKlas1Groep2.Services;
using Microsoft.Extensions.Options;
using VeilingKlokKlas1Groep2.Configuration;

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
        private readonly VeilingKlokContext _db;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly JwtSettings _jwtSettings;
    private readonly IAuthService _authService;

        public AuthController(
            VeilingKlokContext db,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IOptions<JwtSettings> jwtSettings,
            IAuthService authService)
        {
            _db = db;
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
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var error = new HtppError(
                    "Validation Error",
                    string.Join(". ", errors),
                    400
                );
                return BadRequest(error);
            }

            try
            {
                // Find account by email using LINQ
                var account = await _db.Accounts
                    .Where(a => a.Email == loginRequest.Email)
                    .FirstOrDefaultAsync();

                // Check if account exists
                if (account == null)
                {
                    var error = new HtppError(
                        "Unauthorized",
                        "Invalid email or password",
                        401
                    );
                    return Unauthorized(error);
                }

                // Verify password
                if (!_passwordHasher.VerifyPassword(account.Password, loginRequest.Password))
                {
                    var error = new HtppError(
                        "Unauthorized",
                        "Invalid email or password",
                        401
                    );
                    return Unauthorized(error);
                }

                // Determine account type using LINQ
                var accountType = await DetermineAccountType(account.Id);

                // Use AuthService to perform sign in (generate tokens, persist refresh token, set cookie)
                var authResponse = await _authService.SignInAsync(account, accountType, Response);
                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                var error = new HtppError(
                    "Internal Server Error",
                    $"Login failed: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
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
            // Get refresh token from cookie
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshTokenString) || string.IsNullOrEmpty(refreshTokenString))
            {
                var error = new HtppError(
                    "Unauthorized",
                    "Refresh token not found",
                    401
                );
                return Unauthorized(error);
            }
            // Delegate to AuthService which performs validation, rotation and cookie update
            try
            {
                var authResponse = await _authService.RefreshAsync(refreshTokenString, Response);
                if (authResponse == null)
                {
                    var error = new HtppError(
                        "Unauthorized",
                        "Invalid or expired refresh token",
                        401
                    );
                    return Unauthorized(error);
                }

                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                var error = new HtppError(
                    "Internal Server Error",
                    $"Token refresh failed: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion

        #region Logout

        /// <summary>
        /// Logs out a user by revoking their refresh token
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Get refresh token from cookie
                Request.Cookies.TryGetValue("refreshToken", out var refreshTokenString);
                await _authService.LogoutAsync(refreshTokenString ?? string.Empty, Response);
                return Ok(new { message = "Logout successful" });
            }
            catch (Exception ex)
            {
                var error = new HtppError(
                    "Internal Server Error",
                    $"Logout failed: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
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
        [VeilingKlokKlas1Groep2.Attributes.Authorize]
        public async Task<IActionResult> RevokeAllTokens()
        {
            try
            {
                // Get authenticated user's account ID
                var accountId = HttpContext.Items["AccountId"] as int?;

                if (!accountId.HasValue)
                {
                    var error = new HtppError(
                        "Unauthorized",
                        "Invalid token claims",
                        401
                    );
                    return Unauthorized(error);
                }

                // Revoke all active refresh tokens for this account using LINQ
                var activeTokens = await _db.RefreshTokens
                    .Where(rt => rt.AccountId == accountId.Value && rt.RevokedAt == null)
                    .ToListAsync();

                foreach (var token in activeTokens)
                {
                    token.RevokedAt = DateTime.UtcNow;
                }

                await _db.SaveChangesAsync();

                // Clear all refresh token cookies (in case user has multiple sessions)
                Response.Cookies.Delete("refreshToken");

                return Ok(new { message = $"Revoked {activeTokens.Count} refresh token(s)" });
            }
            catch (Exception ex)
            {
                var error = new HtppError(
                    "Internal Server Error",
                    $"Token revocation failed: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion

        #region Account Info

        /// <summary>
        /// Returns the authenticated user's account info (email and account type)
        /// </summary>
        [HttpGet("account")]
        [VeilingKlokKlas1Groep2.Attributes.Authorize]
        public IActionResult GetAccountInfo()
        {
            try
            {
                var email = HttpContext.User?.Claims
                    .FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

                var accountType = HttpContext.Items["AccountType"] as string
                                   ?? HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "AccountType")?.Value
                                   ?? string.Empty;

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(accountType))
                {
                    var error = new HtppError(
                        "Unauthorized",
                        "Invalid token claims",
                        401
                    );
                    return Unauthorized(error);
                }

                return Ok(new { email, accountType });
            }
            catch (Exception ex)
            {
                var error = new HtppError(
                    "Internal Server Error",
                    $"Failed to retrieve account info: {ex.Message}",
                    500
                );
                return StatusCode(500, error);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Determines the account type (Koper, Kweker, or Veilingmeester) using LINQ
        /// </summary>
        private async Task<string> DetermineAccountType(int accountId)
        {
            // Check if Koper
            if (await _db.Kopers.AnyAsync(k => k.Id == accountId))
            {
                return "Koper";
            }

            // Check if Kweker
            if (await _db.Kwekers.AnyAsync(k => k.Id == accountId))
            {
                return "Kweker";
            }

            // Check if Veilingmeester
            if (await _db.Veilingmeesters.AnyAsync(v => v.Id == accountId))
            {
                return "Veilingmeester";
            }

            return "Unknown";
        }

        #endregion
    }
}
