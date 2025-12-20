namespace VeilingKlokApp.Declarations
{
    /// <summary>
    /// JWT configuration settings
    /// These values should be configured in appsettings.json
    /// </summary>
    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenExpirationMinutes { get; set; } = 15; // Default: 15 minutes
        public int RefreshTokenExpirationDays { get; set; } = 7; // Default: 7 days
    }
}
