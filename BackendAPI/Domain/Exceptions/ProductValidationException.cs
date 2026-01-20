namespace Domain.Exceptions
{
    public sealed class ProductValidationException : DomainException
    {
        private ProductValidationException(string code, string message)
            : base(code, message) { }

        private ProductValidationException(string code)
            : base(code) { }

        // Define unique, constant error codes (language-agnostic)

        // Factory methods for creating specific exceptions
        public static ProductValidationException ProductDoesntExsist() =>
            new("PRODUCT.DOES_NOT_EXIST");

        public static ProductValidationException InvalidProductPrice() =>
            new("PRODUCT.INVALID_PRODUCT_PRICE");

        public static ProductValidationException InvalidMinimumPrice() =>
            new("PRODUCT.INVALID_MINIMUM_PRICE");

        public static ProductValidationException InsufficientProductStock() =>
            new("PRODUCT.INSUFFICIENT_PRODUCT_STOCK");

        public static ProductValidationException ProductNotAvailableForAuction() =>
            new("PRODUCT.NOT_AVAILABLE_FOR_AUCTION");

        public static ProductValidationException MinimumPriceExceedsPrice() =>
            new("PRODUCT.MINIMUM_PRICE_EXCEEDS_PRICE");

        public static ProductValidationException NegativeStockValue() =>
            new("PRODUCT.NEGATIVE_STOCK_VALUE");
    }
}
