using FrostAura.MCP.Gaia;
using FrostAura.MCP.Gaia.Data;
using FrostAura.MCP.Gaia.Interfaces;
using FrostAura.MCP.Gaia.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

// Normal MCP server execution
var builder = Host.CreateApplicationBuilder(args);

// Configure logging to use stderr for MCP compliance
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Add configuration - embedded to avoid file loading issues
builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Application:Name"] = "fa.mcp.gaia",
    ["Application:Version"] = "1.0.0",
    ["TaskPlanner:DatabasePath"] = ".github/state/Gaia.TaskPlanner.db.json",
    ["Logging:LogLevel:Default"] = "Warning",
    ["Logging:LogLevel:Microsoft.Hosting.Lifetime"] = "Warning",
    ["Logging:LogLevel:ModelContextProtocol"] = "Warning"
});

// Register Task Services
builder.Services.AddScoped<TaskPlannerDbContext>();
builder.Services.AddScoped<ITaskPlannerRepository, TaskPlannerRepository>();

// Register Managers (now includes MCP tools)
builder.Services.AddScoped<ITaskPlannerManager, TaskPlannerManager>();
builder.Services.AddScoped<ILocalMachineManager, LocalMachineManager>();

// Configure MCP Server
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

var host = builder.Build();

// Start the host directly
await host.RunAsync();
