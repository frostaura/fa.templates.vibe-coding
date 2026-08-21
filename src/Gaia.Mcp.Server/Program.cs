using Gaia.Mcp.Server.Models;
using Gaia.Mcp.Server.Storage;
using Gaia.Mcp.Server.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.AspNetCore;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

var dataDir = Environment.GetEnvironmentVariable("GAIA_DATA_DIR")
    ?? Path.Combine(AppContext.BaseDirectory, "data");

var store = new JsonTaskStore(dataDir);
var tasksTool = new TasksTool(store);

var memoryStore = new ThreadSafeJsonStore<MemoryItem>(dataDir, ".memory.json");
var memoryTool = new MemoryTool(memoryStore);

var evolutionStore = new ThreadSafeJsonStore<EvolutionItem>(dataDir, ".evolutions.json");
var evolveTool = new EvolveTool(evolutionStore);

var serverInfo = new Implementation
{
    Name = "gaia-mcp",
    Title = "Gaia MCP Server",
    Version = "11.0.0"
};

var transport = Environment.GetEnvironmentVariable("MCP_TRANSPORT");
if (string.Equals(transport, "stdio", StringComparison.OrdinalIgnoreCase))
{
    await RunStdioServerAsync(args);
    return;
}

var builder = WebApplication.CreateBuilder(args);

ConfigureMcp(builder.Services.AddMcpServer(options => options.ServerInfo = serverInfo))
    // Hybrid session mode: stateless for 2026-07-28 discovery clients, a stateful
    // session for legacy `initialize` clients so server-initiated sampling still works.
    .WithHttpTransport(options => options.SessionMode = HttpServerSessionMode.StatefulForInitializeClients);

var app = builder.Build();

app.MapMcp("/mcp");

app.Run();

async Task RunStdioServerAsync(string[] hostArgs)
{
    var hostBuilder = Host.CreateApplicationBuilder(hostArgs);

    // Keep stdout reserved for the MCP stdio protocol; diagnostics go to stderr.
    hostBuilder.Logging.ClearProviders();
    hostBuilder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

    ConfigureMcp(hostBuilder.Services.AddMcpServer(options => options.ServerInfo = serverInfo))
        .WithStdioServerTransport();

    var host = hostBuilder.Build();
    await host.RunAsync();
}

IMcpServerBuilder ConfigureMcp(IMcpServerBuilder mcp) => mcp
    .WithTools(tasksTool)
    .WithTools(memoryTool)
    .WithTools(evolveTool)
    // Picks up the [McpServerToolType] example classes (echo/progress + sampling demos).
    .WithToolsFromAssembly();
