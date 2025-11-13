namespace VeilingKlokKlas1Groep2.Declarations
{
    public struct HtppError
    {
        // Name of the error    
        public string name { get; set; } = string.Empty;
        // Error code
        public int code { get; set; }
        // Message of the error
        public string message { get; set; } = string.Empty;

        public HtppError(string name, string message, int code)
        {
            this.name = name;
            this.message = message;
            this.code = code;
        }

    }
}
