using Microsoft.AspNetCore.Http;

namespace Application.Common.Exceptions;

public class CustomException : ProcessException
{
    private CustomException(int statusCode, string messageCode)
        : base(statusCode, messageCode) { }

    private CustomException(int statusCode, string messageCode, string? message)
        : base(statusCode, messageCode, message) { }

    public static CustomException ExistingTransaction()
    {
        return new CustomException(
            StatusCodes.Status409Conflict,
            "CUSTOM.EXISTING_TRANSACTION",
            "A database transaction is already in progress."
        );
    }

    public static CustomException AlreadyActiveVeilingInRegion()
    {
        return new CustomException(
            StatusCodes.Status409Conflict,
            "CUSTOM.ALREADY_ACTIVE_VEILING_IN_REGION",
            "There is already an active veiling klok in this region."
        );
    }

    public static CustomException MissingJwtConfiguration()
    {
        return new CustomException(
            StatusCodes.Status500InternalServerError,
            "CUSTOM.JWT_CONFIG_MISSING"
        );
    }

    public static CustomException CannotUpdateProductLinkedToActiveVeilingKlok()
    {
        return new CustomException(
            StatusCodes.Status400BadRequest,
            "CUSTOM.CANNOT_UPDATE_PRODUCT_LINKED_TO_ACTIVE_VEILING_KLOK"
        );
    }

    public static CustomException InvalidOperation()
    {
        return new CustomException(StatusCodes.Status400BadRequest, "CUSTOM.INVALID_OPERATION");
    }

    public static CustomException InvalidCredentials()
    {
        return new CustomException(StatusCodes.Status401Unauthorized, "CUSTOM.INVALID_CREDENTIALS");
    }

    public static CustomException AccountSoftDeleted()
    {
        return new CustomException(StatusCodes.Status403Forbidden, "CUSTOM.ACCOUNT_SOFT_DELETED");
    }

    public static CustomException AccountLocked()
    {
        return new CustomException(
            StatusCodes.Status401Unauthorized,
            "CUSTOM.ACCOUNT_LOCKED",
            "Account is locked due to too many failed attempts."
        );
    }

    public static CustomException VeilingKlokNotStarted()
    {
        return new CustomException(
            StatusCodes.Status400BadRequest,
            "CUSTOM.VEILING_KLOK_NOT_STARTED"
        );
    }

    public static CustomException InvalidVeilingKlokStatus()
    {
        return new CustomException(
            StatusCodes.Status400BadRequest,
            "CUSTOM.INVALID_VEILING_KLOK_STATUS"
        );
    }

    public static CustomException InvalidVeilingKlokProductId()
    {
        return new CustomException(
            StatusCodes.Status400BadRequest,
            "CUSTOM.INVALID_VEILING_KLOK_PRODUCT_ID"
        );
    }

    public static CustomException InvalidVeilingKlokNoProduct()
    {
        return new CustomException(
            StatusCodes.Status400BadRequest,
            "CUSTOM.INVALID_VEILING_KLOK_NO_PRODUCT"
        );
    }

    public static CustomException CannotChangeRunningVeilingKlok()
    {
        return new CustomException(
            StatusCodes.Status400BadRequest,
            "CUSTOM.CANNOT_CHANGE_RUNNING_VEILING_KLOK"
        );
    }

    public static CustomException CannotDeleteStartedVeilingKlok()
    {
        return new CustomException(
            StatusCodes.Status400BadRequest,
            "CUSTOM.CANNOT_DELETE_STARTED_VEILING_KLOK",
            "Cannot delete a VeilingKlok that has already started. Only scheduled VeilingKlokken can be deleted."
        );
    }

    public static CustomException InvalidOperationKlokStillRunning()
    {
        return new CustomException(
            StatusCodes.Status400BadRequest,
            "CUSTOM.INVALID_OPERATION_KLOK_STILL_RUNNING"
        );
    }

    public static CustomException InvalidProductPrice()
    {
        return new CustomException(StatusCodes.Status400BadRequest, "CUSTOM.INVALID_PRODUCT_PRICE");
    }

    public static CustomException FailedTokenGeneration()
    {
        return new CustomException(
            StatusCodes.Status500InternalServerError,
            "CUSTOM.TOKEN_GENERATION_FAILED"
        );
    }

    public static CustomException UnauthorizedAccess()
    {
        return new CustomException(StatusCodes.Status401Unauthorized, "CUSTOM.UNAUTHORIZED_ACCESS");
    }

    public static CustomException AccessForbidden()
    {
        return new CustomException(StatusCodes.Status403Forbidden, "CUSTOM.ACCESS_FORBIDDEN");
    }

    public static CustomException InsufficientPermissions()
    {
        return new CustomException(
            StatusCodes.Status403Forbidden,
            "CUSTOM.INSUFFICIENT_PERMISSIONS"
        );
    }

    public static CustomException InsufficientStock()
    {
        return new CustomException(StatusCodes.Status400BadRequest, "PRODUCT.INSUFFICIENT_STOCK");
    }

    public static CustomException ProductAlreadyLinkedToVeilingKlok()
    {
        return new CustomException(
            StatusCodes.Status400BadRequest,
            "CUSTOM.PRODUCT_ALREADY_LINKED_TO_VEILING_KLOK"
        );
    }

    public static CustomException CannotDeleteProductWithOrders()
    {
        return new CustomException(
            StatusCodes.Status400BadRequest,
            "CUSTOM.CANNOT_DELETE_PRODUCT_WITH_ORDERS",
            "Cannot delete a product that has existing orders."
        );
    }

    public static CustomException ProductNotLinkedToVeilingKlok()
    {
        return new CustomException(
            StatusCodes.Status400BadRequest,
            "CUSTOM.PRODUCT_NOT_LINKED_TO_VEILING_KLOK"
        );
    }

    public static CustomException ProductAlreadyInVeilingKlok()
    {
        return new CustomException(
            StatusCodes.Status400BadRequest,
            "CUSTOM.PRODUCT_ALREADY_IN_VEILING_KLOK"
        );
    }

    public static CustomException AccountDeactivated()
    {
        return new CustomException(
            StatusCodes.Status401Unauthorized,
            "CUSTOM.ACCOUNT_DEACTIVATED",
            "This account has been deactivated."
        );
    }
}
