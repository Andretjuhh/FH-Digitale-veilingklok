using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VeilingKlokApp.Declarations;
using VeilingKlokApp.Types;

namespace VeilingKlokApp.Utils
{
    public readonly record struct AccountContextInfo(Guid AccountId, AccountType AccountType);

    public static class ControllerHelper
    {
        public static AccountContextInfo? GetAccountContextInfo(HttpContext httpContext)
        {
            var accountId = httpContext.Items["AccountId"] as Guid?;
            var accountTypeString = httpContext.Items["AccountType"] as string;

            if (
                accountId.HasValue
                && !string.IsNullOrEmpty(accountTypeString)
                && Enum.TryParse<AccountType>(accountTypeString, out var accountType)
            )
            {
                return new AccountContextInfo(accountId.Value, accountType);
            }

            return null;
        }

        public static IActionResult? ValidateModelState(ModelStateDictionary modelState)
        {
            // If ModelState contains an error because the DTO couldn't be created
            // (i.e., it was null), your helper method will execute the error logic:
            if (modelState.IsValid)
            {
                return null;
            }

            var errors = modelState
                .Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return HtppError.BadRequest(string.Join(". ", errors));
        }

        public static IActionResult HandleException(Exception ex)
        {
            // If the exception is not an instance of a known error type (which we don't have yet as exceptions),
            // return Internal Server Error.
            // The user asked: "check if the instance is not an isntance of HttpError"
            // Since HtppError is IActionResult, it cannot be thrown/caught as an Exception.
            // Assuming standard handling for now.

            return HtppError.InternalServerError($"An error occurred: {ex.Message}");
        }
    }
}
