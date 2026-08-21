using System.Globalization;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;

namespace Gaia.Mcp.Server.Integrations.Woolworths;

/// <summary>
/// Product search for Woolworths SA.
///
/// Search is not served by Woolworths itself — the storefront delegates it to
/// Constructor.io using a public client key embedded in the page. That means
/// searching needs no cookies, no sign-in and no credentials, so it is safe to
/// expose anonymously.
/// </summary>
public sealed class WoolworthsSearchClient(HttpClient http, ILogger<WoolworthsSearchClient> log)
{
    /// <summary>Public client key lifted from the Woolworths storefront bundle.</summary>
    private const string ApiKey = "key_tw9hKe0fkfgEf36D";
    private const string SearchBase = "https://wpkmgeuco-zone.cnstrc.com/v1/search";

    public async Task<IReadOnlyList<Product>> SearchAsync(string query, int limit, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query)) return [];

        // The visibility filter MUST be sent twice, exactly as the storefront
        // does. Sending only "web and app" returns zero FOOD results while
        // still returning non-food, which looks convincingly like a
        // location-gated catalogue and is not: "baby spinach" comes back empty
        // and "garlic" returns garlic presses from homeware.
        var url =
            $"{SearchBase}/{Uri.EscapeDataString(query)}" +
            $"?key={ApiKey}" +
            "&filters%5Bvisibility%5D=all" +
            "&filters%5Bvisibility%5D=web+and+app" +
            "&us=default" +
            $"&num_results_per_page={limit}&page=1&s=1" +
            $"&i={Guid.NewGuid()}";

        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.TryAddWithoutValidation("user-agent",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 " +
            "(KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36");

        using var res = await http.SendAsync(req, ct);
        if (!res.IsSuccessStatusCode)
        {
            log.LogWarning("Woolworths search for {Query} returned {Status}", query, (int)res.StatusCode);
            return [];
        }

        var json = JsonNode.Parse(await res.Content.ReadAsStringAsync(ct));
        if (json?["response"]?["results"] is not JsonArray results) return [];

        var products = new List<Product>(results.Count);
        foreach (var node in results)
        {
            var data = node?["data"];

            // data.id is the same identifier the cart expects as catalogRefId.
            // That equivalence is what lets search feed add-to-cart directly.
            var id = data?["id"]?.GetValue<string>();
            var name = node?["value"]?.GetValue<string>();
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name)) continue;

            // p10 is the shelf price. It arrives as a JSON number but is
            // occasionally a string, so parse defensively — and with the
            // invariant culture, since a comma-decimal host locale would
            // otherwise silently drop every price.
            decimal? price = null;
            if (data?["p10"] is JsonNode p &&
                decimal.TryParse(p.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
            {
                price = parsed;
            }

            var path = data?["url"]?.GetValue<string>();
            products.Add(new Product(id, name, price,
                path is null ? null : $"https://www.woolworths.co.za/{path.TrimStart('/')}"));
        }

        return products;
    }

    /// <summary>
    /// Resolve an ingredient to ranked candidates, widening the phrasing until
    /// something matches. Returns the query that worked so callers can explain
    /// (or tune) the match.
    /// </summary>
    public async Task<(string? Query, IReadOnlyList<Product> Candidates)> ResolveIngredientAsync(
        string ingredient, int limit, CancellationToken ct)
    {
        foreach (var query in ShoppingListParser.ToQueries(ingredient))
        {
            var products = await SearchAsync(query, limit, ct);
            if (products.Count > 0) return (query, products);
        }

        return (null, []);
    }
}
