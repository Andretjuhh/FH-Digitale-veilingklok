using Domain.Interfaces;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Infrastructure.MicroServices.Security;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string rawPassword)
    {
        return BCryptNet.HashPassword(rawPassword);
    }

    public bool Verify(string providedPassword, string hashedPassword)
    {
        return BCryptNet.Verify(providedPassword, hashedPassword);
    }
}