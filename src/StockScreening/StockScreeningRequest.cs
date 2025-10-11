using System.Text.Json.Serialization; // Required for JsonPropertyName

namespace CyFinance.Models.StockScreening;
/// <summary>
/// Represents the main request body for the Yahoo Finance screener API.
/// </summary>
public class ScreenerRequest
{
    [JsonPropertyName("size")]
    public int Size { get; set; } = 25;

    [JsonPropertyName("offset")]
    public int Offset { get; set; } = 0;

    [JsonPropertyName("sortField")]
    public string SortField { get; set; } = "marketcap"; // e.g., "marketcap", "intradayprice", "regularmarketchange"

    [JsonPropertyName("sortType")]
    public string SortType { get; set; } = "DESC"; // "ASC" or "DESC"

    [JsonPropertyName("quoteType")]
    public string QuoteType { get; set; } = "EQUITY";

    [JsonPropertyName("query")]
    public ScreenerQuery Query { get; set; }

    public ScreenerRequest(ScreenerQuery query)
    {
        Query = query;
    }
}

/// <summary>
/// Defines the filtering criteria for the screener.
/// </summary>
public class ScreenerQuery
{
    [JsonPropertyName("operator")]
    public string Operator { get; set; } = "AND";

    [JsonPropertyName("operands")]
    public List<ScreenerOperand> Operands { get; set; } = new List<ScreenerOperand>();
}

/// <summary>
/// Represents a single filter criterion (e.g., region is 'US').
/// </summary>
public class ScreenerOperand
{
    [JsonPropertyName("operator")]
    public string Operator { get; set; } // e.g., "EQ", "GT", "LT"

    [JsonPropertyName("operands")]
    public List<object> Operands { get; set; } // e.g., ["region", "us"] or ["marketcap", 2000000000]

    public ScreenerOperand(string field, string op, object value)
    {
        Operator = op;
        Operands = new List<object> { field, value };
    }
}
