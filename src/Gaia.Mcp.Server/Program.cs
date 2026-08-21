using Gaia.Mcp.Server.Integrations;
using Gaia.Mcp.Server.Integrations.Woolworths;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.AspNetCore;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

var serverInfo = new Implementation
{
    Name = "gaia-mcp",
    Title = "Gaia MCP Server",
    Version = "13.0.0"
};

var transport = Environment.GetEnvironmentVariable("MCP_TRANSPORT");
if (string.Equals(transport, "stdio", StringComparison.OrdinalIgnoreCase))
{
    await RunStdioServerAsync(args);
    return;
}

var builder = WebApplication.CreateBuilder(args);

AddIntegrationServices(builder.Services);
builder.Services.AddProblemDetails();

ConfigureMcp(builder.Services.AddMcpServer(options => options.ServerInfo = serverInfo))
    // Hybrid session mode: stateless for 2026-07-28 discovery clients, a stateful
    // session for legacy `initialize` clients so server-initiated sampling still works.
    // Integration tools read their credentials from the inbound request headers, so
    // whatever the mode, a tool call must still execute inside the caller's HTTP request.
    .WithHttpTransport(options => options.SessionMode = HttpServerSessionMode.StatefulForInitializeClients);

var app = builder.Build();

// Translate domain failures into honest status codes. Without this a missing
// header is an opaque 500 and the caller has no idea what to fix.
app.UseExceptionHandler(handler => handler.Run(async context =>
{
    var error = context.Features
        .Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;

    var (status, title) = error switch
    {
        MissingCredentialsException => (StatusCodes.Status401Unauthorized, "Missing credentials"),
        ProviderAuthenticationException => (StatusCodes.Status401Unauthorized, "Provider rejected the credentials"),
        TaskCanceledException or OperationCanceledException => (StatusCodes.Status504GatewayTimeout, "Upstream timed out"),
        HttpRequestException => (StatusCodes.Status502BadGateway, "Upstream request failed"),
        _ => (StatusCodes.Status500InternalServerError, "Unexpected error")
    };

    context.Response.StatusCode = status;
    await context.Response.WriteAsJsonAsync(new
    {
        error = title,
        // Safe to surface: these messages describe the caller's own request,
        // never provider internals.
        detail = error is MissingCredentialsException or ProviderAuthenticationException
            ? error.Message
            : "See server logs for detail."
    });
}));

app.MapGet("/", () => Results.Redirect("/api/integrations"));

app.MapGet("/api/health", () => Results.Ok(new { status = "healthy" }))
   .WithName("Health");

// Discovery: what can this service drive, and what must a caller supply?
// Both humans and MCP clients start here.
app.MapGet("/api/integrations", (IEnumerable<IIntegration> integrations) => Results.Ok(new
{
    integrations = integrations.Select(i => new
    {
        i.Key,
        i.DisplayName,
        i.Description,
        credentials = i.Credentials.Select(c => new
        {
            c.Key,
            header = c.Header,
            environmentVariable = CredentialAccessor.EnvironmentVariableFor(c),
            c.Description,
            c.Required,
            c.Secret
        })
    })
}));

var woolworths = app.MapGroup("/api/woolworths");

// Anonymous: search needs no account.
woolworths.MapGet("/search", async (
    string q, int? limit, WoolworthsSearchClient search, CancellationToken ct) =>
{
    var products = await search.SearchAsync(q, Math.Clamp(limit ?? 5, 1, 24), ct);
    return Results.Ok(new { query = q, count = products.Count, products });
});

// Anonymous: resolve a list without touching a cart.
woolworths.MapPost("/shopping-list/preview", async (
    ShoppingListRequest request, WoolworthsCartService cart, CancellationToken ct) =>
{
    var result = await cart.PreviewAsync(request.Text, ct);
    return Results.Ok(result);
});

// Authenticated by the caller's own Woolworths credentials, via headers.
woolworths.MapPost("/shopping-list/cart", async (
    ShoppingListRequest request,
    WoolworthsCartService cart,
    CredentialAccessor accessor,
    WoolworthsIntegration integration,
    CancellationToken ct) =>
{
    var credentials = accessor.Resolve(integration);
    var result = await cart.AddShoppingListAsync(request.Text, credentials, ct);
    return Results.Ok(result);
});

woolworths.MapPost("/cart/items", async (
    AddProductRequest request,
    WoolworthsCartService cart,
    CredentialAccessor accessor,
    WoolworthsIntegration integration,
    CancellationToken ct) =>
{
    var outcome = await cart.AddProductAsync(
        request.ProductId, request.Quantity ?? 1, accessor.Resolve(integration), ct);
    return Results.Ok(outcome);
});

app.MapMcp("/mcp");

app.Run();

async Task RunStdioServerAsync(string[] hostArgs)
{
    var hostBuilder = Host.CreateApplicationBuilder(hostArgs);

    // Keep stdout reserved for the MCP stdio protocol; diagnostics go to stderr.
    hostBuilder.Logging.ClearProviders();
    hostBuilder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

    AddIntegrationServices(hostBuilder.Services);

    ConfigureMcp(hostBuilder.Services.AddMcpServer(options => options.ServerInfo = serverInfo))
        .WithStdioServerTransport();

    var host = hostBuilder.Build();
    await host.RunAsync();
}

// Integrations register themselves as IIntegration so discovery and credential
// resolution pick them up without a central list to keep in sync. Adding an
// integration is three lines here and a class; there is no registry to update.
void AddIntegrationServices(IServiceCollection services)
{
    services.AddHttpContextAccessor();
    services.AddSingleton<CredentialAccessor>();

    services.AddSingleton<WoolworthsIntegration>();
    services.AddSingleton<IIntegration>(sp => sp.GetRequiredService<WoolworthsIntegration>());
    services.AddHttpClient<WoolworthsSearchClient>();
    services.AddSingleton<WoolworthsCartService>();
}

IMcpServerBuilder ConfigureMcp(IMcpServerBuilder mcp) => mcp
    // Picks up every [McpServerToolType] in the assembly: the Woolworths
    // integration tools plus the echo/progress and sampling examples.
    .WithToolsFromAssembly();

/// <param name="Text">Raw shared shopping-list text.</param>
public sealed record ShoppingListRequest(string Text);

/// <param name="ProductId">A Woolworths product id from a search result.</param>
public sealed record AddProductRequest(string ProductId, int? Quantity);

/// <summary>Exposed so integration tests can spin the host up in-memory.</summary>
public partial class Program;
