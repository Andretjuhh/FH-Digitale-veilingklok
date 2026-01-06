namespace Domain.Exceptions
{
    public abstract class DomainException : Exception
    {
        public string ErrorCode { get; private set; }

        // Optional: Pass the raw message/code key here
        protected DomainException(string code, string message)
            : base(message)
        {
            ErrorCode = code;
        }

        protected DomainException(string code)
            : base()
        {
            ErrorCode = code;
        }
    }
}
