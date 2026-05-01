using CyFinance.Models.Currency;
using CyFinance.Models.HistoricalData;
using System.Net.Http.Json;

namespace CyFinance.Services.Currency;

/// <summary>
/// Currency/Forex API service backed by Yahoo chart data.
/// </summary>
public class CurrencyService : BaseService, ICurrencyService
{
    private const string BaseUrl = "https://query2.finance.yahoo.com";

    public CurrencyService(HttpClient client) : base(client)
    {
        if (!Client.DefaultRequestHeaders.Contains("User-Agent"))
        {
            Client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        }
    }

    public async Task<CurrencyQuote?> GetExchangeRateAsync(string baseCurrency, string quoteCurrency)
    {
        var normalizedBase = NormalizeCurrencyCode(baseCurrency, nameof(baseCurrency));
        var normalizedQuote = NormalizeCurrencyCode(quoteCurrency, nameof(quoteCurrency));
        var symbol = BuildForexSymbol(normalizedBase, normalizedQuote);

        var chart = await GetQuoteChartAsync(symbol);
        var result = chart?.Chart?.Result?.FirstOrDefault();
        var quote = result?.Indicators?.Quote?.FirstOrDefault();

        if (result == null || quote == null)
        {
            return null;
        }

        var (latestRate, asOf) = GetLatestClose(quote.Close, result.Timestamp);
        var previousClose = result.Meta?.PreviousClose;
        double? change = latestRate.HasValue && previousClose.HasValue
            ? latestRate.Value - previousClose.Value
            : null;
        double? changePercent = change.HasValue && previousClose.HasValue && previousClose.Value != 0
            ? (change.Value / previousClose.Value) * 100
            : null;

        return new CurrencyQuote
        {
            Symbol = symbol,
            BaseCurrency = normalizedBase,
            QuoteCurrency = normalizedQuote,
            Rate = latestRate,
            PreviousClose = previousClose,
            Change = change,
            ChangePercent = changePercent,
            MarketState = result.Meta?.CurrentTradingPeriod?.Regular != null ? "REGULAR" : null,
            AsOf = asOf
        };
    }

    public async Task<List<CurrencyHistoricalPoint>> GetHistoricalRatesAsync(
        string baseCurrency,
        string quoteCurrency,
        DateTime? startDate = null,
        DateTime? endDate = null,
        ChartInterval interval = ChartInterval.OneDay)
    {
        var normalizedBase = NormalizeCurrencyCode(baseCurrency, nameof(baseCurrency));
        var normalizedQuote = NormalizeCurrencyCode(quoteCurrency, nameof(quoteCurrency));
        var symbol = BuildForexSymbol(normalizedBase, normalizedQuote);

        var chart = await GetHistoricalChartAsync(symbol, startDate, endDate, interval);
        var result = chart?.Chart?.Result?.FirstOrDefault();
        var quote = result?.Indicators?.Quote?.FirstOrDefault();

        if (result?.Timestamp == null || quote == null)
        {
            return new List<CurrencyHistoricalPoint>();
        }

        var points = new List<CurrencyHistoricalPoint>(result.Timestamp.Count);
        for (var i = 0; i < result.Timestamp.Count; i++)
        {
            points.Add(new CurrencyHistoricalPoint
            {
                Date = DateTimeOffset.FromUnixTimeSeconds(result.Timestamp[i]).UtcDateTime,
                Open = GetValueAtIndex(quote.Open, i),
                High = GetValueAtIndex(quote.High, i),
                Low = GetValueAtIndex(quote.Low, i),
                Close = GetValueAtIndex(quote.Close, i),
                Volume = GetLongValueAtIndex(quote.Volume, i)
            });
        }

        return points.OrderBy(p => p.Date).ToList();
    }

    public async Task<double?> ConvertAsync(double amount, string baseCurrency, string quoteCurrency)
    {
        var quote = await GetExchangeRateAsync(baseCurrency, quoteCurrency);
        return quote?.Rate.HasValue == true ? amount * quote.Rate.Value : null;
    }

    private async Task<ChartResponse?> GetQuoteChartAsync(string symbol)
    {
        await EnsureAuthenticatedAsync(symbol);
        var url = $"{BaseUrl}/v8/finance/chart/{Uri.EscapeDataString(symbol)}?interval=1m&range=1d&includePrePost=false";
        return await Client.GetFromJsonAsync<ChartResponse>(url, _jsonOptions);
    }

    private async Task<ChartResponse?> GetHistoricalChartAsync(
        string symbol,
        DateTime? startDate,
        DateTime? endDate,
        ChartInterval interval)
    {
        await EnsureAuthenticatedAsync(symbol);

        var period1 = ((DateTimeOffset)(startDate ?? DateTime.UtcNow.AddYears(-1))).ToUnixTimeSeconds();
        var period2 = ((DateTimeOffset)(endDate ?? DateTime.UtcNow)).ToUnixTimeSeconds();

        var url =
            $"{BaseUrl}/v8/finance/chart/{Uri.EscapeDataString(symbol)}" +
            $"?period1={period1}&period2={period2}&interval={GetIntervalString(interval)}&includePrePost=false";

        return await Client.GetFromJsonAsync<ChartResponse>(url, _jsonOptions);
    }

    private static string BuildForexSymbol(string baseCurrency, string quoteCurrency)
        => $"{baseCurrency}{quoteCurrency}=X";

    private static string NormalizeCurrencyCode(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Currency code cannot be empty", paramName);
        }

        var normalized = value.Trim().ToUpperInvariant();
        if (normalized.Length != 3 || normalized.Any(c => !char.IsAsciiLetter(c)))
        {
            throw new ArgumentException("Currency code must be a 3-letter ISO code", paramName);
        }

        return normalized;
    }

    private static (double? close, DateTime? asOf) GetLatestClose(List<double?>? closes, List<long>? timestamps)
    {
        if (closes == null || timestamps == null)
        {
            return (null, null);
        }

        var count = Math.Min(closes.Count, timestamps.Count);
        for (var i = count - 1; i >= 0; i--)
        {
            if (!closes[i].HasValue)
            {
                continue;
            }

            return (closes[i], DateTimeOffset.FromUnixTimeSeconds(timestamps[i]).UtcDateTime);
        }

        return (null, null);
    }

    private static long? GetLongValueAtIndex(List<long?>? values, int index)
    {
        return values != null && index < values.Count ? values[index] : null;
    }

    private static double? GetValueAtIndex(List<double?>? values, int index)
    {
        return values != null && index < values.Count ? values[index] : null;
    }

    private static string GetIntervalString(ChartInterval interval)
    {
        return interval switch
        {
            ChartInterval.OneMinute => "1m",
            ChartInterval.TwoMinutes => "2m",
            ChartInterval.FiveMinutes => "5m",
            ChartInterval.FifteenMinutes => "15m",
            ChartInterval.ThirtyMinutes => "30m",
            ChartInterval.SixtyMinutes => "60m",
            ChartInterval.NinetyMinutes => "90m",
            ChartInterval.OneHour => "1h",
            ChartInterval.OneDay => "1d",
            ChartInterval.FiveDays => "5d",
            ChartInterval.OneWeek => "1wk",
            ChartInterval.OneMonth => "1mo",
            ChartInterval.ThreeMonths => "3mo",
            _ => "1d"
        };
    }
}