namespace Domain.Exceptions;

public sealed class OrderValidationException : DomainException
{
    private OrderValidationException(string code, string message)
        : base(code, message)
    {
    }

    private OrderValidationException(string code)
        : base(code)
    {
    }

    // Define unique, constant error codes (language-agnostic)

    // Factory methods for creating specific exceptions
    public static OrderValidationException InvalidOrderStatus()
    {
        return new OrderValidationException("ORDER.INVALID_ORDER_STATUS");
    }

    public static OrderValidationException OrderIsNotOpen()
    {
        return new OrderValidationException("ORDER.ORDER_IS_NOT_OPEN");
    }

    public static OrderValidationException OrderAlreadyClosed()
    {
        return new OrderValidationException("ORDER.ORDER_ALREADY_CLOSED");
    }

    public static OrderValidationException InvalidOrderedItem()
    {
        return new OrderValidationException("ORDER.INVALID_ORDERED_ITEM");
    }

    public static OrderValidationException MinOrderQuantityOne()
    {
        return new OrderValidationException("ORDER.MIN_ORDER_QUANTITY_ONE");
    }

    public static OrderValidationException InvalidProductPrice()
    {
        return new OrderValidationException("ORDER.INVALID_PRODUCT_PRICE");
    }

    public static OrderValidationException OrderNotFound()
    {
        return new OrderValidationException("ORDER.ORDER_NOT_FOUND");
    }
}