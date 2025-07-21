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

// Add configuration - embedded to avoid file loading issues
builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Application:Name"] = "fa.mcp.gaia",
    ["Application:Version"] = "1.0.0",
    ["TreePlanner:DatabasePath"] = ".github/state/Gaia.TreePlanner.db.json",
    ["Logging:LogLevel:Default"] = "Critical",
    ["Logging:LogLevel:Microsoft"] = "Critical",
    ["Logging:LogLevel:FrostAura.Gaia.Data"] = "Critical"
});

// Configure logging from appsettings.json
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
// Remove console logging to avoid interfering with MCP JSON-RPC protocol
// builder.Logging.AddConsole(consoleLogOptions =>
// {
//     consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Information;
// });

// Register TODO Services
builder.Services.AddScoped<TreePlannerDbContext>();
builder.Services.AddScoped<ITreePlannerRepository, TreePlannerRepository>();

// Register Managers (now includes MCP tools)
builder.Services.AddScoped<ITreePlannerManager, TreePlannerManager>();

// Configure MCP Server
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

var host = builder.Build();

// Start the host directly without logging startup info
await host.RunAsync();
