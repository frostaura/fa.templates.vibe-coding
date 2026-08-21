
namespace Gaia.Mcp.Server.Integrations.Woolworths;

/// <summary>
/// Declares the Woolworths South Africa integration and the credentials it
/// needs. Woolworths has no API keys or OAuth — the only way to act on a cart
/// is to sign in as the account holder, so the scheme is a plain account
/// login.
/// </summary>
public sealed class WoolworthsIntegration : IIntegration
{
    public const string IntegrationKey = "woolworths";

    public string Key => IntegrationKey;

    public string DisplayName => "Woolworths South Africa";

    public string Description =>
        "Search the Woolworths SA grocery catalogue and add items to the signed-in customer's cart.";

    public IReadOnlyList<CredentialField> Credentials =>
    [
        new("username", "X-Woolworths-Username",
            "The email address of the Woolworths online account.",
            Secret: false),

        new("password", "X-Woolworths-Password",
            "The account password, sent as plaintext over TLS. The service base64-encodes it "
            + "for Woolworths itself and never stores or logs it.",
            Secret: true)
    ];
}
