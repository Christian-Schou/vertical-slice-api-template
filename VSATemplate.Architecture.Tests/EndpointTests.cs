using Carter;
using NetArchTest.Rules;
using VSATemplate.Entities;

namespace VSATemplate.Architecture.Tests;

public class EndpointTests
{
    [Fact]
    public void Endpoints_Should_Implement_ICarterModule()
    {
        var result = Types.InAssembly(typeof(Product).Assembly)
            .That()
            .ResideInNamespace(ArchitectureConstants.FeaturesNamespace)
            .And()
            .HaveNameEndingWith("Endpoint")
            .Should()
            .ImplementInterface(typeof(ICarterModule))
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Endpoints_Should_Be_Sealed()
    {
        var result = Types.InAssembly(typeof(Product).Assembly)
            .That()
            .ResideInNamespace(ArchitectureConstants.FeaturesNamespace)
            .And()
            .HaveNameEndingWith("Endpoint")
            .Should()
            .BeSealed()
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}