namespace Gaia.Mcp.Server.Integrations;

/// <summary>
/// Resolves an integration's declared credentials for a single call.
///
/// Over HTTP they come from the inbound request headers, and REST and MCP share
/// that deliberately: an MCP client configures exactly the same headers a curl
/// caller would send, so there is one auth story to document per integration
/// rather than two.
///
/// Over stdio there is no request to read, so the process environment stands in
/// — same trust model, since a stdio server runs as the caller's own process.
/// Nothing is cached in either case: values are read per call and discarded
/// with the call.
/// </summary>
public sealed class CredentialAccessor(IHttpContextAccessor accessor)
{
    /// <summary>
    /// The environment variable a stdio caller sets for a credential field,
    /// derived from its header so the two never drift: <c>X-Woolworths-Password</c>
    /// becomes <c>GAIA_WOOLWORTHS_PASSWORD</c>.
    /// </summary>
    public static string EnvironmentVariableFor(CredentialField field)
    {
        var name = field.Header;
        if (name.StartsWith("X-", StringComparison.OrdinalIgnoreCase)) name = name[2..];

        return "GAIA_" + name.Replace('-', '_').ToUpperInvariant();
    }

    /// <summary>
    /// Collect every credential the integration declared.
    /// </summary>
    /// <exception cref="MissingCredentialsException">
    /// Thrown when a required field is absent. The message names the exact
    /// header — and the exact environment variable — to set, because a caller
    /// wiring up an MCP client cannot see this code.
    /// </exception>
    public IntegrationCredentials Resolve(IIntegration integration)
    {
        var headers = accessor.HttpContext?.Request.Headers;

        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var missing = new List<string>();

        foreach (var field in integration.Credentials)
        {
            var value = headers?[field.Header].ToString();

            // Fall back to the environment only when the transport gave us no
            // header to read — an HTTP caller who omitted one must be told so,
            // not silently served the host's own environment.
            if (headers is null && string.IsNullOrWhiteSpace(value))
                value = Environment.GetEnvironmentVariable(EnvironmentVariableFor(field));

            if (string.IsNullOrWhiteSpace(value))
            {
                if (field.Required)
                {
                    missing.Add(headers is null ? EnvironmentVariableFor(field) : field.Header);
                }

                continue;
            }

            values[field.Key] = value;
        }

        if (missing.Count > 0)
        {
            var what = headers is null ? "environment variable(s)" : "request header(s)";
            throw new MissingCredentialsException(
                $"{integration.DisplayName} requires the following {what}: {string.Join(", ", missing)}.");
        }

        return new IntegrationCredentials(values);
    }
}
