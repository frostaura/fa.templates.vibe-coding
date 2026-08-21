namespace Gaia.Mcp.Server.Integrations;

/// <summary>One line parsed out of a shared shopping list.</summary>
/// <param name="Raw">The original line, kept so callers can show what was interpreted.</param>
/// <param name="Name">The ingredient name with quantity and unit stripped.</param>
/// <param name="Quantity">Recipe quantity, when the line carried one. This is the amount the recipe needs, NOT the number of retail units to buy.</param>
/// <param name="Unit">Recipe unit (g, ml, tsp...), when present.</param>
/// <param name="Category">The section heading the line appeared under.</param>
public sealed record ShoppingListItem(
    string Raw,
    string Name,
    double? Quantity,
    string? Unit,
    string? Category);

/// <summary>A product offered by a retailer.</summary>
public sealed record Product(
    string Id,
    string Name,
    decimal? Price,
    string? Url);

/// <summary>The outcome of trying to put one ingredient into the cart.</summary>
/// <param name="Ingredient">The ingredient as parsed from the list.</param>
/// <param name="Product">The product actually added, or null when nothing could be added.</param>
/// <param name="Query">The search phrase that produced the match, useful for tuning.</param>
/// <param name="Alternatives">Runner-up candidates, so a caller can offer a swap.</param>
/// <param name="Status">Whether the ingredient made it into the cart.</param>
/// <param name="Detail">Human-readable reason when the status is not Added.</param>
public sealed record CartLineResult(
    string Ingredient,
    Product? Product,
    string? Query,
    IReadOnlyList<Product> Alternatives,
    CartLineStatus Status,
    string? Detail);

public enum CartLineStatus
{
    /// <summary>In the cart.</summary>
    Added,

    /// <summary>Search returned nothing for any query variation.</summary>
    NoMatch,

    /// <summary>Matched products exist but none could be added (out of stock, not sold at this location).</summary>
    Unavailable,

    /// <summary>Dry run — resolved but deliberately not added.</summary>
    Resolved
}

/// <summary>The result of processing a whole shopping list.</summary>
public sealed record CartBuildResult(
    IReadOnlyList<CartLineResult> Lines,
    int CartItemCount,
    decimal? CartTotal,
    string CartUrl)
{
    public int AddedCount => Lines.Count(l => l.Status == CartLineStatus.Added);
    public IReadOnlyList<CartLineResult> Failures =>
        [.. Lines.Where(l => l.Status is CartLineStatus.NoMatch or CartLineStatus.Unavailable)];
}
