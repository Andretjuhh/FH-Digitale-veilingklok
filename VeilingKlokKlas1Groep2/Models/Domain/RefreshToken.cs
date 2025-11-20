using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeilingKlokApp.Models.Domain
{
    /// <summary>
    /// RefreshToken model for storing and managing JWT refresh tokens
    /// Enables secure token rotation and revocation
    /// </summary>
    [Table("RefreshToken")]
    public class RefreshToken
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("token")]
        [Required]
        [MaxLength(500)]
        public string Token { get; set; } = string.Empty;

        [Column("account_id")]
        [Required]
        public int AccountId { get; set; }

        [Column("expires_at")]
        [Required]
        public DateTime ExpiresAt { get; set; }

        [Column("created_at")]
        [Required]
        public DateTime CreatedAt { get; set; }

        [Column("revoked_at")]
        public DateTime? RevokedAt { get; set; }

        [Column("replaced_by_token")]
        [MaxLength(500)]
        public string? ReplacedByToken { get; set; }

        /// <summary>
        /// Check if the refresh token is currently active
        /// </summary>
        [NotMapped]
        public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;

        /// <summary>
        /// Check if the refresh token has expired
        /// </summary>
        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        // Navigation property to Account
        public Account Account { get; set; } = default!;
    }
}
