using CyFinance.Models.Crypto;
using CyFinance.Models.HistoricalData;
using System.Net.Http.Json;

namespace CyFinance.Services.Crypto;

/// <summary>
/// Cryptocurrency API service backed by Yahoo chart data.
/// </summary>
public class CryptoService : BaseService, ICryptoService
{
    private const string BaseUrl = "https://query2.finance.yahoo.com";

    public CryptoService(HttpClient client) : base(client)
    {
        if (!Client.DefaultRequestHeaders.Contains("User-Agent"))
        {
            Client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        }
    }

    public async Task<CryptoQuote?> GetCryptoQuoteAsync(string cryptoSymbol, string quoteCurrency = "USD")
    {
        var normalizedBase = NormalizeAssetCode(cryptoSymbol, nameof(cryptoSymbol));
        var normalizedQuote = NormalizeAssetCode(quoteCurrency, nameof(quoteCurrency));
        var symbol = BuildCryptoSymbol(normalizedBase, normalizedQuote);

        var chart = await GetQuoteChartAsync(symbol);
        var result = chart?.Chart?.Result?.FirstOrDefault();
        var quote = result?.Indicators?.Quote?.FirstOrDefault();

        if (result == null || quote == null)
        {
            return null;
        }

        var (latestPrice, asOf) = GetLatestClose(quote.Close, result.Timestamp);
        var previousClose = result.Meta?.PreviousClose;
        double? change = latestPrice.HasValue && previousClose.HasValue
            ? latestPrice.Value - previousClose.Value
            : null;
        double? changePercent = change.HasValue && previousClose.HasValue && previousClose.Value != 0
            ? (change.Value / previousClose.Value) * 100
            : null;

        return new CryptoQuote
        {
            Symbol = symbol,
            BaseCurrency = normalizedBase,
            QuoteCurrency = normalizedQuote,
            Price = latestPrice,
            PreviousClose = previousClose,
            Change = change,
            ChangePercent = changePercent,
            AsOf = asOf
        };
    }

    public async Task<List<CryptoHistoricalPoint>> GetHistoricalPricesAsync(
        string cryptoSymbol,
        string quoteCurrency = "USD",
        DateTime? startDate = null,
        DateTime? endDate = null,
        ChartInterval interval = ChartInterval.OneDay)
    {
        var normalizedBase = NormalizeAssetCode(cryptoSymbol, nameof(cryptoSymbol));
        var normalizedQuote = NormalizeAssetCode(quoteCurrency, nameof(quoteCurrency));
        var symbol = BuildCryptoSymbol(normalizedBase, normalizedQuote);

        var chart = await GetHistoricalChartAsync(symbol, startDate, endDate, interval);
        var result = chart?.Chart?.Result?.FirstOrDefault();
        var quote = result?.Indicators?.Quote?.FirstOrDefault();

        if (result?.Timestamp == null || quote == null)
        {
            return new List<CryptoHistoricalPoint>();
        }

        var points = new List<CryptoHistoricalPoint>(result.Timestamp.Count);
        for (var i = 0; i < result.Timestamp.Count; i++)
        {
            points.Add(new CryptoHistoricalPoint
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

    private static string BuildCryptoSymbol(string cryptoSymbol, string quoteCurrency)
        => $"{cryptoSymbol}-{quoteCurrency}";

    private static string NormalizeAssetCode(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Asset code cannot be empty", paramName);
        }

        var normalized = value.Trim().ToUpperInvariant();
        if (normalized.Any(c => !(char.IsAsciiLetterOrDigit(c) || c == '-')))
        {
            throw new ArgumentException("Asset code can only contain letters, digits, and hyphens", paramName);
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