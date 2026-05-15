using System.Text.Json.Serialization;

namespace Aldaman.Services.Dtos.Mcp.LifecycleModels;

/// <summary>
/// Represents the capabilities of an MCP server.
/// </summary>
public class McpServerCapabilities
{
    [JsonPropertyName("tools")]
    public object? Tools { get; set; } = new { }; // Indicates we support tools

    [JsonPropertyName("resources")]
    public object? Resources { get; set; }

    [JsonPropertyName("prompts")]
    public object? Prompts { get; set; }

    [JsonPropertyName("logging")]
    public object? Logging { get; set; }
}
