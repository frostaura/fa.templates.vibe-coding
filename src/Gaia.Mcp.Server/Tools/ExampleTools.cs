using System.ComponentModel;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace Gaia.Mcp.Server.Tools;

/// <summary>
/// Example MCP tools demonstrating tool creation, progress notifications, and usage.
/// </summary>
[McpServerToolType]
public static class ExampleTools
{
    /// <summary>
    /// Echoes back a request. This is an example tool, used for systems testing.
    /// </summary>
    [McpServerTool(Name = "example_echo", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Echoes back a request. This is an example tool, used for systems testing.")]
    public static Task<string> EchoAsync(
        [Description("The message to echo back.")] string message,
        CancellationToken cancellationToken)
        => Task.FromResult(message);

    /// <summary>
    /// Stream back an echo of the request message, one letter at a time, reporting
    /// incremental progress notifications to the client as it goes.
    /// </summary>
    [McpServerTool(Name = "example_echo_by_letter", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description(
        "Stream back an echo of the request message, one letter at a time. Emits one notifications/progress " +
        "per character (progress/total/message) when the client supplies a progress token — use it to verify " +
        "progress plumbing end to end. This is an example tool, used for systems testing.")]
    public static async Task<CallToolResult> EchoByLetterAsync(
        [Description("The message to echo back.")] string message,
        IProgress<ProgressNotificationValue> progress,
        CancellationToken cancellationToken)
    {
        var content = new List<ContentBlock>(message.Length);

        for (var index = 0; index < message.Length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Task.Delay(150, cancellationToken);

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
