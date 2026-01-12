using Shouldly;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSubstitute;
using VSATemplate.Extensions;

namespace VSATemplate.UnitTests.Extensions;

public class HealthChecksExtensionsTests
{
    [Fact]
    public void AddHealthCheckServices_ShouldRegisterHealthCheckService()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // We need to register the DbContext to test AddDbContextCheck, otherwise it might fail or not be verifiable easily
        // Actually AddDbContextCheck just registers a health check, it doesn't resolve the DbContext immediately.
        // But let's see.

        // Act
        services.AddHealthCheckServices();

        // Assert
        services.ShouldContain(d => d.ServiceType == typeof(HealthCheckService));
    }

    [Fact]
    public void UseHealthChecks_ShouldAddEndpointSource()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddHealthChecks();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();

        var builder = Substitute.For<IEndpointRouteBuilder>();
        builder.ServiceProvider.Returns(serviceProvider);
        
        var dataSources = new List<EndpointDataSource>();
        builder.DataSources.Returns(dataSources);

        // Act
        builder.UseHealthChecks();

        // Assert
        dataSources.ShouldNotBeEmpty();
    }
}

