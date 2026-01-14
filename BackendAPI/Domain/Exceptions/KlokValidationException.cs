namespace Domain.Exceptions;

public sealed class KlokValidationException : DomainException
{
    private KlokValidationException(string code, string message)
        : base(code, message)
    {
    }

    private KlokValidationException(string code)
        : base(code)
    {
    }

    public static KlokValidationException ProductNotInVeilingKlok()
    {
        return new KlokValidationException(
            "KLOK.PRODUCT_NOT_IN_KLOK",
            "The specified product is not part of the VeilingKlok."
        );
    }

    public static KlokValidationException InvalidDuration()
    {
        return new KlokValidationException(
            "KLOK.INVALID_DURATION",
            "The provided duration for the VeilingKlok is invalid."
        );
    }

    // Factory methods for creating specific exceptions
    public static KlokValidationException InvalidLivePeakedView()
    {
        return new KlokValidationException(
            "KLOK.INVALID_PEAK_VIEW",
            "The new peaked live view count is invalid."
        );
    }

    public static KlokValidationException KlokNotAvailableForUpdate()
    {
        return new KlokValidationException(
            "KLOK.NOT_AVAILABLE_FOR_UPDATE",
            "The VeilingKlok is not available for the requested update."
        );
    }

    public static KlokValidationException InvalidStatusTransition()
    {
        return new KlokValidationException(
            "KLOK.INVALID_STATUS_TRANSITION",
            "The status transition is invalid."
        );
    }

    public static KlokValidationException CannotModifyEndedKlok()
    {
        return new KlokValidationException(
            "KLOK.CANNOT_MODIFY_ENDED_KLOK",
            "Cannot modify a VeilingKlok that has already ended."
        );
    }

    public static KlokValidationException InvalidProductIndex()
    {
        return new KlokValidationException(
            "KLOK.INVALID_PRODUCT_INDEX",
            "The provided product index is invalid."
        );
    }
}