namespace CyFinance.Models.Crypto;

/// <summary>
/// Current crypto quote snapshot.
/// </summary>
public class CryptoQuote
{
    public string? Symbol { get; set; }
    public string? BaseCurrency { get; set; }
    public string? QuoteCurrency { get; set; }
    public double? Price { get; set; }
    public double? PreviousClose { get; set; }
    public double? Change { get; set; }
    public double? ChangePercent { get; set; }
    public DateTime? AsOf { get; set; }
}

/// <summary>
/// OHLC candle for crypto price history.
/// </summary>
public class CryptoHistoricalPoint
{
    public DateTime Date { get; set; }
    public double? Open { get; set; }
    public double? High { get; set; }
    public double? Low { get; set; }
    public double? Close { get; set; }
    public long? Volume { get; set; }
}