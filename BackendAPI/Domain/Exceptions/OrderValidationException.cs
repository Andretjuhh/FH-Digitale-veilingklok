namespace Domain.Exceptions
{
    public sealed class OrderValidationException : DomainException
    {
        private OrderValidationException(string code, string message)
            : base(code, message) { }

        private OrderValidationException(string code)
            : base(code) { }

        // Define unique, constant error codes (language-agnostic)

        // Factory methods for creating specific exceptions
        public static OrderValidationException InvalidOrderStatus() =>
            new("ORDER.INVALID_ORDER_STATUS");

        public static OrderValidationException OrderIsNotOpen() => new("ORDER.ORDER_IS_NOT_OPEN");

        public static OrderValidationException OrderAlreadyClosed() =>
            new("ORDER.ORDER_ALREADY_CLOSED");

        public static OrderValidationException InvalidOrderedItem() =>
            new("ORDER.INVALID_ORDERED_ITEM");

        public static OrderValidationException MinOrderQuantityOne() =>
            new("ORDER.MIN_ORDER_QUANTITY_ONE");

        public static OrderValidationException InvalidProductPrice() =>
            new("ORDER.INVALID_PRODUCT_PRICE");
    }
}
