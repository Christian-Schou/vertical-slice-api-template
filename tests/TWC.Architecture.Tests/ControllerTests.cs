using Microsoft.AspNetCore.Mvc;
using NetArchTest.Rules;
using TWC.API.Entities;

namespace TWC.Architecture.Tests;

public class ControllerTests
{
    [Fact]
    public void Controllers_Should_Not_Exist()
    {
        var result = Types.InAssembly(typeof(Product).Assembly)
            .Should()
            .NotInherit(typeof(ControllerBase))
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}