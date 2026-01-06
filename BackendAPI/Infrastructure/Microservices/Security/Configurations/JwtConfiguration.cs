using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.MicroServices.Security.Configurations;

public class JwtConfiguration
{
    // Properties must match the keys inside the "Jwt" in appsettings.json
    public const string Key = "JwtSettings";
    public required string Secret { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required int AccessTokenExpirationMinutes { get; init; } = 45; // Default: 45 minutes
    public required int RefreshTokenExpirationDays { get; init; } = 365; // Default: 365 days
}