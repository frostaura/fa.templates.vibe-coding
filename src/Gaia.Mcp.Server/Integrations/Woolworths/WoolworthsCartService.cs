using Microsoft.Extensions.Logging;

namespace Gaia.Mcp.Server.Integrations.Woolworths;

/// <summary>
/// Turns a shared shopping list into a filled Woolworths cart.
///
/// Each call owns a complete session: sign in, do the work, sign out. Sessions
/// are never pooled or cached, so no caller's cookies can leak into another's
/// request and the service holds no logged-in state between calls.
/// </summary>
public sealed class WoolworthsCartService(
    WoolworthsSearchClient search,
    ILoggerFactory loggerFactory,
    ILogger<WoolworthsCartService> log)
{
    /// <summary>How many candidates to consider per ingredient before giving up.</summary>
    private const int CandidateDepth = 5;

    /// <summary>
    /// Resolve every ingredient without signing in or touching a cart.
    /// Needs no credentials, which makes it a safe preview.
    /// </summary>
    public async Task<CartBuildResult> PreviewAsync(string shoppingListText, CancellationToken ct)
    {
        var items = ShoppingListParser.Parse(shoppingListText);
        var lines = new List<CartLineResult>(items.Count);

        foreach (var item in items)
        {
            var (query, candidates) = await search.ResolveIngredientAsync(item.Name, CandidateDepth, ct);

            lines.Add(candidates.Count == 0
                ? new CartLineResult(item.Name, null, null, [], CartLineStatus.NoMatch, "No matching product found.")
                : new CartLineResult(item.Name, candidates[0], query, [.. candidates.Skip(1)],
                    CartLineStatus.Resolved, null));
        }

        return new CartBuildResult(lines, 0, null, WoolworthsClient.CartUrl);
    }

    /// <summary>Add one known product id to the caller's cart.</summary>
    public async Task<AddItemOutcome> AddProductAsync(
        string productId, int quantity, IntegrationCredentials credentials, CancellationToken ct)
    {
        using var client = new WoolworthsClient(loggerFactory.CreateLogger<WoolworthsClient>());
        await client.SignInAsync(credentials["username"], credentials["password"], ct);

        try
        {
            return await client.AddToCartAsync(productId, Math.Max(1, quantity), ct);
        }
        finally
        {
            await client.SignOutAsync(ct);
        }
    }

    /// <summary>
    /// Resolve every ingredient and add it to the caller's cart.
    ///
    /// Items are added one at a time on purpose: the upstream batch endpoint
    /// discards an entire batch if any single product is unavailable, and
    /// reports success while doing it.
    /// </summary>
    public async Task<CartBuildResult> AddShoppingListAsync(
        string shoppingListText, IntegrationCredentials credentials, CancellationToken ct)
    {
        var items = ShoppingListParser.Parse(shoppingListText);
        var lines = new List<CartLineResult>(items.Count);

        using var client = new WoolworthsClient(loggerFactory.CreateLogger<WoolworthsClient>());
        await client.SignInAsync(credentials["username"], credentials["password"], ct);

        try
        {
            int? cartCount = null;
            decimal? cartTotal = null;

            foreach (var item in items)
            {
                var (query, candidates) = await search.ResolveIngredientAsync(item.Name, CandidateDepth, ct);

                if (candidates.Count == 0)
                {
                    lines.Add(new CartLineResult(item.Name, null, null, [],
                        CartLineStatus.NoMatch, "No matching product found."));
                    continue;
                }

                // Walk the candidates until one is actually purchasable — the
                // best-named match is regularly out of stock.
                CartLineResult? resolved = null;
                string? lastReason = null;

                for (var i = 0; i < candidates.Count; i++)
                {
                    var candidate = candidates[i];
                    var outcome = await client.AddToCartAsync(candidate.Id, 1, ct);

                    if (outcome.Success)
                    {
                        cartCount = outcome.CartCount ?? cartCount;
                        cartTotal = outcome.CartTotal ?? cartTotal;

                        var alternatives = candidates.Where((_, idx) => idx != i).ToList();
                        resolved = new CartLineResult(item.Name, candidate, query, alternatives,
                            CartLineStatus.Added, null);
                        break;
                    }

                    lastReason = outcome.Reason;
                }

                lines.Add(resolved ?? new CartLineResult(item.Name, candidates[0], query,
                    [.. candidates.Skip(1)], CartLineStatus.Unavailable,
                    lastReason ?? "No candidate could be added."));
            }

            // Trust the cart itself for the final numbers rather than the
            // running tally from the last add.
            var (finalCount, finalTotal) = await client.GetCartAsync(ct);

            log.LogInformation("Added {Added}/{Total} ingredients to the Woolworths cart",
                lines.Count(l => l.Status == CartLineStatus.Added), lines.Count);

            return new CartBuildResult(lines, finalCount ?? cartCount ?? 0,
                finalTotal ?? cartTotal, WoolworthsClient.CartUrl);
        }
        finally
        {
            // Always end the session, even if the run failed part-way.
            await client.SignOutAsync(ct);
        }
    }
}
