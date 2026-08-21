using Gaia.Mcp.Server.Integrations;

namespace Gaia.Mcp.Tests;

/// <summary>
/// The parser is the only part of the pipeline that can be tested without
/// touching Woolworths, so it carries the behavioural contract: quantities are
/// stripped, section headings are not mistaken for ingredients, and the
/// Australian vocabulary Cookidoo emits is translated to what the South
/// African catalogue actually calls things.
/// </summary>
public class ShoppingListParserTests
{
    /// <summary>A verbatim Cookidoo share, including its indentation and blank lines.</summary>
    private const string SampleShare = """
            BEVERAGES, LIQUIDS & ALCOHOLIC DRINKS

              170 g water
            CEREALS & DRIED LEGUMES

              150 g dried fusilli pasta
            DAIRY PRODUCTS INCL. BUTTER

              grated Parmesan cheese
              130 g pouring (whipping) cream
            MEAT

              250 g skinless chicken thigh fillets
            SPICES & SEASONINGS

              0.33333 tsp paprika
            VEGETABLES & FRESH HERBS

              fresh flat-leaf parsley
              1 garlic clove
              0.5 red capsicum
        """;

    [Fact]
    public void Parse_extracts_every_ingredient_and_no_headings()
    {
        var items = ShoppingListParser.Parse(SampleShare);

        Assert.Equal(9, items.Count);
        Assert.DoesNotContain(items, i => i.Name.Contains("BEVERAGES"));
        Assert.DoesNotContain(items, i => i.Name.Contains("VEGETABLES"));
    }

    [Fact]
    public void Parse_separates_quantity_unit_and_name()
    {
        var water = ShoppingListParser.Parse(SampleShare).First();

        Assert.Equal("water", water.Name);
        Assert.Equal(170, water.Quantity);
        Assert.Equal("g", water.Unit);
    }

    [Fact]
    public void Parse_handles_fractional_quantities()
    {
        var paprika = ShoppingListParser.Parse(SampleShare).Single(i => i.Name == "paprika");

        Assert.Equal(0.33333, paprika.Quantity!.Value, 5);
        Assert.Equal("tsp", paprika.Unit);
    }

    [Fact]
    public void Parse_keeps_ingredients_without_a_quantity()
    {
        var items = ShoppingListParser.Parse(SampleShare);

        Assert.Contains(items, i => i.Name == "grated Parmesan cheese" && i.Quantity is null);
    }

    [Fact]
    public void Parse_attributes_each_ingredient_to_its_section()
    {
        var items = ShoppingListParser.Parse(SampleShare);
        var parsley = items.Single(i => i.Name.Contains("parsley"));

        Assert.Equal("VEGETABLES & FRESH HERBS", parsley.Category);
    }

    [Fact]
    public void Parse_tolerates_empty_input()
    {
        Assert.Empty(ShoppingListParser.Parse(""));
        Assert.Empty(ShoppingListParser.Parse("   \n\n  "));
    }

    // Localisation: these are the cases that silently return zero results
    // against the South African catalogue if left untranslated.

    [Theory]
    [InlineData("red capsicum", "red pepper")]
    [InlineData("flat-leaf parsley", "italian parsley")]
    [InlineData("courgette", "baby marrow")]
    public void ToQueries_translates_australian_vocabulary_first(string ingredient, string expected)
    {
        var queries = ShoppingListParser.ToQueries(ingredient);

        Assert.Equal(expected, queries[0]);
    }

    [Fact]
    public void ToQueries_strips_parentheticals()
    {
        var queries = ShoppingListParser.ToQueries("pouring (whipping) cream");

        Assert.Contains("pouring cream", queries);
        Assert.DoesNotContain(queries, q => q.Contains('('));
    }

    [Fact]
    public void ToQueries_widens_from_specific_to_general()
    {
        var queries = ShoppingListParser.ToQueries("fresh baby spinach leaves");

        // Most specific first, then progressively stripped of prep words so a
        // failed search has somewhere to fall back to.
        Assert.Equal("fresh baby spinach leaves", queries[0]);
        Assert.Contains("spinach", queries);
        Assert.True(queries.Count > 1, "A multi-word ingredient must offer fallbacks.");
    }

    [Fact]
    public void ToQueries_never_repeats_a_phrase()
    {
        var queries = ShoppingListParser.ToQueries("paprika");

        Assert.Equal(queries.Distinct(StringComparer.OrdinalIgnoreCase).Count(), queries.Count);
    }
}
