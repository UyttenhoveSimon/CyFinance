using System.Text.Json.Serialization;

namespace CyFinance.Models.HistoricalData
{
  // Enums
    public enum ChartInterval
    {
        OneMinute,
        TwoMinutes,
        FiveMinutes,
        FifteenMinutes,
        ThirtyMinutes,
        SixtyMinutes,
        NinetyMinutes,
        OneHour,
        OneDay,
        FiveDays,
        OneWeek,
        OneMonth,
        ThreeMonths
    }

    // Response Models
    public record ChartResponse(
        Chart Chart
    );

    public record Chart(
        List<ChartResult> Result,
        object Error
    );

    public record ChartResult(
        StockMetaData Meta,
        List<long> Timestamp,
        Indicators Indicators,
        Events Events
    );

    public record StockMetaData(
        string Currency,
        string Symbol,
        string ExchangeName,
        string InstrumentType,
        long FirstTradeDate,
        long RegularMarketTime,
        int Gmtoffset,
        string Timezone,
        string ExchangeTimezoneName,
        double RegularMarketPrice,
        double ChartPreviousClose,
        double PreviousClose,
        int Scale,
        int PriceHint,
        CurrentTradingPeriod CurrentTradingPeriod,
        List<List<TradingPeriod>> TradingPeriods,
        string DataGranularity,
        string Range,
        List<string> ValidRanges
    );

    public record CurrentTradingPeriod(
        TradingPeriod Pre,
        TradingPeriod Regular,
        TradingPeriod Post
    );

    public record TradingPeriod(
        string Timezone,
        long Start,
        long End,
        int Gmtoffset
    );

    public record Indicators(
        List<Quote> Quote,
        List<AdjClose> AdjClose
    );

    public record Quote(
        List<double?> Open,
        List<double?> Low,
        List<double?> High,
        List<double?> Close,
        List<long?> Volume
    );

    public record AdjClose(
        [property: JsonPropertyName("adjclose")] List<double?> AdjustedClose
    );

    public record Events(
        Dictionary<string, Dividend> Dividends,
        Dictionary<string, Split> Splits
    );

    public record Dividend(
        double Amount,
        long Date
    );

    public record Split(
        long Date,
        int Numerator,
        int Denominator,
        string SplitRatio
    );

    public class HistoricalPrice
    {
        public DateTime Date { get; set; }
        public double? Open { get; set; }
        public double? High { get; set; }
        public double? Low { get; set; }
        public double? Close { get; set; }
        public double? AdjustedClose { get; set; }
        public long? Volume { get; set; }

        // Calculated properties
        public double? Change => Close.HasValue && Open.HasValue ? Close - Open : null;
        public double? ChangePercent => Change.HasValue && Open.HasValue && Open.Value != 0 
            ? (Change.Value / Open.Value) * 100 : null;

        public override string ToString()
        {
            return $"{Date:yyyy-MM-dd} | O: {Open:F2} | H: {High:F2} | L: {Low:F2} | C: {Close:F2} | V: {Volume:N0}";
        }
    }

    public class DividendInfo
    {
        public DateTime Date { get; set; }
        public double Amount { get; set; }

        public override string ToString()
        {
            return $"{Date:yyyy-MM-dd}: ${Amount:F4}";
        }
    }

    public class SplitInfo  {
        public DateTime Date { get; set; }
        public int Numerator { get; set; }
        public int Denominator { get; set; }
        public string SplitRatio { get; set; }

        public double SplitFactor => Denominator != 0 ? (double)Numerator / Denominator : 1.0;

        public override string ToString()
        {
            return $"{Date:yyyy-MM-dd}: {SplitRatio} (Factor: {SplitFactor:F2})";
        }
    }
}