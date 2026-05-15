using System.Text.Json.Serialization;

namespace Aldaman.Services.Dtos.Mcp.LifecycleModels;

/// <summary>
/// Represents the parameters for the 'initialize' request in the MCP protocol.
/// </summary>
public class McpInitializeParams
{
    [JsonPropertyName("protocolVersion")]
    public string ProtocolVersion { get; set; } = string.Empty;

    [JsonPropertyName("capabilities")]
    public McpClientCapabilities Capabilities { get; set; } = new();

    [JsonPropertyName("clientInfo")]
    public McpImplementation? ClientInfo { get; set; }
}
