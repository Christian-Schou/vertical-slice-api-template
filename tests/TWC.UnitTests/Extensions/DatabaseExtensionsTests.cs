using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TWC.API.Database;
using TWC.API.Extensions;

namespace TWC.UnitTests.Extensions;

public class DatabaseExtensionsTests
{
    [Fact]
    public void AddDatabaseServices_ShouldRegisterApplicationDbContext()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:Database", "Host=localhost;Database=test;Username=postgres;Password=postgres" }
            })
            .Build();

        // Act
        services.AddDatabaseServices(configuration);

        // Assert
        services.ShouldContain(d => d.ServiceType == typeof(ApplicationDbContext) &&
                                    d.Lifetime == ServiceLifetime.Scoped);

        services.ShouldContain(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
    }
}