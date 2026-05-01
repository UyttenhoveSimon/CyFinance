namespace CyFinance.Models.Currency;

/// <summary>
/// Current exchange-rate snapshot for a currency pair.
/// </summary>
public class CurrencyQuote
{
    public string? Symbol { get; set; }
    public string? BaseCurrency { get; set; }
    public string? QuoteCurrency { get; set; }
    public double? Rate { get; set; }
    public double? PreviousClose { get; set; }
    public double? Change { get; set; }
    public double? ChangePercent { get; set; }
    public string? MarketState { get; set; }
    public DateTime? AsOf { get; set; }
}

/// <summary>
/// OHLC point for a currency pair time series.
/// </summary>
public class CurrencyHistoricalPoint
{
    public DateTime Date { get; set; }
    public double? Open { get; set; }
    public double? High { get; set; }
    public double? Low { get; set; }
    public double? Close { get; set; }
    public long? Volume { get; set; }
}