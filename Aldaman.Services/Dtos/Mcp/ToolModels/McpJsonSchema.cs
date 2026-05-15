using System.Text.Json.Serialization;

namespace Aldaman.Services.Dtos.Mcp.ToolModels;

/// <summary>
/// Represents a simplified JSON Schema used for MCP tool input definitions.
/// </summary>
public class McpJsonSchema
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; } = new();

    [JsonPropertyName("required")]
    public List<string> Required { get; set; } = new();
}
