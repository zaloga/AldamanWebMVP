using System.Text.Json.Serialization;

namespace Aldaman.Services.Dtos.Mcp.ToolModels;

/// <summary>
/// Represents the parameters for a 'tools/call' request in the MCP protocol.
/// </summary>
public class McpCallToolParams
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("arguments")]
    public Dictionary<string, object>? Arguments { get; set; }
}
