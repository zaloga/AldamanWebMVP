using System.Text.Json.Serialization;

namespace Aldaman.Services.Dtos.Mcp.ToolModels;

/// <summary>
/// Represents a content item in an MCP response (e.g., text, image).
/// </summary>
public class McpContent
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "text";

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}
