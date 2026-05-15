using System.Text.Json.Serialization;

namespace Aldaman.Services.Dtos.Mcp.RpcModels;

/// <summary>
/// Represents a JSON-RPC 2.0 request or notification.
/// </summary>
public class JsonRpcRequest
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;

    [JsonPropertyName("params")]
    public object? Params { get; set; }

    [JsonPropertyName("id")]
    public object? Id { get; set; }
}
