using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VeilingKlokKlas1Groep2.Declarations;
using VeilingKlokKlas1Groep2.Services;

namespace VeilingKlokKlas1Groep2.Attributes
{
    /// <summary>
    /// Authorization attribute to ensure only authenticated users can access endpoints
    /// Similar to authentication middleware in Node.js/Express
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Get JWT service from DI container
            var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();
            
            if (jwtService == null)
            {
                context.Result = new JsonResult(new HtppError(
                    "Server Error",
                    "JWT service not configured",
                    500
                ))
                {
                    StatusCode = 500
                };
                return;
            }

            // Extract token from Authorization header
            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Result = new JsonResult(new HtppError(
                    "Unauthorized",
                    "Access token is required. Please provide a valid token in the Authorization header",
                    401
                ))
                {
                    StatusCode = 401
                };
                return;
            }

            // Extract token (remove "Bearer " prefix)
            var token = authHeader.Substring("Bearer ".Length).Trim();

            // Validate token
            var principal = jwtService.ValidateAccessToken(token);
            
            if (principal == null)
            {
                context.Result = new JsonResult(new HtppError(
                    "Unauthorized",
                    "Invalid or expired access token",
                    401
                ))
                {
                    StatusCode = 401
                };
                return;
            }

            // Store user information in HttpContext for use in controllers
            context.HttpContext.Items["AccountId"] = jwtService.GetAccountIdFromToken(token);
            context.HttpContext.Items["AccountType"] = jwtService.GetAccountTypeFromToken(token);
            context.HttpContext.User = principal;
        }
    }

    /// <summary>
    /// Authorization attribute to restrict access to specific account types
    /// Usage: [AuthorizeAccountType("Koper", "Kweker")] - allows multiple types
    /// Similar to role-based middleware in Node.js/Express
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAccountTypeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _allowedAccountTypes;

        public AuthorizeAccountTypeAttribute(params string[] allowedAccountTypes)
        {
            _allowedAccountTypes = allowedAccountTypes;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // First, ensure user is authenticated
            var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();
            
            if (jwtService == null)
            {
                context.Result = new JsonResult(new HtppError(
                    "Server Error",
                    "JWT service not configured",
                    500
                ))
                {
                    StatusCode = 500
                };
                return;
            }

            // Extract and validate token
            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Result = new JsonResult(new HtppError(
                    "Unauthorized",
                    "Access token is required",
                    401
                ))
                {
                    StatusCode = 401
                };
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var principal = jwtService.ValidateAccessToken(token);
            
            if (principal == null)
            {
                context.Result = new JsonResult(new HtppError(
                    "Unauthorized",
                    "Invalid or expired access token",
                    401
                ))
                {
                    StatusCode = 401
                };
                return;
            }

            // Get account type from token
            var accountType = jwtService.GetAccountTypeFromToken(token);
            
            if (string.IsNullOrWhiteSpace(accountType) || !_allowedAccountTypes.Contains(accountType))
            {
                context.Result = new JsonResult(new HtppError(
                    "Forbidden",
                    $"Access denied. This resource is only available to: {string.Join(", ", _allowedAccountTypes)}",
                    403
                ))
                {
                    StatusCode = 403
                };
                return;
            }

            // Store user information in HttpContext
            context.HttpContext.Items["AccountId"] = jwtService.GetAccountIdFromToken(token);
            context.HttpContext.Items["AccountType"] = accountType;
            context.HttpContext.User = principal;
        }
    }

    /// <summary>
    /// Authorization attribute to ensure users can only access their own resources
    /// Validates that the route parameter matches the authenticated user's ID
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeOwnerAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _parameterName;

        /// <summary>
        /// Creates an owner authorization check
        /// </summary>
        /// <param name="parameterName">Name of the route parameter to check (default: "accountId")</param>
        public AuthorizeOwnerAttribute(string parameterName = "accountId")
        {
            _parameterName = parameterName;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Get JWT service
            var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();
            
            if (jwtService == null)
            {
                context.Result = new JsonResult(new HtppError(
                    "Server Error",
                    "JWT service not configured",
                    500
                ))
                {
                    StatusCode = 500
                };
                return;
            }

            // Extract and validate token
            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Result = new JsonResult(new HtppError(
                    "Unauthorized",
                    "Access token is required",
                    401
                ))
                {
                    StatusCode = 401
                };
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var principal = jwtService.ValidateAccessToken(token);
            
            if (principal == null)
            {
                context.Result = new JsonResult(new HtppError(
                    "Unauthorized",
                    "Invalid or expired access token",
                    401
                ))
                {
                    StatusCode = 401
                };
                return;
            }

            // Get authenticated user's account ID
            var authenticatedAccountId = jwtService.GetAccountIdFromToken(token);
            
            if (!authenticatedAccountId.HasValue)
            {
                context.Result = new JsonResult(new HtppError(
                    "Unauthorized",
                    "Invalid token claims",
                    401
                ))
                {
                    StatusCode = 401
                };
                return;
            }

            // Get the account ID from route parameters
            if (!context.RouteData.Values.TryGetValue(_parameterName, out var routeValue) ||
                !int.TryParse(routeValue?.ToString(), out int requestedAccountId))
            {
                context.Result = new JsonResult(new HtppError(
                    "Bad Request",
                    $"Invalid or missing parameter: {_parameterName}",
                    400
                ))
                {
                    StatusCode = 400
                };
                return;
            }

            // Verify the authenticated user is accessing their own resource
            if (authenticatedAccountId.Value != requestedAccountId)
            {
                context.Result = new JsonResult(new HtppError(
                    "Forbidden",
                    "You can only access your own account information",
                    403
                ))
                {
                    StatusCode = 403
                };
                return;
            }

            // Store user information in HttpContext
            context.HttpContext.Items["AccountId"] = authenticatedAccountId;
            context.HttpContext.Items["AccountType"] = jwtService.GetAccountTypeFromToken(token);
            context.HttpContext.User = principal;
        }
    }
}
