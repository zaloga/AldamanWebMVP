using System.Text.Json.Serialization;

namespace Aldaman.Services.Dtos.Mcp.LifecycleModels;

/// <summary>
/// Represents the result of an 'initialize' request in the MCP protocol.
/// </summary>
public class McpInitializeResult
{
    /// <summary>
    /// The protocol version that the server decided to use. 
    /// "2024-11-05" is the current stable version of the MCP protocol.
    /// </summary>
    [JsonPropertyName("protocolVersion")]
    public string ProtocolVersion { get; set; } = "2024-11-05";

    [JsonPropertyName("capabilities")]
    public McpServerCapabilities Capabilities { get; set; } = new();

    [JsonPropertyName("serverInfo")]
    public McpImplementation ServerInfo { get; set; } = new();
}
