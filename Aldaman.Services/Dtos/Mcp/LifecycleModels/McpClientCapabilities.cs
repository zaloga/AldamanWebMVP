using System.Text.Json.Serialization;

namespace Aldaman.Services.Dtos.Mcp.LifecycleModels;

/// <summary>
/// Represents the capabilities of an MCP client.
/// </summary>
public class McpClientCapabilities
{
    // Simplified for now, can be extended
    [JsonPropertyName("roots")]
    public object? Roots { get; set; }

    [JsonPropertyName("sampling")]
    public object? Sampling { get; set; }
}
