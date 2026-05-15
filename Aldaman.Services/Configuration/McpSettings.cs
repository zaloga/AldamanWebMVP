namespace Aldaman.Services.Configuration;

/// <summary>
/// Settings for Model Context Protocol (MCP) server.
/// </summary>
public sealed class McpSettings
{
    public const string SectionName = "Mcp";

    public string ServerName { get; set; } = "Aldaman CMS MCP Server";
    public string ServerVersion { get; set; } = "1.0.0";
}
