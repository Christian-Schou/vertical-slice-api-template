using NetArchTest.Rules;
using VSATemplate.Entities;

namespace VSATemplate.Architecture.Tests;

public class MessageTests
{
    [Fact]
    public void Commands_Should_Be_Sealed()
    {
        var result = Types.InAssembly(typeof(Product).Assembly)
            .That()
            .ResideInNamespace(ArchitectureConstants.FeaturesNamespace)
            .And()
            .HaveNameEndingWith("Command")
            .Should()
            .BeSealed()
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Queries_Should_Be_Sealed()
    {
        var result = Types.InAssembly(typeof(Product).Assembly)
            .That()
            .ResideInNamespace(ArchitectureConstants.FeaturesNamespace)
            .And()
            .HaveNameEndingWith("Query")
            .Should()
            .BeSealed()
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}