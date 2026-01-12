using NetArchTest.Rules;
using VSATemplate.Entities;

namespace VSATemplate.Architecture.Tests;

public class LayerTests
{
    [Fact]
    public void Domain_Should_Not_Depend_On_Other_Layers()
    {
        var result = Types.InAssembly(typeof(Product).Assembly)
            .That()
            .ResideInNamespace(ArchitectureConstants.DomainNamespace)
            .Should()
            .NotHaveDependencyOn(ArchitectureConstants.FeaturesNamespace)
            .And()
            .NotHaveDependencyOn(ArchitectureConstants.DatabaseNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}