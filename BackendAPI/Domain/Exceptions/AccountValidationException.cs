namespace Domain.Exceptions
{
    public sealed class AccountValidationException : DomainException
    {
        private AccountValidationException(string code, string message)
            : base(code, message) { }

        private AccountValidationException(string code)
            : base(code) { }

        // Define unique, constant error codes (language-agnostic)

        // Factory methods for creating specific exceptions
        public static AccountValidationException EmailInvalid() => new("ACCOUNT.EMAIL_INVALID");

        public static AccountValidationException PasswordTooWeak() =>
            new("ACCOUNT.PASSWORD_TOO_WEAK");

        public static AccountValidationException AdressMaximum() => new("ACCOUNT.ADRESS_MAXIMUM");

        public static AccountValidationException InvalidAdress() => new("ACCOUNT.INVALID_ADRESS");

        public static AccountValidationException CannotRemovePrimaryAdress() =>
            new("ACCOUNT.CANNOT_REMOVE_PRIMARY_ADRESS");

        public static AccountValidationException PasswordEmpty() =>
            new("ACCOUNT.PASSWORD_EMPTY", "Password cannot be empty.");

        public static AccountValidationException PasswordTooShort() =>
            new("ACCOUNT.PASSWORD_TOO_SHORT", "Password must be at least 8 characters.");

        public static AccountValidationException PasswordMissingUppercase() =>
            new("ACCOUNT.PASSWORD_MISSING_UPPERCASE", "Password must contain an uppercase letter.");

        public static AccountValidationException PasswordMissingDigit() =>
            new("ACCOUNT.PASSWORD_MISSING_DIGIT", "Password must contain a digit.");
    }
}
