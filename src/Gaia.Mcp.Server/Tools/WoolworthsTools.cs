using System.ComponentModel;
using Gaia.Mcp.Server.Integrations;
using Gaia.Mcp.Server.Integrations.Woolworths;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace Gaia.Mcp.Server.Tools;

/// <summary>
/// MCP tools for the Woolworths integration.
///
/// These are thin wrappers over the same services the REST API under /api uses,
/// so both surfaces behave identically. Credentials come from the request headers
/// the MCP client is configured with, or from the environment when the server is
/// running over stdio — see <see cref="CredentialAccessor"/>.
/// </summary>
[McpServerToolType]
public sealed class WoolworthsTools(
    WoolworthsSearchClient search,
    WoolworthsCartService cart,
    CredentialAccessor credentials,
    WoolworthsIntegration integration)
{
    [McpServerTool(Name = "woolworths_search_products")]
    [Description("Search the Woolworths South Africa grocery catalogue. Needs no credentials. "
        + "Returns products with their id, name and price in ZAR; the id can be passed to woolworths_add_product_to_cart.")]
    public async Task<object> SearchProductsAsync(
        [Description("What to search for, e.g. 'baby spinach' or 'fusilli pasta'.")] string query,
        [Description("Maximum products to return. Defaults to 5.")] int limit = 5,
        CancellationToken ct = default)
    {
        var products = await search.SearchAsync(query, Math.Clamp(limit, 1, 24), ct);
        return new
        {
            query,
            count = products.Count,
            products = products.Select(p => new { p.Id, p.Name, priceZar = p.Price, p.Url })
        };
    }

    [McpServerTool(Name = "woolworths_preview_shopping_list")]
    [Description("Resolve a shared shopping list (such as the text shared from the Cookidoo/Thermomix app) "
        + "to Woolworths products WITHOUT signing in or changing the cart. Needs no credentials. "
        + "Use this to check the matches before committing to woolworths_add_shopping_list_to_cart.")]
    public async Task<object> PreviewShoppingListAsync(
        [Description("The raw shopping list text, one ingredient per line. Section headings and quantities are handled.")]
        string shoppingListText,
        CancellationToken ct = default)
    {
        var result = await cart.PreviewAsync(shoppingListText, ct);
        return Describe(result);
    }

    [McpServerTool(Name = "woolworths_add_shopping_list_to_cart")]
    [Description("Parse a shared shopping list, match every ingredient to a Woolworths product, and add them "
        + "to the customer's cart. Requires the Woolworths credential headers to be configured on the MCP server. "
        + "Items are added individually and unavailable products fall back to the next best match. "
        + "Returns what was added, what failed, and the cart URL to open.")]
    public async Task<object> AddShoppingListToCartAsync(
        [Description("The raw shopping list text, one ingredient per line.")] string shoppingListText,
        CancellationToken ct = default)
    {
        var creds = RequireCredentials();
        var result = await TranslateAsync(() => cart.AddShoppingListAsync(shoppingListText, creds, ct));
        return Describe(result);
    }

    [McpServerTool(Name = "woolworths_add_product_to_cart")]
    [Description("Add one specific Woolworths product to the customer's cart by its product id, as returned "
        + "by woolworths_search_products. Requires the Woolworths credential headers.")]
    public async Task<object> AddProductAsync(
        [Description("The Woolworths product id.")] string productId,
        [Description("How many units to add. Defaults to 1.")] int quantity = 1,
        CancellationToken ct = default)
    {
        var creds = RequireCredentials();
        var outcome = await TranslateAsync(() => cart.AddProductAsync(productId, quantity, creds, ct));

        return new
        {
            added = outcome.Success,
            reason = outcome.Reason,
            cartItemCount = outcome.CartCount,
            cartTotalZar = outcome.CartTotal,
            cartUrl = WoolworthsClient.CartUrl
        };
    }

    /// <summary>
    /// Resolve credentials, converting a missing-header failure into an
    /// <see cref="McpException"/>.
    ///
    /// This matters: the SDK replaces arbitrary exception text with a generic
    /// "an error occurred" so servers cannot leak internals, but an MCP client
    /// that has simply not been configured with the credential headers needs
    /// to be told exactly that. McpException messages are passed through.
    /// </summary>
    private IntegrationCredentials RequireCredentials()
    {
        try
        {
            return credentials.Resolve(integration);
        }
        catch (MissingCredentialsException ex)
        {
            throw new McpException(
                $"{ex.Message} Set them on the MCP server connection — as HTTP headers over "
                + "streamable HTTP, or in the server process environment over stdio.");
        }
    }

    /// <summary>
    /// Run an operation that signs in, converting a provider rejection into an
    /// <see cref="McpException"/>.
    ///
    /// Same reason as <see cref="RequireCredentials"/>: the SDK replaces arbitrary
    /// exception text with a generic "an error occurred", so a caller whose password
    /// is simply wrong would otherwise be told nothing at all — and a wrong password
    /// is at least as common as an unconfigured one.
    /// </summary>
    private static async Task<T> TranslateAsync<T>(Func<Task<T>> operation)
    {
        try
        {
            return await operation();
        }
        catch (ProviderAuthenticationException ex)
        {
            throw new McpException($"Woolworths rejected the credentials: {ex.Message}");
        }
    }

    private static object Describe(CartBuildResult result) => new
    {
        addedCount = result.AddedCount,
        totalIngredients = result.Lines.Count,
        cartItemCount = result.CartItemCount,
        cartTotalZar = result.CartTotal,
        cartUrl = result.CartUrl,
        lines = result.Lines.Select(l => new
        {
            ingredient = l.Ingredient,
            status = l.Status.ToString(),
            matchedProduct = l.Product?.Name,
            productId = l.Product?.Id,
            priceZar = l.Product?.Price,
            searchQuery = l.Query,
            detail = l.Detail,
            alternatives = l.Alternatives.Take(3).Select(a => new { a.Id, a.Name, priceZar = a.Price })
        })
    };
}
