using Aldaman.Services.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Aldaman.Web.Tests.Mcp;

/// <summary>
/// Unit tests for <see cref="McpSessionManager"/>.
/// </summary>
public class McpSessionManagerTests
{
    private readonly Mock<ILogger<McpSessionManager>> _loggerMock;
    private readonly McpSessionManager _manager;

    public McpSessionManagerTests()
    {
        _loggerMock = new Mock<ILogger<McpSessionManager>>();
        _manager = new McpSessionManager(_loggerMock.Object);
    }

    [Fact]
    public void CreateSession_ReturnsUniqueId()
    {
        // Act
        var id1 = _manager.CreateSession();
        var id2 = _manager.CreateSession();

        // Assert
        Assert.False(string.IsNullOrEmpty(id1));
        Assert.False(string.IsNullOrEmpty(id2));
        Assert.NotEqual(id1, id2);
    }

    [Fact]
    public async Task SendMessage_WritesToChannel()
    {
        // Arrange
        var sessionId = _manager.CreateSession();
        var channel = _manager.GetChannel(sessionId);
        var message = "{\"test\":1}";

        // Act
        await _manager.SendMessageAsync(sessionId, message);

        // Assert
        Assert.NotNull(channel);
        var received = await channel.Reader.ReadAsync();
        Assert.Equal(message, received);
    }

    [Fact]
    public void RemoveSession_CompletesChannel()
    {
        // Arrange
        var sessionId = _manager.CreateSession();
        var channel = _manager.GetChannel(sessionId);

        // Act
        _manager.RemoveSession(sessionId);

        // Assert
        Assert.NotNull(channel);
        Assert.True(channel.Reader.Completion.IsCompleted);
        Assert.Null(_manager.GetChannel(sessionId));
    }
}
