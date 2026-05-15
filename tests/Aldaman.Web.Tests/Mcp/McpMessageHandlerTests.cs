using System.Text.Json;
using Aldaman.Services.Configuration;
using Aldaman.Services.Dtos.Mcp.LifecycleModels;
using Aldaman.Services.Dtos.Mcp.RpcModels;
using Aldaman.Services.Dtos.Mcp.ToolModels;
using Aldaman.Services.Interfaces;
using Aldaman.Services.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Aldaman.Web.Tests.Mcp;

/// <summary>
/// Unit tests for <see cref="McpMessageHandlerService"/>.
/// </summary>
public class McpMessageHandlerTests
{
    private readonly Mock<ISearchService> _searchServiceMock;
    private readonly Mock<ILogger<McpMessageHandlerService>> _loggerMock;
    private readonly McpMessageHandlerService _handler;

    public McpMessageHandlerTests()
    {
        _searchServiceMock = new Mock<ISearchService>();
        _loggerMock = new Mock<ILogger<McpMessageHandlerService>>();

        var options = Options.Create(new McpSettings());
        _handler = new McpMessageHandlerService(_searchServiceMock.Object, options, _loggerMock.Object);
    }

    [Fact]
    public async Task HandleRequest_Initialize_ReturnsCorrectResult()
    {
        // Arrange
        var request = new JsonRpcRequest
        {
            Id = 1,
            Method = "initialize",
            Params = new { protocolVersion = "2024-11-05" }
        };

        // Act
        var response = await _handler.HandleRequestAsync(request, "http://localhost");

        // Assert
        Assert.Null(response.Error);
        Assert.NotNull(response.Result);
        var result = Assert.IsType<McpInitializeResult>(response.Result);
        Assert.Equal("2024-11-05", result.ProtocolVersion);
        Assert.Equal(1, (int)response.Id!);
    }

    [Fact]
    public async Task HandleRequest_ToolsList_ReturnsSearchTool()
    {
        // Arrange
        var request = new JsonRpcRequest
        {
            Id = "abc",
            Method = "tools/list"
        };

        // Act
        var response = await _handler.HandleRequestAsync(request, "http://localhost");

        // Assert
        Assert.Null(response.Error);
        var result = Assert.IsType<McpToolListResult>(response.Result);
        Assert.Contains(result.Tools, t => t.Name == "search");
    }

    [Fact]
    public async Task HandleRequest_ToolsCall_Search_CallsSearchService()
    {
        // Arrange
        var callParams = new McpCallToolParams
        {
            Name = "search",
            Arguments = new Dictionary<string, object> { { "query", "test" } }
        };

        // Wrap in JsonElement because the service expects it (deserialized from object by framework)
        var json = JsonSerializer.Serialize(callParams);
        var jsonElement = JsonDocument.Parse(json).RootElement;

        var request = new JsonRpcRequest
        {
            Id = 123,
            Method = "tools/call",
            Params = jsonElement
        };

        _searchServiceMock.Setup(s => s.SearchAsync("test", "cs", "http://localhost", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Aldaman.Services.Dtos.Search.SearchResultDto>
            {
                new() { Title = "Result 1", Content = "Content", Url = "/url", Type = "Post" }
            });

        // Act
        var response = await _handler.HandleRequestAsync(request, "http://localhost");

        // Assert
        Assert.Null(response.Error);
        var result = Assert.IsType<McpCallToolResult>(response.Result);
        Assert.Single(result.Content);
        Assert.Contains("Result 1", result.Content[0].Text);
        _searchServiceMock.Verify(s => s.SearchAsync("test", "cs", "http://localhost", It.IsAny<CancellationToken>()), Times.Once);
    }
}
