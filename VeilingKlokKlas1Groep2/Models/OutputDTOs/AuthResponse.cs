namespace VeilingKlokApp.Models.OutputDTOs
{
    /// <summary>
    /// DTO for authentication responses containing tokens and user info
    /// Note: RefreshToken is nullable because it's stored in secure HTTP-only cookies
    /// </summary>
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } // Nullable - stored in HTTP-only cookie instead
        public DateTime AccessTokenExpiresAt { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }
        public int AccountId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty; // "Koper", "Kweker", or "Veilingmeester"
    }
}
