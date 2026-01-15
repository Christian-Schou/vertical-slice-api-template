using Microsoft.AspNetCore.Identity;

namespace TWC.API.Features.Auth.Login;

public sealed class LoginEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", Handle)
            .WithTags("Auth")
            .AllowAnonymous();
    }

    private static async Task<IResult> Handle(
        LoginRequest request, 
        UserManager<User> userManager, 
        ITokenService tokenService) 
    {
        var user = await userManager.FindByNameAsync(request.Username);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Results.Unauthorized();
        }

        var token = tokenService.GenerateToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = tokenService.GetRefreshTokenExpiryTime();
        
        await userManager.UpdateAsync(user);

        return Results.Ok(new LoginResponse(token, refreshToken));
    }
}
