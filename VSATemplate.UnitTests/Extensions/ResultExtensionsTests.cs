using Shouldly;
using FluentResults;
using VSATemplate.Extensions;

namespace VSATemplate.UnitTests.Extensions;

public class ResultExtensionsTests
{
    [Fact]
    public void Match_ShouldReturnOnSuccess_WhenResultIsSuccess()
    {
        // Arrange
        var result = Result.Ok();

        // Act
        var output = result.Match(
            onSuccess: () => "Success",
            onFailure: _ => "Failure"
        );

        // Assert
        output.ShouldBe("Success");
    }

    [Fact]
    public void Match_ShouldReturnOnFailure_WhenResultIsFailure()
    {
        // Arrange
        var result = Result.Fail("Something went wrong");

        // Act
        var output = result.Match(
            onSuccess: () => "Success",
            onFailure: error => error.Message
        );

        // Assert
        output.ShouldBe("Something went wrong");
    }

    [Fact]
    public void MatchGeneric_ShouldReturnOnSuccess_WhenResultIsSuccess()
    {
        // Arrange
        var result = Result.Ok(42);

        // Act
        var output = result.Match(
            onSuccess: () => result.Value.ToString(),
            onFailure: _ => "Failure"
        );

        // Assert
        output.ShouldBe("42");
    }

    [Fact]
    public void MatchGeneric_ShouldReturnOnFailure_WhenResultIsFailure()
    {
        // Arrange
        var result = Result.Fail<int>("Something went wrong");

        // Act
        var output = result.Match(
            onSuccess: () => result.Value.ToString(),
            onFailure: error => error.Message
        );

        // Assert
        output.ShouldBe("Something went wrong");
    }
}

