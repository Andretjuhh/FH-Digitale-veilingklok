using System;
using VeilingKlokKlas1Groep2.Services;
using Xunit;

namespace VeilingKlokKlas1Groep2.Tests;

public class PasswordHasherTests
{
    [Fact]
    public void HashPassword_ReturnsDifferentHashEachCall()
    {
        var hasher = new PasswordHasher();

        var hash1 = hasher.HashPassword("secret123");
        var hash2 = hasher.HashPassword("secret123");

        Assert.NotEqual(hash1, hash2
        ); // salts should make hashes unique
        Assert.Contains('.', hash1);
        Assert.Contains('.', hash2);
        // Check if compare hash are valid 
        Assert.True(hasher.VerifyPassword(hash1, "secret123"));
        Assert.False(hasher.VerifyPassword(hash2, "secret1234")); // wrong password
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void HashPassword_ThrowsForNullOrWhitespace(string password)
    {
        var hasher = new PasswordHasher();

        Assert.Throws<ArgumentException>(() => hasher.HashPassword(password));
    }

    [Fact]
    public void HashPassword_ProducesExpectedFormat()
    {
        var hasher = new PasswordHasher();

        var hash = hasher.HashPassword("secret123");
        var parts = hash.Split('.');

        Assert.Equal(3, parts.Length);
        Assert.Equal("10000", parts[0]);

        var saltBytes = Convert.FromBase64String(parts[1]);
        var hashBytes = Convert.FromBase64String(parts[2]);

        Assert.Equal(16, saltBytes.Length);
        Assert.Equal(32, hashBytes.Length);
    }


    [Fact]
    public void VerifyPassword_ReturnsTrueForCorrectPassword()
    {
        var hasher = new PasswordHasher();
        var password = "Secr3t!Pass";

        var hash = hasher.HashPassword(password);
        var result = hasher.VerifyPassword(hash, password);

        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_ReturnsFalseForWrongPassword()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.HashPassword("correct-password");

        var result = hasher.VerifyPassword(hash, "wrong-password");

        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_ReturnsFalseForMalformedHash()
    {
        var hasher = new PasswordHasher();

        var result = hasher.VerifyPassword("invalid-format-hash", "secret123");

        Assert.False(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void VerifyPassword_ReturnsFalseForEmptyProvidedPassword(string providedPassword)
    {
        var hasher = new PasswordHasher();
        var hash = hasher.HashPassword("secret123");

        var result = hasher.VerifyPassword(hash, providedPassword);

        Assert.False(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void VerifyPassword_ThrowsForNullOrWhitespaceHash(string hashedPassword)
    {
        var hasher = new PasswordHasher();

        Assert.Throws<ArgumentException>(() => hasher.VerifyPassword(hashedPassword, "secret123"));
    }

    [Fact]
    public void VerifyPassword_ReturnsFalseForInvalidBase64Salt()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.HashPassword("secret123");
        var parts = hash.Split('.');
        var invalidHash = $"{parts[0]}.!!!.{parts[2]}";

        var result = hasher.VerifyPassword(invalidHash, "secret123");

        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_ReturnsFalseForMissingParts()
    {
        var hasher = new PasswordHasher();

        var result = hasher.VerifyPassword("10000|salt|hash", "secret123");

        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_ReturnsFalseWhenIterationsTampered()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.HashPassword("secret123");
        var parts = hash.Split('.');
        var tamperedHash = $"20000.{parts[1]}.{parts[2]}";

        var result = hasher.VerifyPassword(tamperedHash, "secret123");

        Assert.False(result);
    }
    
}
