using System.Collections.Concurrent;
using System.Threading.Channels;
using Aldaman.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Aldaman.Services.Services;

public sealed class McpSessionManager : IMcpSessionManager
{
    private readonly ConcurrentDictionary<string, Channel<string>> _sessions = new();
    private readonly ILogger<McpSessionManager> _logger;

    public McpSessionManager(ILogger<McpSessionManager> logger)
    {
        _logger = logger;
    }

    public string CreateSession()
    {
        var sessionId = Guid.NewGuid().ToString();
        var channel = Channel.CreateUnbounded<string>();

        if (_sessions.TryAdd(sessionId, channel))
        {
            _logger.LogInformation("MCP Session created: {SessionId}", sessionId);
            return sessionId;
        }

        throw new InvalidOperationException("Failed to create MCP session.");
    }

    public Channel<string>? GetChannel(string sessionId)
    {
        return _sessions.TryGetValue(sessionId, out var channel) ? channel : null;
    }

    public async Task SendMessageAsync(string sessionId, string message)
    {
        if (_sessions.TryGetValue(sessionId, out var channel))
        {
            await channel.Writer.WriteAsync(message);
        }
        else
        {
            _logger.LogWarning("Attempted to send message to non-existent session: {SessionId}", sessionId);
        }
    }

    public void RemoveSession(string sessionId)
    {
        if (_sessions.TryRemove(sessionId, out var channel))
        {
            channel.Writer.TryComplete();
            _logger.LogInformation("MCP Session removed: {SessionId}", sessionId);
        }
    }
}
