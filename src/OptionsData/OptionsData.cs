using System.Text.Json.Serialization;
using System.Collections.Generic;

// This is the top-level object that wraps the entire JSON response.
public class OptionsDataResponse
{
    [JsonPropertyName("optionChain")]
    public OptionChain OptionChain { get; set; }
}

public class OptionChain
{
    [JsonPropertyName("result")]
    public List<OptionsResult> Result { get; set; }
}

public class OptionsResult
{
    [JsonPropertyName("underlyingSymbol")]
    public string UnderlyingSymbol { get; set; }

    [JsonPropertyName("expirationDates")]
    public List<long> ExpirationDates { get; set; }

    [JsonPropertyName("strikes")]
    public List<double> Strikes { get; set; }

    [JsonPropertyName("hasMiniOptions")]
    public bool HasMiniOptions { get; set; }

    [JsonPropertyName("quote")]
    public Quote Quote { get; set; }

    [JsonPropertyName("options")]
    public List<OptionsChainData> Options { get; set; }
}

// Represents a single option chain for a specific expiration date.
public class OptionsChainData
{
    [JsonPropertyName("expirationDate")]
    public long ExpirationDate { get; set; }

    [JsonPropertyName("hasMiniOptions")]
    public bool HasMiniOptions { get; set; }

    [JsonPropertyName("calls")]
    public List<OptionContract> Calls { get; set; }

    [JsonPropertyName("puts")]
    public List<OptionContract> Puts { get; set; }
}

// Represents a single call or put option contract.
public class OptionContract
{
    [JsonPropertyName("contractSymbol")]
    public string ContractSymbol { get; set; }

    [JsonPropertyName("strike")]
    public double Strike { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    [JsonPropertyName("lastPrice")]
    public double LastPrice { get; set; }

    [JsonPropertyName("change")]
    public double Change { get; set; }

    [JsonPropertyName("percentChange")]
    public double PercentChange { get; set; }

    [JsonPropertyName("volume")]
    public int? Volume { get; set; } // Nullable for contracts with no volume

    [JsonPropertyName("openInterest")]
    public int? OpenInterest { get; set; } // Nullable for contracts with no OI

    [JsonPropertyName("bid")]
    public double Bid { get; set; }

    [JsonPropertyName("ask")]
    public double Ask { get; set; }

    [JsonPropertyName("contractSize")]
    public string ContractSize { get; set; }

    [JsonPropertyName("expiration")]
    public long Expiration { get; set; }

    [JsonPropertyName("lastTradeDate")]
    public long LastTradeDate { get; set; }

    [JsonPropertyName("impliedVolatility")]
    public double ImpliedVolatility { get; set; }

    [JsonPropertyName("inTheMoney")]
    public bool InTheMoney { get; set; }
}

// Represents the detailed quote information for the underlying stock.
public class Quote
{
    [JsonPropertyName("language")]
    public string Language { get; set; }

    [JsonPropertyName("region")]
    public string Region { get; set; }

    [JsonPropertyName("quoteType")]
    public string QuoteType { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    [JsonPropertyName("regularMarketDayLow")]
    public double RegularMarketDayLow { get; set; }

    [JsonPropertyName("regularMarketVolume")]
    public long RegularMarketVolume { get; set; }

    [JsonPropertyName("regularMarketPreviousClose")]
    public double RegularMarketPreviousClose { get; set; }

    [JsonPropertyName("bid")]
    public double Bid { get; set; }

    [JsonPropertyName("ask")]
    public double Ask { get; set; }

    [JsonPropertyName("longName")]
    public string LongName { get; set; }

    [JsonPropertyName("fiftyTwoWeekLow")]
    public double FiftyTwoWeekLow { get; set; }

    [JsonPropertyName("fiftyTwoWeekHigh")]
    public double FiftyTwoWeekHigh { get; set; }

    [JsonPropertyName("marketCap")]
    public long MarketCap { get; set; }

    [JsonPropertyName("forwardPE")]
    public double ForwardPE { get; set; }

    [JsonPropertyName("priceToBook")]
    public double PriceToBook { get; set; }

    [JsonPropertyName("sourceInterval")]
    public int SourceInterval { get; set; }

    [JsonPropertyName("exchangeDataDelayedBy")]
    public int ExchangeDataDelayedBy { get; set; }

    [JsonPropertyName("regularMarketChangePercent")]
    public double RegularMarketChangePercent { get; set; }

    [JsonPropertyName("regularMarketPrice")]
    public double RegularMarketPrice { get; set; }

    [JsonPropertyName("shortName")]
    public string ShortName { get; set; }

    [JsonPropertyName("exchange")]
    public string Exchange { get; set; }

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; }

    [JsonPropertyName("postMarketChangePercent")]
    public double PostMarketChangePercent { get; set; }

    [JsonPropertyName("postMarketPrice")]
    public double PostMarketPrice { get; set; }

    [JsonPropertyName("regularMarketChange")]
    public double RegularMarketChange { get; set; }

    [JsonPropertyName("regularMarketDayHigh")]
    public double RegularMarketDayHigh { get; set; }

    [JsonPropertyName("regularMarketTime")]
    public long RegularMarketTime { get; set; }
}