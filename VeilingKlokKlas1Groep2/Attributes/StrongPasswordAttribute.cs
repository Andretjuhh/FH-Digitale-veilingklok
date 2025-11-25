using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace VeilingKlokKlas1Groep2.Attributes
{
    /// <summary>
    /// Custom validation attribute for strong password requirements
    /// Ensures passwords meet security standards
    /// </summary>
    public class StrongPasswordAttribute : ValidationAttribute
    {
        private readonly int _minLength;
        private readonly bool _requireUppercase;
        private readonly bool _requireLowercase;
        private readonly bool _requireDigit;
        private readonly bool _requireSpecialChar;

        /// <summary>
        /// Creates a strong password validator with customizable requirements
        /// </summary>
        /// <param name="minLength">Minimum password length (default: 8)</param>
        /// <param name="requireUppercase">Require at least one uppercase letter (default: true)</param>
        /// <param name="requireLowercase">Require at least one lowercase letter (default: true)</param>
        /// <param name="requireDigit">Require at least one digit (default: true)</param>
        /// <param name="requireSpecialChar">Require at least one special character (default: true)</param>
        public StrongPasswordAttribute(
            int minLength = 8,
            bool requireUppercase = true,
            bool requireLowercase = true,
            bool requireDigit = true,
            bool requireSpecialChar = true)
        {
            _minLength = minLength;
            _requireUppercase = requireUppercase;
            _requireLowercase = requireLowercase;
            _requireDigit = requireDigit;
            _requireSpecialChar = requireSpecialChar;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Password is required");
            }

            string password = value.ToString()!;
            var errors = new List<string>();

            // Check minimum length
            if (password.Length < _minLength)
            {
                errors.Add($"Password must be at least {_minLength} characters long");
            }

            // Check for uppercase letter
            if (_requireUppercase && !Regex.IsMatch(password, @"[A-Z]"))
            {
                errors.Add("Password must contain at least one uppercase letter");
            }

            // Check for lowercase letter
            if (_requireLowercase && !Regex.IsMatch(password, @"[a-z]"))
            {
                errors.Add("Password must contain at least one lowercase letter");
            }

            // Check for digit
            if (_requireDigit && !Regex.IsMatch(password, @"\d"))
            {
                errors.Add("Password must contain at least one digit");
            }

            // Check for special character
            if (_requireSpecialChar && !Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>/?]"))
            {
                errors.Add("Password must contain at least one special character (!@#$%^&*()_+-=[]{}etc.)");
            }

            if (errors.Any())
            {
                return new ValidationResult(string.Join(". ", errors));
            }

            return ValidationResult.Success;
        }
    }
}
