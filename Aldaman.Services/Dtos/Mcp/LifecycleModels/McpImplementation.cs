using System.Text.Json.Serialization;

namespace Aldaman.Services.Dtos.Mcp.LifecycleModels;

/// <summary>
/// Represents the implementation details (name and version) of an MCP client or server.
/// </summary>
public class McpImplementation
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;
}
