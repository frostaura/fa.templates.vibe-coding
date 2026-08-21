using System.Globalization;
using System.Text.RegularExpressions;

namespace Gaia.Mcp.Server.Integrations;

/// <summary>
/// Parses the plain-text shopping list that the Cookidoo (Thermomix) app puts
/// on the share sheet, and turns each ingredient into retailer search phrases.
///
/// The share format is indentation-based: ALL-CAPS section headings
/// ("VEGETABLES &amp; FRESH HERBS") followed by indented ingredient lines that
/// may or may not carry a leading quantity and unit.
/// </summary>
public static partial class ShoppingListParser
{
    [GeneratedRegex(@"^[A-Z][A-Z\s,&.()'’\-]+$")]
    private static partial Regex CategoryHeading();

    [GeneratedRegex(@"^\s*([\d.]+)?\s*(g|kg|ml|l|tsp|tbsp|cup|cups|pinch|clove|cloves)?\s+(.*)$",
        RegexOptions.IgnoreCase)]
    private static partial Regex QuantityPrefix();

    [GeneratedRegex(@"\(.*?\)")]
    private static partial Regex Parenthetical();

    [GeneratedRegex(@"\s+")]
    private static partial Regex Whitespace();

    /// <summary>
    /// Words describing preparation or form rather than the product itself.
    /// Dropped progressively to widen a search that returned nothing.
    /// </summary>
    private static readonly HashSet<string> Noise = new(StringComparer.OrdinalIgnoreCase)
    {
        "fresh", "dried", "ground", "grated", "skinless", "boneless", "chopped",
        "sliced", "minced", "crushed", "whole", "raw", "cooked", "frozen",
        "pouring", "whipping", "baby", "leaves", "fillets", "clove", "cloves"
    };

    /// <summary>
    /// Cookidoo ships Australian/European product vocabulary. Without these
    /// the South African catalogue misses outright — "capsicum" returns
    /// nothing at Woolworths, and "flat-leaf parsley" is sold as "Italian
    /// parsley". Multi-word keys are matched against the whole ingredient;
    /// single-word keys are substituted per word.
    /// </summary>
    private static readonly Dictionary<string, string> Synonyms = new(StringComparer.OrdinalIgnoreCase)
    {
        ["capsicum"] = "pepper",
        ["aubergine"] = "brinjal",
        ["courgette"] = "baby marrow",
        ["zucchini"] = "baby marrow",
        ["rockmelon"] = "melon",
        ["red capsicum"] = "red pepper",
        ["green capsicum"] = "green pepper",
        ["yellow capsicum"] = "yellow pepper",
        ["flat-leaf parsley"] = "italian parsley",
        ["chicken stock paste"] = "chicken stock",
        ["vegetable stock paste"] = "vegetable stock",
        ["pouring (whipping) cream"] = "pouring cream",
        ["thickened cream"] = "whipping cream",
        ["caster sugar"] = "castor sugar",
        ["plain flour"] = "cake flour",
        ["self-raising flour"] = "self raising flour"
    };

    /// <summary>Parse the shared text into ingredient lines.</summary>
    public static IReadOnlyList<ShoppingListItem> Parse(string text)
    {
        var items = new List<ShoppingListItem>();
        if (string.IsNullOrWhiteSpace(text)) return items;

        string? category = null;

        foreach (var rawLine in text.Split('\n'))
        {
            var line = rawLine.Trim();
            if (line.Length == 0) continue;

            // A heading is all-caps and never starts with a quantity.
            if (!char.IsDigit(line[0]) && CategoryHeading().IsMatch(line))
            {
                category = line;
                continue;
            }

            double? qty = null;
            string? unit = null;
            var name = line;

            var m = QuantityPrefix().Match(line);
            if (m.Success)
            {
                if (m.Groups[1].Success &&
                    double.TryParse(m.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
                {
                    qty = parsed;
                }

                if (m.Groups[2].Success) unit = m.Groups[2].Value.ToLowerInvariant();
                name = m.Groups[3].Value.Trim();
            }

            if (name.Length == 0) continue;
            items.Add(new ShoppingListItem(line, name, qty, unit, category));
        }

        return items;
    }

    /// <summary>
    /// Build an ordered list of search phrases for an ingredient, most
    /// specific first. Callers should try each until one returns results:
    /// the later entries trade precision for the chance of any match at all.
    /// </summary>
    public static IReadOnlyList<string> ToQueries(string ingredientName)
    {
        var queries = new List<string>();

        void Push(string candidate)
        {
            var value = Whitespace().Replace(candidate, " ").Trim();
            if (value.Length > 0 && !queries.Contains(value, StringComparer.OrdinalIgnoreCase))
                queries.Add(value);
        }

        var lower = Whitespace()
            .Replace(Parenthetical().Replace(ingredientName.ToLowerInvariant(), " "), " ")
            .Trim();

        // Whole-phrase synonym, e.g. "flat-leaf parsley" -> "italian parsley".
        if (Synonyms.TryGetValue(lower, out var direct)) Push(direct);

        // Word-level synonyms, e.g. "red capsicum strips" -> "red pepper strips".
        var mapped = lower;
        foreach (var (from, to) in Synonyms.Where(kv => !kv.Key.Contains(' ')))
            mapped = Regex.Replace(mapped, $@"\b{Regex.Escape(from)}\b", to, RegexOptions.IgnoreCase);
        Push(mapped);

        // Strip preparation words to broaden.
        var words = mapped.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var core = words.Where(w => !Noise.Contains(w)).ToArray();
        if (core.Length > 0) Push(string.Join(' ', core));

        // Last resorts: the trailing pair, then the head noun alone.
        if (core.Length > 2) Push(string.Join(' ', core[^2..]));
        if (core.Length > 1) Push(core[^1]);

        return queries;
    }
}
