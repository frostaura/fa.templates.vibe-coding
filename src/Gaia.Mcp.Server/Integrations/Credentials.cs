namespace Gaia.Mcp.Server.Integrations;

/// <summary>
/// One credential an integration needs from the caller.
/// </summary>
/// <param name="Key">Stable short name the integration reads, e.g. "username".</param>
/// <param name="Header">The HTTP header the caller sets, e.g. "X-Woolworths-Username".</param>
/// <param name="Description">What to put in it, shown in discovery output.</param>
/// <param name="Secret">True when the value must never be echoed back in a response or log.</param>
/// <param name="Required">False for optional refinements such as a store or region.</param>
public sealed record CredentialField(
    string Key,
    string Header,
    string Description,
    bool Secret = false,
    bool Required = true);

/// <summary>
/// Describes an integration this service can drive.
///
/// Every integration is open — the service enforces no gateway authentication
/// of its own. What authorises a call is the caller's own credentials for the
/// downstream provider, supplied per request. Each integration therefore
/// declares its own auth scheme here, because what Woolworths needs (an
/// account login) has nothing to do with what the next provider might need
/// (an API key, a token, a store id).
///
/// Implementations are registered in DI and enumerated by the discovery
/// endpoint, so adding an integration surfaces it automatically to both REST
/// callers and MCP clients.
/// </summary>
public interface IIntegration
{
    /// <summary>Stable lowercase identifier used in routes, e.g. "woolworths".</summary>
    string Key { get; }

    string DisplayName { get; }

    /// <summary>One sentence on what the integration does.</summary>
    string Description { get; }

    /// <summary>The credentials a caller must supply to act on their own account.</summary>
    IReadOnlyList<CredentialField> Credentials { get; }
}

/// <summary>
/// Credential values resolved for a single request. Never cached, never
/// logged, and discarded when the request ends.
/// </summary>
public sealed class IntegrationCredentials(IReadOnlyDictionary<string, string> values)
{
    /// <summary>Value for a declared field key.</summary>
    public string this[string key] =>
        values.TryGetValue(key, out var v)
            ? v
            : throw new KeyNotFoundException($"No credential '{key}' was supplied.");

    public string? GetOptional(string key) => values.GetValueOrDefault(key);

    public bool Has(string key) => values.ContainsKey(key);
}

/// <summary>
/// Raised when a caller omits credentials an integration declared as required.
/// Surfaces as HTTP 401 listing the exact headers to set.
/// </summary>
public sealed class MissingCredentialsException(string message) : Exception(message);

/// <summary>
/// Raised when the downstream provider rejects the supplied credentials.
/// Surfaces as HTTP 401 — the caller's credentials are wrong, not the request.
/// </summary>
public sealed class ProviderAuthenticationException(string message) : Exception(message);
