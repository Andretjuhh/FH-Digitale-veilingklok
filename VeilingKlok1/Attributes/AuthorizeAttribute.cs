using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using VeilingKlokApp.Data;
using VeilingKlokApp.Declarations;
using VeilingKlokApp.Services;
using VeilingKlokApp.Types;
using VeilingKlokApp.Utils;

namespace VeilingKlokApp.Attributes
{
    /// <summary>
    /// Authorization attribute to ensure only authenticated users can access endpoints
    /// Validates both Access Token and Refresh Token for enhanced security
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public virtual async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Retrieve required services contexts
            var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();
            var dbContext = context.HttpContext.RequestServices.GetService<DatabaseContext>();

            // If services are not available, return Internal Server Error
            if (jwtService == null || dbContext == null)
            {
                context.Result = HtppError.InternalServerError(
                    "Required services are not available"
                );
                return;
            }

            // Get Bearer token from Authorization header
            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Result = HtppError.Unauthorized("Missing or invalid Authorization header");
                return;
            }

            // Validate Access Token Return Claims Principal
            var principal = jwtService.ValidateAccessToken(
                authHeader.Substring("Bearer ".Length).Trim()
            );

            // Proceed only if principal is valid
            if (principal == null)
            {
                context.Result = HtppError.Unauthorized("Invalid access token");
                return;
            }

            // Validate Refresh Token from cookies
            if (
                !context.HttpContext.Request.Cookies.TryGetValue(
                    "refreshToken",
                    out var refreshToken
                ) || string.IsNullOrEmpty(refreshToken)
            )
            {
                context.Result = HtppError.Unauthorized("Missing refresh token");
                return;
            }

            // Check if refresh token exists and is active in the database
            var storedRefreshToken = await dbContext.RefreshTokens.FirstOrDefaultAsync(rt =>
                rt.Token == refreshToken
            );
            if (storedRefreshToken == null || !storedRefreshToken.IsActive)
            {
                context.Result = HtppError.Unauthorized("Invalid or expired refresh token");
                return;
            }

            // Validate that the account ID in the access token matches the one associated with the refresh token
            var accountIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var accountTypeClaim = principal.FindFirst(ClaimTypes.Role)?.Value;
            if (
                !Guid.TryParse(accountIdClaim, out Guid accountId)
                || accountId != storedRefreshToken.AccountId
            )
            {
                context.Result = HtppError.Unauthorized("Token account mismatch");
                return;
            }

            // Store account info in HttpContext for downstream access
            context.HttpContext.Items["AccountId"] = accountId;
            context.HttpContext.Items["AccountType"] = accountTypeClaim;
            context.HttpContext.User = principal;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAccountTypeAttribute : AuthorizeAttribute
    {
        private readonly AccountType[] _allowedAccountTypes;

        public AuthorizeAccountTypeAttribute(params AccountType[] allowedAccountTypes)
        {
            _allowedAccountTypes = allowedAccountTypes;
        }

        public override async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            await base.OnAuthorizationAsync(context);
            if (context.Result != null)
                return;

            var accountTypeString = context.HttpContext.Items["AccountType"]?.ToString();
            if (
                string.IsNullOrEmpty(accountTypeString)
                || !Enum.TryParse<AccountType>(accountTypeString, out var accountType)
                || !_allowedAccountTypes.Contains(accountType)
            )
            {
                context.Result = HtppError.Forbidden(
                    $"Access denied. Required: {string.Join(", ", _allowedAccountTypes)}"
                );
            }
        }
    }
}
