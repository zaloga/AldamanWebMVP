using System.Text.Json.Serialization;

namespace Aldaman.Services.Dtos.Mcp.ToolModels;

/// <summary>
/// Represents the result of a 'tools/call' request in the MCP protocol.
/// </summary>
public class McpCallToolResult
{
    [JsonPropertyName("content")]
    public List<McpContent> Content { get; set; } = new();

    [JsonPropertyName("isError")]
    public bool IsError { get; set; }
}
