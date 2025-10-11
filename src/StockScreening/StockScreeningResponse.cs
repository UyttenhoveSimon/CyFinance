// --- File: Models/ScreenerResponse.cs ---

using System.Text.Json.Serialization;
namespace CyFinance.Models.StockScreening;

/// <summary>
/// The root object of the API response.
/// </summary>
public class ScreenerResponse
{
    [JsonPropertyName("finance")]
    public FinanceResult Finance { get; set; }
}

public class FinanceResult
{
    [JsonPropertyName("result")]
    public List<ScreenerResult> Result { get; set; }

    [JsonPropertyName("error")]
    public object Error { get; set; }
}

/// <summary>
/// Contains the metadata and the list of quotes from the screener.
/// </summary>
public class ScreenerResult
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("quotes")]
    public List<Quote> Quotes { get; set; }
}

/// <summary>
/// Represents a single stock/quote returned by the screener.
/// This is a subset of the available fields for brevity.
/// </summary>
public class Quote
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; }

    [JsonPropertyName("longName")]
    public string LongName { get; set; }

    [JsonPropertyName("quoteType")]
    public string QuoteType { get; set; }

    [JsonPropertyName("exchange")]
    public string Exchange { get; set; }

    [JsonPropertyName("marketCap")]
    public long? MarketCap { get; set; }

    [JsonPropertyName("regularMarketPrice")]
    public decimal? RegularMarketPrice { get; set; }

    [JsonPropertyName("regularMarketChange")]
    public decimal? RegularMarketChange { get; set; }

    [JsonPropertyName("regularMarketChangePercent")]
    public decimal? RegularMarketChangePercent { get; set; }

    [JsonPropertyName("regularMarketVolume")]
    public long? RegularMarketVolume { get; set; }

    [JsonPropertyName("trailingPE")]
    public decimal? TrailingPE { get; set; }
}