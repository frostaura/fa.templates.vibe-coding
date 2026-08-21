using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;

namespace Gaia.Mcp.Server.Integrations.Woolworths;

/// <summary>
/// A signed-in Woolworths South Africa session.
///
/// Woolworths exposes no public API. Every endpoint here was established by
/// capturing real browser traffic (see docs/architecture). There is no bearer
/// token and no CSRF token — authentication is entirely cookie-based, which is
/// why this type owns its own <see cref="CookieContainer"/> and must be
/// disposed.
///
/// Construct one per operation — <see cref="WoolworthsCartService"/> does this
/// and disposes it in a finally. Never share or pool an instance: the cookie
/// jar is the signed-in identity.
/// </summary>
public sealed class WoolworthsClient : IDisposable
{
    private const string BaseUrl = "https://www.woolworths.co.za";
    public const string CartUrl = $"{BaseUrl}/check-out/cart";

    private const string UserAgent =
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 " +
        "(KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36";

    private readonly HttpClient _http;
    private readonly HttpClientHandler _handler;
    private readonly ILogger _log;
    private bool _signedIn;
    private string? _placeId;

    public WoolworthsClient(ILogger logger)
    {
        _log = logger;
        _handler = new HttpClientHandler
        {
            CookieContainer = new CookieContainer(),
            UseCookies = true,
            AllowAutoRedirect = false
        };
        _http = new HttpClient(_handler) { Timeout = TimeSpan.FromSeconds(30) };
    }

    private HttpRequestMessage Request(HttpMethod method, string path, object? body = null)
    {
        var req = new HttpRequestMessage(method, $"{BaseUrl}{path}");
        req.Headers.TryAddWithoutValidation("user-agent", UserAgent);
        req.Headers.TryAddWithoutValidation("accept", "application/json, text/plain, */*");
        req.Headers.TryAddWithoutValidation("referer", $"{BaseUrl}/");

        // Both custom headers are what the site's own SPA sends. Omitting them
        // is the difference between a JSON reply and an HTML error page.
        req.Headers.TryAddWithoutValidation("x-requested-by", "Woolworths Online");
        req.Headers.TryAddWithoutValidation("x-source-chanel", "web"); // sic: the API really does spell it this way

        if (body is not null)
        {
            req.Content = new StringContent(
                JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        }

        return req;
    }

    /// <summary>
    /// Fetch the homepage to obtain the Cloudflare and load-balancer cookies
    /// (__cf_bm, TS*, f5avr*) that gate every subsequent call.
    ///
    /// Skipping this does not fail gracefully: POSTing to /server/login without
    /// these cookies returns a 301 to /500//login rather than any useful error.
    /// </summary>
    private async Task BootstrapAsync(CancellationToken ct)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/");
        req.Headers.TryAddWithoutValidation("user-agent", UserAgent);
        req.Headers.TryAddWithoutValidation("accept", "text/html,application/xhtml+xml");

        using var res = await _http.SendAsync(req, ct);
        _log.LogDebug("Woolworths bootstrap returned {Status}", (int)res.StatusCode);
    }

    /// <summary>Bootstrap the session then sign in. Throws on bad credentials.</summary>
    public async Task<string> SignInAsync(string username, string password, CancellationToken ct)
    {
        await BootstrapAsync(ct);

        // The site base64-encodes the password before sending it. This is
        // obfuscation, not a hash — TLS is what actually protects it — but the
        // endpoint rejects a plaintext password, so we must match the format.
        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

        using var req = Request(HttpMethod.Post, "/server/login",
            new { login = username, password = encoded });
        using var res = await _http.SendAsync(req, ct);

        // A 301 here almost always means the bootstrap cookies were missing.
        if (res.StatusCode is HttpStatusCode.Moved or HttpStatusCode.Redirect)
            throw new ProviderAuthenticationException(
                "Woolworths redirected the login request, which usually means the session cookies were not established.");

        var json = await ReadJsonAsync(res, ct);

        // Bad credentials come back as HTTP 200 with an errorMessage body,
        // so the status code alone tells us nothing.
        var error = json?["errorMessage"]?.GetValue<string>();
        if (!string.IsNullOrWhiteSpace(error))
            throw new ProviderAuthenticationException(error);

        var profileId = json?["profileId"]?.GetValue<string>();
        if (string.IsNullOrWhiteSpace(profileId))
            throw new ProviderAuthenticationException("Woolworths returned an unrecognised login response.");

        _signedIn = true;
        _log.LogInformation("Signed in to Woolworths as profile {ProfileId}", profileId);

        // cartAddItems refuses everything with "Place Id is missing." unless the
        // request carries a delivery placeId — a fresh headless session has no
        // confirmed place, unlike a browser session that has been through the
        // address modal. Resolve the account's default saved address up front.
        _placeId = await FetchDefaultPlaceIdAsync(ct);
        if (_placeId is null)
            _log.LogWarning("Woolworths account has no saved address with a place id; cart adds will fail.");

        return profileId;
    }

    /// <summary>
    /// The placeId of the account's default saved address, or the first saved
    /// address that has one. Null when the account has never set an address.
    /// </summary>
    private async Task<string?> FetchDefaultPlaceIdAsync(CancellationToken ct)
    {
        using var req = Request(HttpMethod.Get, "/server/savedAddress");
        using var res = await _http.SendAsync(req, ct);
        var json = await ReadJsonAsync(res, ct);

        if (json?["addresses"] is not JsonObject addresses) return null;

        var defaultNickname = json["defaultProfileAddressNickName"]?.GetValue<string>();

        string? fallback = null;
        foreach (var (nickname, address) in addresses)
        {
            var placeId = address?["placeId"]?.GetValue<string>();
            if (string.IsNullOrWhiteSpace(placeId)) continue; // legacy entries carry an empty placeId

            if (nickname == defaultNickname) return placeId;
            fallback ??= placeId;
        }

        return fallback;
    }

    /// <summary>
    /// Add a single product to the cart.
    ///
    /// Deliberately one product per call. The API accepts an items array, but
    /// a batch containing one unavailable product silently adds NOTHING while
    /// still returning HTTP 200 — so a batch is a correctness hazard, not an
    /// optimisation.
    /// </summary>
    public async Task<AddItemOutcome> AddToCartAsync(string productId, int quantity, CancellationToken ct)
    {
        EnsureSignedIn();

        if (_placeId is null)
        {
            return new AddItemOutcome(false,
                "This Woolworths account has no saved delivery address, so items cannot be added. "
                + "Set a delivery address once at woolworths.co.za and try again.", null, null);
        }

        using var req = Request(HttpMethod.Post, "/server/cartAddItems", new
        {
            deliveryType = "Standard",
            fromDeliverySelectionPopup = "true",
            address = new { placeId = _placeId },
            items = new[]
            {
                new
                {
                    productId,
                    catalogRefId = productId,
                    quantity,
                    itemListName = "Product Description Page"
                }
            }
        });

        using var res = await _http.SendAsync(req, ct);
        var json = await ReadJsonAsync(res, ct);

        // Out of stock / not sold here: HTTP 200 with a formexceptions array.
        if (json?["formexceptions"] is JsonArray { Count: > 0 } exceptions)
        {
            var message = exceptions[0]?["message"]?.GetValue<string>() ?? "Product unavailable.";
            return new AddItemOutcome(false, message, null, null);
        }

        var counts = json?["productCountMap"];
        if (counts is null)
            return new AddItemOutcome(false, "Woolworths returned an unrecognised add-to-cart response.", null, null);

        return new AddItemOutcome(
            true,
            null,
            counts["totalProductCount"]?.GetValue<int>(),
            json?["groupSubTotal"]?["orderTotal"]?.GetValue<decimal>());
    }

    /// <summary>Current cart size and value.</summary>
    public async Task<(int? Count, decimal? Total)> GetCartAsync(CancellationToken ct)
    {
        EnsureSignedIn();

        using var req = Request(HttpMethod.Get, "/server/cartDetails?page=cart");
        using var res = await _http.SendAsync(req, ct);
        var json = await ReadJsonAsync(res, ct);

        return (json?["productCountMap"]?["totalProductCount"]?.GetValue<int>(),
                json?["groupSubTotal"]?["orderTotal"]?.GetValue<decimal>());
    }

    /// <summary>
    /// End the session. Must be POST — a GET to the same path returns 200,
    /// serves HTML, and leaves the session very much alive.
    /// </summary>
    public async Task<bool> SignOutAsync(CancellationToken ct)
    {
        if (!_signedIn) return true;

        try
        {
            using var req = Request(HttpMethod.Post, "/server/logout");
            using var res = await _http.SendAsync(req, ct);

            using var checkReq = Request(HttpMethod.Get, "/server/currentUser");
            using var checkRes = await _http.SendAsync(checkReq, ct);
            var json = await ReadJsonAsync(checkRes, ct);

            var status = json?["loggedInStatus"]?.GetValue<int>();
            _signedIn = status != 0;

            if (_signedIn) _log.LogWarning("Woolworths sign-out did not clear the session.");
            return !_signedIn;
        }
        catch (Exception ex)
        {
            // Never let a failed sign-out mask the result of the actual work.
            _log.LogWarning(ex, "Woolworths sign-out failed.");
            return false;
        }
    }

    private void EnsureSignedIn()
    {
        if (!_signedIn) throw new InvalidOperationException("Sign in before using the Woolworths cart.");
    }

    private static async Task<JsonNode?> ReadJsonAsync(HttpResponseMessage res, CancellationToken ct)
    {
        var raw = await res.Content.ReadAsStringAsync(ct);
        if (string.IsNullOrWhiteSpace(raw)) return null;

        try { return JsonNode.Parse(raw); }
        catch (JsonException) { return null; } // Several endpoints serve HTML on error.
    }

    public void Dispose()
    {
        _http.Dispose();
        _handler.Dispose();
    }
}

/// <param name="Success">Whether the product is now in the cart.</param>
/// <param name="Reason">Why it was refused, when it was.</param>
public sealed record AddItemOutcome(bool Success, string? Reason, int? CartCount, decimal? CartTotal);
