using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace Gaia.Mcp.Server.Tools;

/// <summary>
/// Example MCP tools demonstrating tool creation and usage.
/// </summary>
[McpServerToolType]
public class ExampleTools
{
    private readonly ILogger<ExampleTools> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExampleTools"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public ExampleTools(ILogger<ExampleTools> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Echoes back a request. This is an example tool, used for systems testing.
    /// </summary>
    /// <param name="message">The message to echo back.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The same as the request message.</returns>
    [McpServerTool]
    [Description("Echoes back a request. This is an example tool, used for systems testing.")]
    public Task<string> EchoAsync(
        [Description("The message to echo back.")] string message,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Echoing back message: {Message}", message);

        return Task.FromResult(message);
    }

    /// <summary>
    /// Echoes back a request that passes an auth context. This is an example tool, used for systems testing.
    /// </summary>
    /// <param name="authContext">The mandatory auth credentials.</param>
    /// <param name="message">The message to echo back.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The same as the request message.</returns>
    // [McpServerTool]
    // [Description("Echoes back a request that passes an auth context. This is an example tool, used for systems testing.")]
    // public Task<string> EchoWithAuthAsync(
    //     [Description("The mandatory auth credentials.")] ToolAuthRequest authContext,
    //     [Description("The message to echo back.")] string message,
    //     CancellationToken cancellationToken)
    // {
    //     _logger.LogInformation("Echoing back message with auth: {Message}, AuthContext: {AuthContext}", message, authContext);

    //     return Task.FromResult(JsonSerializer.Serialize(new { AuthContext = authContext, Message = message }));
    // }

    /// <summary>
    /// Stream back an echo of the request message, one letter at a time.
    /// </summary>
    /// <param name="message">The message to echo back.</param>
    /// <param name="progress">The progress sink for incremental character updates.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The request message as one text content block per character.</returns>
    [McpServerTool]
    [Description("Stream back an echo of the request message, one letter at a time. Emits incremental progress notifications when the client supplies a progress token. This is an example tool, used for systems testing.")]
    public async Task<CallToolResult> EchoByLetterAsync(
        [Description("The message to echo back.")] string message,
        IProgress<ProgressNotificationValue> progress,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Echoing back message letter by letter: {Message}", message);

        var content = new List<ContentBlock>(message.Length);

        for (var index = 0; index < message.Length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Delay(1000, cancellationToken);

            var letter = message[index].ToString();
            content.Add(new TextContentBlock
            {
                Text = letter,
            });

            progress.Report(new ProgressNotificationValue
            {
                Progress = index + 1,
                Total = message.Length,
                Message = letter,
            });
        }

        return new CallToolResult
        {
            Content = content,
            IsError = false,
        };
    }
}
