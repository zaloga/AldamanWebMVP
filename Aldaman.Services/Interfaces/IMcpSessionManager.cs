using System.Threading.Channels;

namespace Aldaman.Services.Interfaces;

/// <summary>
/// Manages MCP sessions for SSE transport.
/// </summary>
public interface IMcpSessionManager
{
    /// <summary>
    /// Creates a new session and returns its ID.
    /// </summary>
    string CreateSession();

    /// <summary>
    /// Gets the message channel for a session.
    /// </summary>
    Channel<string>? GetChannel(string sessionId);

    /// <summary>
    /// Sends a message to a session's SSE stream.
    /// </summary>
    Task SendMessageAsync(string sessionId, string message);

    /// <summary>
    /// Closes and removes a session.
    /// </summary>
    void RemoveSession(string sessionId);
}
