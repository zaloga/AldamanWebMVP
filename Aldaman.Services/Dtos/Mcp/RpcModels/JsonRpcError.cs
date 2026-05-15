using System.Text.Json.Serialization;

namespace Aldaman.Services.Dtos.Mcp.RpcModels;

/// <summary>
/// Represents a JSON-RPC error object.
/// </summary>
public class JsonRpcError
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public object? Data { get; set; }
}
