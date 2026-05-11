using System.Globalization;
using System.Text.Json;
using CyFinance.Models.MarketSummary;

namespace CyFinance.Services.MarketSummary;

public class MarketSummaryService : BaseService, IMarketSummaryService
{
    private const string BaseUrl = "https://query1.finance.yahoo.com";
    private const string MarketSummaryUrl = BaseUrl + "/v6/finance/quote/marketSummary";
    private const string MarketTimeUrl = BaseUrl + "/v6/finance/markettime";
    private static readonly string[] SummaryFields =
    [
        "shortName",
        "regularMarketPrice",
        "regularMarketChange",
        "regularMarketChangePercent"
    ];

    public MarketSummaryService(HttpClient client) : base(client)
    {
        if (!Client.DefaultRequestHeaders.Contains("User-Agent"))
        {
            Client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        }
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, MarketSummaryItem>?> GetMarketSummaryAsync(string market = "US")
    {
        ValidateMarket(market);

        try
        {
            var response = await Client.GetAsync(BuildMarketSummaryUrl(market));
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Yahoo Market Summary API returned {(int) response.StatusCode}: {content}");
            }

            var parsed = JsonSerializer.Deserialize(content, MarketSummaryJsonSerializerContext.Default.MarketSummaryRoot);
            var result = parsed?.MarketSummaryResponse?.Result;
            if (result is null)
            {
                return null;
            }

            return result
                .Where(item => !string.IsNullOrWhiteSpace(item.Exchange))
                .ToDictionary(
                    item => item.Exchange!,
                    item => new MarketSummaryItem
                    {
                        Exchange = item.Exchange,
                        ShortName = item.ShortName,
                        RegularMarketPrice = item.RegularMarketPrice?.Raw,
                        RegularMarketChange = item.RegularMarketChange?.Raw,
                        RegularMarketChangePercent = item.RegularMarketChangePercent?.Raw,
                    },
                    StringComparer.OrdinalIgnoreCase);
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Failed to get market summary for '{market}': {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<MarketStatus?> GetMarketStatusAsync(string market = "US")
    {
        ValidateMarket(market);

        try
        {
            var response = await Client.GetAsync(BuildMarketStatusUrl(market));
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Yahoo Market Time API returned {(int) response.StatusCode}: {content}");
            }

            var parsed = JsonSerializer.Deserialize(content, MarketSummaryJsonSerializerContext.Default.MarketTimeRoot);
            var entry = parsed?.Finance?.MarketTimes?.FirstOrDefault()?.MarketTime?.FirstOrDefault();
            if (entry is null)
            {
                return null;
            }

            var timezone = entry.Timezone?.FirstOrDefault();
            return new MarketStatus
            {
                Market = market,
                TimezoneName = timezone?.Name,
                TimezoneShortName = timezone?.Short,
                GmtOffsetMilliseconds = timezone?.GmtOffset,
                Open = ParseDateTimeOffset(entry.Open),
                Close = ParseDateTimeOffset(entry.Close),
            };
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Failed to get market status for '{market}': {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<MarketSnapshot?> GetMarketSnapshotAsync(string market = "US")
    {
        ValidateMarket(market);

        var summary = await GetMarketSummaryAsync(market);
        var status = await GetMarketStatusAsync(market);

        if (summary is null && status is null)
        {
            return null;
        }

        return new MarketSnapshot
        {
            Market = market,
            Status = status,
            SummaryByExchange = summary,
        };
    }

    private static string BuildMarketSummaryUrl(string market)
    {
        return $"{MarketSummaryUrl}?fields={Uri.EscapeDataString(string.Join(',', SummaryFields))}&formatted=false&lang=en-US&market={Uri.EscapeDataString(market)}";
    }

    private static string BuildMarketStatusUrl(string market)
    {
        return $"{MarketTimeUrl}?formatted=true&key=finance&lang=en-US&market={Uri.EscapeDataString(market)}";
    }

    private static void ValidateMarket(string market)
    {
        if (string.IsNullOrWhiteSpace(market))
        {
            throw new ArgumentException("Market cannot be empty", nameof(market));
        }
    }

    private static DateTimeOffset? ParseDateTimeOffset(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed)
            ? parsed
            : null;
    }
}