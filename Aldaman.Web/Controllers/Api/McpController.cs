using System.Text.Json;
using Aldaman.Services.Dtos.Mcp.RpcModels;
using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public sealed class McpController : ControllerBase
{
    private IMcpSessionManager SessionManager { get; }
    private IMcpMessageHandlerService McpMessageHandler { get; }
    private ILogger<McpController> Logger { get; }

    public McpController(
        IMcpSessionManager sessionManager,
        IMcpMessageHandlerService mcpMessageHandler,
        ILogger<McpController> logger)
    {
        SessionManager = sessionManager;
        McpMessageHandler = mcpMessageHandler;
        Logger = logger;
    }

    /// <summary>
    /// Establishes an SSE (Server-Sent Events) connection for MCP. 
    /// This provides a long-running channel for the server to send asynchronous messages and notifications back to the client.
    /// </summary>
    [HttpGet("sse")]
    public async Task Sse()
    {
        var sessionId = SessionManager.CreateSession();
        var channel = SessionManager.GetChannel(sessionId);

        if (channel == null)
        {
            Response.StatusCode = 500;
            return;
        }

        Response.Headers.Append("Content-Type", "text/event-stream"); // Required for SSE
        Response.Headers.Append("Cache-Control", "no-cache"); // Prevents caching of the stream
        Response.Headers.Append("Connection", "keep-alive"); // Keeps the connection open
        Response.Headers.Append("X-Accel-Buffering", "no"); // Disables buffering in proxy servers (e.g. Nginx) to avoid delays in message delivery

        // The endpoint URL that the client should use for POST messages
        var messageUrl = Url.Action("Message", "Mcp", new { sessionId }, Request.Scheme);

        // Send the endpoint event as per MCP spec
        await Response.WriteAsync($"event: endpoint\ndata: {messageUrl}\n\n");
        await Response.Body.FlushAsync();

        try
        {
            await foreach (var message in channel.Reader.ReadAllAsync(HttpContext.RequestAborted))
            {
                await Response.WriteAsync($"event: message\ndata: {message}\n\n");
                await Response.Body.FlushAsync();
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when client disconnects
            Logger.LogInformation("MCP SSE client disconnected: {SessionId}", sessionId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in MCP SSE stream for session {SessionId}", sessionId);
        }
        finally
        {
            SessionManager.RemoveSession(sessionId);
        }
    }

    /// <summary>
    /// Receives JSON-RPC messages from the client.
    /// </summary>
    [HttpPost("message")]
    public async Task<IActionResult> Message([FromQuery] string sessionId, [FromBody] JsonRpcRequest request)
    {
        if (string.IsNullOrEmpty(sessionId))
        {
            return BadRequest("SessionId is required.");
        }

        var channel = SessionManager.GetChannel(sessionId);
        if (channel == null)
        {
            return NotFound("Session not found or expired.");
        }

        var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        var response = await McpMessageHandler.HandleRequestAsync(request, baseUrl, HttpContext.RequestAborted);

        if (response != null)
        {
            // For MCP over SSE, the response to a request is sent back through the SSE stream,
            // while the POST request itself just returns 202 Accepted (or 200 OK).
            // Official MCP spec says: "The server SHOULD send the response message as a 'message' event on the SSE stream."

            var jsonResponse = JsonSerializer.Serialize(response);
            await SessionManager.SendMessageAsync(sessionId, jsonResponse);
        }

        return Accepted();
    }
}
