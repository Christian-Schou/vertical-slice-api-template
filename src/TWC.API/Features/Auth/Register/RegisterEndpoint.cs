using Carter;
using Microsoft.AspNetCore.Identity;
using TWC.API.Entities;

namespace TWC.API.Features.Auth.Register;

public sealed class RegisterEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register", Handle)
            .WithTags("Auth")
            .AllowAnonymous();
    }

    private static async Task<IResult> Handle(
        RegisterRequest request, 
        UserManager<User> userManager)
    {
        var user = new User
        {
            UserName = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            return Results.Ok(new { Message = "User registered successfully" });
        }

        return Results.BadRequest(result.Errors);
    }
}

