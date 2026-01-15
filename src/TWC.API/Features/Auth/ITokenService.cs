using System.Security.Claims;

namespace TWC.API.Features.Auth;

public interface ITokenService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    DateTimeOffset GetRefreshTokenExpiryTime();
}
