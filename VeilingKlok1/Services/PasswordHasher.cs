using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace VeilingKlokApp.Services
{
    /// <summary>
    /// Service for secure password hashing and verification
    /// Uses PBKDF2 algorithm with salt for maximum security
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashes a password using PBKDF2 with a random salt
        /// </summary>
        /// <param name="password">Plain text password to hash</param>
        /// <returns>Hashed password with embedded salt</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifies a password against a hashed password
        /// </summary>
        /// <param name="hashedPassword">The stored hashed password</param>
        /// <param name="providedPassword">The password to verify</param>
        /// <returns>True if passwords match, false otherwise</returns>
        bool VerifyPassword(string hashedPassword, string providedPassword);
    }

    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 128 / 8; // 128 bits
        private const int KeySize = 256 / 8; // 256 bits
        private const int Iterations = 10000;

        /// <summary>
        /// Hashes a password using PBKDF2 with random salt
        /// Format: [iterations].[salt].[hash]
        /// </summary>
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(password));
            }

            // Generate a random salt
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash the password with the salt
            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: Iterations,
                numBytesRequested: KeySize
            );

            // Combine salt and hash for storage
            // Format: iterations.salt.hash (all base64 encoded)
            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        /// <summary>
        /// Verifies a password against a stored hash
        /// </summary>
        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword))
            {
                throw new ArgumentException(
                    "Hashed password cannot be null or empty",
                    nameof(hashedPassword)
                );
            }

            if (string.IsNullOrWhiteSpace(providedPassword))
            {
                return false;
            }

            try
            {
                // Split the stored hash into its components
                var parts = hashedPassword.Split('.');
                if (parts.Length != 3)
                {
                    return false;
                }

                var iterations = int.Parse(parts[0]);
                var salt = Convert.FromBase64String(parts[1]);
                var hash = Convert.FromBase64String(parts[2]);

                // Hash the provided password with the same salt
                byte[] testHash = KeyDerivation.Pbkdf2(
                    password: providedPassword,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: iterations,
                    numBytesRequested: hash.Length
                );

                // Compare the hashes using constant-time comparison
                return CryptographicOperations.FixedTimeEquals(hash, testHash);
            }
            catch
            {
                // If any error occurs during verification, return false
                return false;
            }
        }
    }
}
