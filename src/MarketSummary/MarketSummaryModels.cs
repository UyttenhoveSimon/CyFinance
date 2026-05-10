using System.Text.Json.Serialization;
using CyFinance.Models.QuoteSummary;

namespace CyFinance.Models.MarketSummary;

public class MarketSnapshot
{
    public string? Market { get; set; }
    public MarketStatus? Status { get; set; }
    public Dictionary<string, MarketSummaryItem>? SummaryByExchange { get; set; }
}

public class MarketStatus
{
    public string? Market { get; set; }
    public string? TimezoneName { get; set; }
    public string? TimezoneShortName { get; set; }
    public int? GmtOffsetMilliseconds { get; set; }
    public DateTimeOffset? Open { get; set; }
    public DateTimeOffset? Close { get; set; }
}

public class MarketSummaryItem
{
    public string? Exchange { get; set; }
    public string? ShortName { get; set; }
    public double? RegularMarketPrice { get; set; }
    public double? RegularMarketChange { get; set; }
    public double? RegularMarketChangePercent { get; set; }
}

internal sealed class MarketSummaryRoot
{
    [JsonPropertyName("marketSummaryResponse")]
    public MarketSummaryResponse? MarketSummaryResponse { get; set; }
}

internal sealed class MarketSummaryResponse
{
    [JsonPropertyName("result")]
    public List<MarketSummaryQuote>? Result { get; set; }

    [JsonPropertyName("error")]
    public object? Error { get; set; }
}

internal sealed class MarketSummaryQuote
{
    [JsonPropertyName("exchange")]
    public string? Exchange { get; set; }

    [JsonPropertyName("shortName")]
    public string? ShortName { get; set; }

    [JsonPropertyName("regularMarketPrice")]
    public YahooValue? RegularMarketPrice { get; set; }

    [JsonPropertyName("regularMarketChange")]
    public YahooValue? RegularMarketChange { get; set; }

    [JsonPropertyName("regularMarketChangePercent")]
    public YahooValue? RegularMarketChangePercent { get; set; }
}

internal sealed class MarketTimeRoot
{
    [JsonPropertyName("finance")]
    public MarketTimeFinance? Finance { get; set; }
}

internal sealed class MarketTimeFinance
{
    [JsonPropertyName("marketTimes")]
    public List<MarketTimesContainer>? MarketTimes { get; set; }
}

internal sealed class MarketTimesContainer
{
    [JsonPropertyName("marketTime")]
    public List<MarketTimeEntry>? MarketTime { get; set; }
}

internal sealed class MarketTimeEntry
{
    [JsonPropertyName("open")]
    public string? Open { get; set; }

    [JsonPropertyName("close")]
    public string? Close { get; set; }

    [JsonPropertyName("timezone")]
    public List<MarketTimezone>? Timezone { get; set; }
}

internal sealed class MarketTimezone
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("short")]
    public string? Short { get; set; }

    [JsonPropertyName("gmtoffset")]
    public int? GmtOffset { get; set; }
}