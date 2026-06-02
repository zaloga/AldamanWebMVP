using System.Text.Json;
using Aldaman.Services.Configuration;
using Aldaman.Services.Dtos.Mcp.LifecycleModels;
using Aldaman.Services.Dtos.Mcp.RpcModels;
using Aldaman.Services.Dtos.Mcp.ToolModels;
using Aldaman.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aldaman.Services.Services;

public sealed class McpMessageHandlerService : IMcpMessageHandlerService
{
    private readonly ISearchService _searchService;
    private readonly McpSettings _settings;
    private readonly LocalizationSettings _localizationSettings;
    private readonly ILogger<McpMessageHandlerService> _logger;

    public McpMessageHandlerService(
        ISearchService searchService,
        IOptions<McpSettings> options,
        IOptions<LocalizationSettings> localizationOptions,
        ILogger<McpMessageHandlerService> logger)
    {
        _searchService = searchService;
        _settings = options.Value;
        _localizationSettings = localizationOptions.Value;
        _logger = logger;
    }

    public async Task<JsonRpcResponse?> HandleRequestAsync(JsonRpcRequest request, string baseUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            // According to JSON-RPC 2.0: "A Notification is a Request object without an 'id' member.
            // The Server MUST NOT reply to a Notification."
            bool isNotification = request.Id == null;

            JsonRpcResponse? response = request.Method switch
            {
                "initialize" => HandleInitialize(request),
                "notifications/initialized" => null, // This is a notification
                "ping" => new JsonRpcResponse { Id = request.Id, Result = new { } },
                "tools/list" => HandleToolsList(request),
                "tools/call" => await HandleToolsCallAsync(request, baseUrl, cancellationToken),
                _ => CreateErrorResponse(request.Id, -32601, $"Method not found: {request.Method}")
            };

            return isNotification ? null : response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling MCP request {Method}", request.Method);
            return CreateErrorResponse(request.Id, -32603, "Internal error");
        }
    }

    private JsonRpcResponse HandleInitialize(JsonRpcRequest request)
    {
        var result = new McpInitializeResult();
        result.ServerInfo.Name = _settings.ServerName;
        result.ServerInfo.Version = _settings.ServerVersion;

        return new JsonRpcResponse { Id = request.Id, Result = result };
    }

    private JsonRpcResponse HandleToolsList(JsonRpcRequest request)
    {
        var result = new McpToolListResult
        {
            Tools = new List<McpTool>
            {
                new McpTool
                {
                    Name = "search",
                    Description = "Searches for content in the website (blog posts, pages).",
                    InputSchema = new McpJsonSchema
                    {
                        Properties = new Dictionary<string, object>
                        {
                            { "query", new { type = "string", description = "The search term." } },
                            { "culture", new { type = "string", description = $"Culture code (cs, en). Defaults to {_localizationSettings.DefaultCulture}." } }
                        },
                        Required = new List<string> { "query" }
                    }
                }
            }
        };

        return new JsonRpcResponse { Id = request.Id, Result = result };
    }

    private async Task<JsonRpcResponse> HandleToolsCallAsync(JsonRpcRequest request, string baseUrl, CancellationToken cancellationToken)
    {
        if (request.Params is not JsonElement paramsElement)
        {
            return CreateErrorResponse(request.Id, -32602, "Invalid params");
        }

        var callParams = JsonSerializer.Deserialize<McpCallToolParams>(paramsElement.GetRawText());
        if (callParams == null)
        {
            return CreateErrorResponse(request.Id, -32602, "Invalid params");
        }

        if (callParams.Name == "search")
        {
            if (callParams.Arguments == null || !callParams.Arguments.TryGetValue("query", out var queryObj) || queryObj == null)
            {
                return CreateErrorResponse(request.Id, -32602, "Missing 'query' argument");
            }

            string query = queryObj.ToString()!;
            string culture = _localizationSettings.DefaultCulture;
            if (callParams.Arguments.TryGetValue("culture", out var cultureObj) && cultureObj != null)
            {
                culture = cultureObj.ToString()!;
            }

            var searchResults = await _searchService.SearchAsync(query, culture, baseUrl, cancellationToken);

            var result = new McpCallToolResult();
            foreach (var item in searchResults)
            {
                result.Content.Add(new McpContent
                {
                    Text = $"Title: {item.Title}\nType: {item.Type}\nURL: {item.Url}\nContent: {item.Content}\n---"
                });
            }

            if (result.Content.Count == 0)
            {
                result.Content.Add(new McpContent { Text = "No results found." });
            }

            return new JsonRpcResponse { Id = request.Id, Result = result };
        }

        return CreateErrorResponse(request.Id, -32601, $"Tool not found: {callParams.Name}");
    }

    private JsonRpcResponse CreateErrorResponse(object? id, int code, string message)
    {
        return new JsonRpcResponse
        {
            Id = id,
            Error = new JsonRpcError { Code = code, Message = message }
        };
    }
}
