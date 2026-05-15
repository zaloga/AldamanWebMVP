using Aldaman.Services.Dtos.Mcp.RpcModels;

namespace Aldaman.Services.Interfaces;

/// <summary>
/// Handles incoming MCP (JSON-RPC) messages.
/// </summary>
public interface IMcpMessageHandlerService
{
    /// <summary>
    /// Handles an MCP JSON-RPC request and returns a response, or null if it's a notification.
    /// </summary>
    Task<JsonRpcResponse?> HandleRequestAsync(JsonRpcRequest request, string baseUrl, CancellationToken cancellationToken = default);
}
