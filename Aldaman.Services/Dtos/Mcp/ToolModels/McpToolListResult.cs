using System.Text.Json.Serialization;

namespace Aldaman.Services.Dtos.Mcp.ToolModels;

/// <summary>
/// Represents the result of a 'tools/list' request in the MCP protocol.
/// </summary>
public class McpToolListResult
{
    [JsonPropertyName("tools")]
    public List<McpTool> Tools { get; set; } = new();
}
