using System.Text.Json.Serialization;

namespace CyFinance.Models.StockScreening;

/// <summary>
/// Request body for Yahoo Finance screener custom queries.
/// Mirrors yfinance screen(query=QueryBase, ...).
/// </summary>
public class ScreenerRequest
{
    [JsonPropertyName("offset")]
    public int Offset { get; set; } = 0;

    [JsonPropertyName("size")]
    public int Size { get; set; } = 25;

    [JsonPropertyName("sortField")]
    public string SortField { get; set; } = "ticker";

    [JsonPropertyName("sortType")]
    public string SortType { get; set; } = "DESC";

    [JsonPropertyName("quoteType")]
    public string QuoteType { get; set; } = "EQUITY";

    [JsonPropertyName("userId")]
    public string UserId { get; set; } = "";

    [JsonPropertyName("userIdType")]
    public string UserIdType { get; set; } = "guid";

    [JsonPropertyName("query")]
    public required ScreenerQueryNode Query { get; set; }
}

/// <summary>
/// Generic screener query node accepted by Yahoo Finance.
/// Supports nested logical operators and value comparisons.
/// </summary>
public class ScreenerQueryNode
{
    [JsonPropertyName("operator")]
    public required string Operator { get; set; }

    [JsonPropertyName("operands")]
    public required List<object> Operands { get; set; }
}

/// <summary>
/// Helper factory for yfinance-style query building.
/// </summary>
public static class ScreenerQuery
{
    public static ScreenerQueryNode Eq(string field, object value) =>
        new() { Operator = "EQ", Operands = new List<object> { field, value } };

    public static ScreenerQueryNode IsIn(string field, params object[] values) =>
        new() { Operator = "IS-IN", Operands = new List<object>(new[] { (object)field }.Concat(values)) };

    public static ScreenerQueryNode Btwn(string field, object lower, object upper) =>
        new() { Operator = "BTWN", Operands = new List<object> { field, lower, upper } };

    public static ScreenerQueryNode Gt(string field, object value) =>
        new() { Operator = "GT", Operands = new List<object> { field, value } };

    public static ScreenerQueryNode Lt(string field, object value) =>
        new() { Operator = "LT", Operands = new List<object> { field, value } };

    public static ScreenerQueryNode Gte(string field, object value) =>
        new() { Operator = "GTE", Operands = new List<object> { field, value } };

    public static ScreenerQueryNode Lte(string field, object value) =>
        new() { Operator = "LTE", Operands = new List<object> { field, value } };

    public static ScreenerQueryNode And(params ScreenerQueryNode[] nodes) =>
        new() { Operator = "AND", Operands = nodes.Cast<object>().ToList() };

    public static ScreenerQueryNode Or(params ScreenerQueryNode[] nodes) =>
        new() { Operator = "OR", Operands = nodes.Cast<object>().ToList() };
}
