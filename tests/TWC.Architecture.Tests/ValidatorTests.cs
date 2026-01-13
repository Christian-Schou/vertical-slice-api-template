using NetArchTest.Rules;
using TWC.API.Entities;

namespace TWC.Architecture.Tests;

public class ValidatorTests
{
    [Fact]
    public void Validators_Should_Be_Sealed()
    {
        var result = Types.InAssembly(typeof(Product).Assembly)
            .That()
            .ResideInNamespace(ArchitectureConstants.FeaturesNamespace)
            .And()
            .HaveNameEndingWith("Validator")
            .Should()
            .BeSealed()
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}