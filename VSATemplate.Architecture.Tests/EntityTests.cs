using NetArchTest.Rules;
using VSATemplate.Entities;

namespace VSATemplate.Architecture.Tests;

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