using System.Text.Json;
using Shouldly;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using VSATemplate.Exceptions;

namespace VSATemplate.UnitTests.Exceptions;

public class GlobalExceptionHandlerTests
{
    private readonly ILogger<GlobalExceptionHandler> _loggerMock;
    private readonly GlobalExceptionHandler _handler;

    public GlobalExceptionHandlerTests()
    {
        _loggerMock = Substitute.For<ILogger<GlobalExceptionHandler>>();
        _handler = new GlobalExceptionHandler(_loggerMock);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldHandleException_AndReturnTrue()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        var exception = new Exception("Test exception");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.TryHandleAsync(context, exception, cancellationToken);

        // Assert
        result.ShouldBeTrue();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        
        // Verify Logger.LogError was called. 
        // Since LogError is an extension method, we must substitute the underlying Log method.
        _loggerMock.ReceivedWithAnyArgs().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());

        responseStream.Position = 0;
        var problemDetails = await JsonSerializer.DeserializeAsync<ProblemDetails>(responseStream);
        
        problemDetails.ShouldNotBeNull();
        problemDetails.Status.ShouldBe(StatusCodes.Status500InternalServerError);
        problemDetails.Title.ShouldBe("Server Error");
    }
}

