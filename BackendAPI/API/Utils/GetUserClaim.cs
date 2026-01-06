using System.Diagnostics;
using System.Security.Claims;
using Application.Common.Exceptions;
using Domain.Enums;

namespace API.Utils;

public sealed class GetUserClaim
{
    public static (Guid userId, AccountType userRole) GetInfo(ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRole = user.FindFirst(ClaimTypes.Role)?.Value;

        Console.WriteLine($"UserId: {userId}, UserRole: {userRole}  is null = {user.Claims.ToList().Count}");

        if (string.IsNullOrEmpty(userId) ||
            !Guid.TryParse(userId, out var accountId) ||
            string.IsNullOrEmpty(userRole) ||
            !Enum.TryParse<AccountType>(userRole, out var accountType
            )
           )
            throw CustomException.AccessForbidden();

        return (accountId, accountType);
    }
}