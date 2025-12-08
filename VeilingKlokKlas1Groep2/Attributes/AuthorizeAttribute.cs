using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VeilingKlokApp.Data;
using VeilingKlokKlas1Groep2.Declarations;
using VeilingKlokKlas1Groep2.Services;

namespace VeilingKlokKlas1Groep2.Attributes
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
            var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();
            var dbContext = context.HttpContext.RequestServices.GetService<VeilingKlokContext>();

            if (jwtService == null || dbContext == null)
            {
                context.Result = new JsonResult(new HtppError("Server Error", "Services not configured", 500)) { StatusCode = 500 };
                return;
            }

            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Result = new JsonResult(new HtppError("Unauthorized", "Access token is required", 401)) { StatusCode = 401 };
                return;
            }

            var accessToken = authHeader.Substring("Bearer ".Length).Trim();
            var principal = jwtService.ValidateAccessToken(accessToken);

            if (principal == null)
            {
                context.Result = new JsonResult(new HtppError("Unauthorized", "Invalid or expired access token", 401)) { StatusCode = 401 };
                return;
            }

            if (!context.HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken) || string.IsNullOrEmpty(refreshToken))
            {
                context.Result = new JsonResult(new HtppError("Unauthorized", "Refresh token is missing", 401)) { StatusCode = 401 };
                return;
            }

            var storedRefreshToken = await dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            if (storedRefreshToken == null || !storedRefreshToken.IsActive)
            {
                context.Result = new JsonResult(new HtppError("Unauthorized", "Invalid or expired refresh token", 401)) { StatusCode = 401 };
                return;
            }

            var accountIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(accountIdClaim, out int accountId) || accountId != storedRefreshToken.AccountId)
            {
                context.Result = new JsonResult(new HtppError("Unauthorized", "Token mismatch", 401)) { StatusCode = 401 };
                return;
            }

            context.HttpContext.Items["AccountId"] = accountId;
            context.HttpContext.Items["AccountType"] = principal.FindFirst("AccountType")?.Value;
            context.HttpContext.User = principal;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAccountTypeAttribute : AuthorizeAttribute
    {
        private readonly string[] _allowedAccountTypes;

        public AuthorizeAccountTypeAttribute(params string[] allowedAccountTypes)
        {
            _allowedAccountTypes = allowedAccountTypes;
        }

        public override async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            await base.OnAuthorizationAsync(context);
            if (context.Result != null) return;

            var accountType = context.HttpContext.Items["AccountType"]?.ToString();
            if (string.IsNullOrEmpty(accountType) || !_allowedAccountTypes.Contains(accountType))
            {
                context.Result = new JsonResult(new HtppError("Forbidden", $"Access denied. Required: {string.Join(", ", _allowedAccountTypes)}", 403)) { StatusCode = 403 };
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeOwnerAttribute : AuthorizeAttribute
    {
        private readonly string _parameterName;

        public AuthorizeOwnerAttribute(string parameterName = "accountId")
        {
            _parameterName = parameterName;
        }

        public override async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            await base.OnAuthorizationAsync(context);
            if (context.Result != null) return;

            var authenticatedAccountId = (int?)context.HttpContext.Items["AccountId"];
            if (!authenticatedAccountId.HasValue)
            {
                context.Result = new JsonResult(new HtppError("Unauthorized", "Invalid token claims", 401)) { StatusCode = 401 };
                return;
            }

            if (!context.RouteData.Values.TryGetValue(_parameterName, out var routeValue) ||
                !int.TryParse(routeValue?.ToString(), out int requestedAccountId))
            {
                context.Result = new JsonResult(new HtppError("Bad Request", $"Invalid or missing parameter: {_parameterName}", 400)) { StatusCode = 400 };
                return;
            }

            if (authenticatedAccountId.Value != requestedAccountId)
            {
                context.Result = new JsonResult(new HtppError("Forbidden", "You can only access your own account information", 403)) { StatusCode = 403 };
            }
        }
    }
}
