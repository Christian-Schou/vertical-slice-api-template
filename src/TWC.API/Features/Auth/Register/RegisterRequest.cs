namespace TWC.API.Features.Auth.Register;

public record RegisterRequest(string Email, string Username, string Password, string FirstName, string LastName);

