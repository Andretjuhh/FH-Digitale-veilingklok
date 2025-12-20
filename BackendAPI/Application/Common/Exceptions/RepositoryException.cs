using Microsoft.AspNetCore.Http;

namespace Application.Common.Exceptions;

public sealed class RepositoryException : ProcessException
{
    private RepositoryException(int statusCode, string messageCode)
        : base(statusCode, messageCode)
    {
    }

    private RepositoryException(int statusCode, string messageCode, string? message)
        : base(statusCode, messageCode, message)
    {
    }

    private static RepositoryException EntityNotFound(string errorCode)
    {
        return new RepositoryException(StatusCodes.Status404NotFound, errorCode);
    }

    private static RepositoryException EntityAlreadyExists(string errorCode)
    {
        return new RepositoryException(StatusCodes.Status409Conflict, errorCode);
    }

    private static RepositoryException EntityUnauthoriazed(string errorCode)
    {
        return new RepositoryException(StatusCodes.Status401Unauthorized, errorCode);
    }

    public static RepositoryException NotFoundAccount()
    {
        return EntityNotFound("ACCOUNT.NOT_FOUND");
    }

    public static RepositoryException NotFoundAddress()
    {
        return EntityNotFound("ACCOUNT.ADDRESS_NOT_FOUND");
    }

    public static RepositoryException NotFoundProduct()
    {
        return EntityNotFound("PRODUCT.NOT_FOUND");
    }

    public static RepositoryException NotFoundOrder()
    {
        return EntityNotFound("ORDER.NOT_FOUND");
    }

    public static RepositoryException NotFoundVeilingKlok()
    {
        return EntityNotFound("VEILINGKLOK.NOT_FOUND");
    }

    public static RepositoryException ExistingAccount()
    {
        return EntityAlreadyExists("ACCOUNT.ALREADY_EXISTS");
    }
}