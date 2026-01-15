using System.Net;
using System.Net.Http.Json;
using Shouldly;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TWC.API.Entities;
using TWC.API.Features.Auth.Login;
using TWC.IntegrationTests.Common;

namespace TWC.IntegrationTests.Features.Auth;

public class LoginTests : BaseIntegrationTest
{
    public LoginTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var username = "testuser";
        var password = "Password123!";
        var user = new User { UserName = username, Email = "test@example.com" };

        using var scope = ServiceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        await userManager.CreateAsync(user, password);

        var request = new LoginRequest(username, password);

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        loginResponse.ShouldNotBeNull();
        loginResponse.Token.ShouldNotBeNullOrEmpty();
        loginResponse.RefreshToken.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var request = new LoginRequest("nonexistent", "wrongpassword");

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
