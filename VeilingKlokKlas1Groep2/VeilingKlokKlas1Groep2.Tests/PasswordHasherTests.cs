using VeilingKlokKlas1Groep2.Services;

namespace VeilingKlokKlas1Groep2.Tests;

public class PasswordHasherTests
{
    [Fact]
    public void HashPassword_ReturnsDifferentHashEachCall()
    {
        var hasher = new PasswordHasher();

        var hash1 = hasher.HashPassword("secret123");
        var hash2 = hasher.HashPassword("secret123");

        Assert.NotEqual(hash1, hash2); // salts should make hashes unique
        Assert.Contains('.', hash1);
        Assert.Contains('.', hash2);
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
}
