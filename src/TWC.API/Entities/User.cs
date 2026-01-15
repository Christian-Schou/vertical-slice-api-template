using Marten.AspNetIdentity;
using Microsoft.AspNetCore.Identity;

namespace TWC.API.Entities;

public sealed class User : IdentityUser, IClaimsUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public List<System.Security.Claims.Claim> Claims { get; set; } = new();
    public IList<string> RoleClaims { get; set; } = [];

    public string? RefreshToken { get; set; }
    public DateTimeOffset? RefreshTokenExpiryTime { get; set; }
}
