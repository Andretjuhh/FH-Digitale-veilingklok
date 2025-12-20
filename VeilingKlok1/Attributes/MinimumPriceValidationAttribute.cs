using System.ComponentModel.DataAnnotations;

namespace VeilingKlokApp.Attributes
{
    /// <summary>
    /// Validates that the minimum price does not exceed the regular price.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MinimumPriceValidationAttribute : ValidationAttribute
    {
        private readonly string _pricePropertyName;

        public MinimumPriceValidationAttribute(string targetPricePropertyName)
        {
            _pricePropertyName = targetPricePropertyName;
            ErrorMessage = "Minimum price cannot be higher than the price.";
        }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            if (value is not decimal minimumPrice)
                return new ValidationResult(
                    $"The value for {validationContext.DisplayName} must be a valid number."
                );

            // Get the price property value
            var priceProperty = validationContext.ObjectType.GetProperty(_pricePropertyName);
            if (priceProperty == null)
            {
                return new ValidationResult($"Property {_pricePropertyName} not found.");
            }

            // Target the value of the price property
            var priceValue = priceProperty.GetValue(validationContext.ObjectInstance);
            if (priceValue is not decimal price)
                return ValidationResult.Success;

            // Validate that minimum price is not higher than price
            if (minimumPrice > price)
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }
    }
}
