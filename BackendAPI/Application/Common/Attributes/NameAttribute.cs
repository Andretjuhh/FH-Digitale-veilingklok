using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Application.Common.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class NameAttribute : ValidationAttribute
{
    private static readonly Regex NameRegex = new(@"^[\p{L}][\p{L}\p{M}\-'\s]*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public int MinLength { get; }
    public int MaxLength { get; }

    public NameAttribute(int minLength = 2, int maxLength = 50)
    {
        MinLength = minLength;
        MaxLength = maxLength;
        ErrorMessage ??= "NAME.INVALID_CHARACTERS";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is null)
            return ValidationResult.Success; // let [Required] handle nulls

        if (value is not string name)
            return new ValidationResult("NAME.INVALID_FORMAT");

        name = name.Trim();

        if (name.Length < MinLength || name.Length > MaxLength)
            return new ValidationResult("NAME.TOO_LONG");

        if (!NameRegex.IsMatch(name))
            return new ValidationResult("NAME.INVALID_CHARACTERS");

        return ValidationResult.Success;
    }
}