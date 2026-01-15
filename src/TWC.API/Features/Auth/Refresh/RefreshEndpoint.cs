using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace TWC.API.Features.Auth.Refresh;

public sealed class RefreshEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/refresh", Handle)
            .WithTags("Auth")
            .AllowAnonymous();
    }

    private static async Task<IResult> Handle(
        [FromBody] RefreshRequest request,
        UserManager<User> userManager,
        ITokenService tokenService)
    {

        ClaimsPrincipal principal;
        try
        {
            principal = tokenService.GetPrincipalFromExpiredToken(request.Token);
        }
        catch
        {
            return Results.BadRequest("Invalid access token or refresh token");
        }

        var userId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrEmpty(userId))
        {
             return Results.BadRequest("Invalid access token or refresh token");
        }

        var user = await userManager.FindByIdAsync(userId);

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTimeOffset.UtcNow)
        {
            return Results.BadRequest("Invalid access token or refresh token");
        }

        var newAccessToken = tokenService.GenerateToken(user);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = tokenService.GetRefreshTokenExpiryTime();
        await userManager.UpdateAsync(user);

        return Results.Ok(new RefreshResponse(newAccessToken, newRefreshToken));
    }
}

