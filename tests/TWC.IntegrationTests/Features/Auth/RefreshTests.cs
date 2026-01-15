using System.Net;
using System.Net.Http.Json;
using Shouldly;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TWC.API.Entities;
using TWC.API.Features.Auth.Login;
using TWC.API.Features.Auth.Refresh;
using TWC.IntegrationTests.Common;

namespace TWC.IntegrationTests.Features.Auth;

public class RefreshTests : BaseIntegrationTest
{
    public RefreshTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Refresh_ShouldReturnNewTokens_WhenRequestIsValid()
    {
        // Arrange
        var username = "testuser_refresh";
        var password = "Password123!";
        var user = new User { UserName = username, Email = "test_refresh@example.com" };

        using var scope = ServiceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        await userManager.CreateAsync(user, password);

        // Login to get tokens
        var loginRequest = new LoginRequest(username, password);
        var loginResponseMsg = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResponse = await loginResponseMsg.Content.ReadFromJsonAsync<LoginResponse>();

        var refreshRequest = new RefreshRequest(loginResponse!.Token, loginResponse.RefreshToken);

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/refresh", refreshRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var refreshResponse = await response.Content.ReadFromJsonAsync<RefreshResponse>();
        refreshResponse.ShouldNotBeNull();
        refreshResponse.Token.ShouldNotBeNullOrEmpty();
        refreshResponse.RefreshToken.ShouldNotBeNullOrEmpty();
        refreshResponse.Token.ShouldNotBe(loginResponse.Token);
        refreshResponse.RefreshToken.ShouldNotBe(loginResponse.RefreshToken);
    }

    [Fact]
    public async Task Refresh_ShouldFail_WhenRefreshTokenIsInvalid()
    {
        // Arrange
        var username = "testuser_invalid_refresh";
        var password = "Password123!";
        var user = new User { UserName = username, Email = "test_invalid_refresh@example.com" };

        using var scope = ServiceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        await userManager.CreateAsync(user, password);

        // Login
        var loginRequest = new LoginRequest(username, password);
        var loginResponseMsg = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResponse = await loginResponseMsg.Content.ReadFromJsonAsync<LoginResponse>();

        var refreshRequest = new RefreshRequest(loginResponse!.Token, "invalid_refresh_token");

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/refresh", refreshRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Refresh_ShouldFail_WhenRefreshTokenIsExpired()
    {
        // Arrange
        var username = "testuser_expired_refresh";
        var password = "Password123!";
        var user = new User { UserName = username, Email = "test_expired_refresh@example.com" };

        using var scope = ServiceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        await userManager.CreateAsync(user, password);

        // Login to get tokens
        var loginRequest = new LoginRequest(username, password);
        var loginResponseMsg = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResponse = await loginResponseMsg.Content.ReadFromJsonAsync<LoginResponse>();

        // Manually expire the refresh token in the database
        // We need to fetch the user again to update it as it might be detached or we need a new session context if using Marten directly, 
        // but UserManager handles it.
        var userFromDb = await userManager.FindByNameAsync(username);
        userFromDb!.RefreshTokenExpiryTime = DateTimeOffset.UtcNow.AddDays(-1);
        await userManager.UpdateAsync(userFromDb);

        var refreshRequest = new RefreshRequest(loginResponse!.Token, loginResponse.RefreshToken);

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/refresh", refreshRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}

