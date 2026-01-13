using NetArchTest.Rules;
using TWC.API.Entities;

namespace TWC.Architecture.Tests;

public class EntityTests
{
    [Fact]
    public void Entities_Should_Be_Sealed()
    {
        var result = Types.InAssembly(typeof(Product).Assembly)
            .That()
            .ResideInNamespace(ArchitectureConstants.DomainNamespace)
            .Should()
            .BeSealed()
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}