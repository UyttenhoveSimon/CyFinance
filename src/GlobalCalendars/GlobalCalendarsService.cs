using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using CyFinance.Models.GlobalCalendars;
using CyFinance.Services.StockScreening;

namespace CyFinance.Services.GlobalCalendars;

public class GlobalCalendarsService : BaseService, IGlobalCalendarsService
{
    private const string BaseUrl = "https://query1.finance.yahoo.com";
    private const string CalendarUrl = BaseUrl + "/v1/finance/visualization?lang=en-US&region=US";
    private const string DateFormat = "yyyy-MM-dd";
    private readonly IStockScreeningService _stockScreeningService;

    private static readonly CalendarDefinition EarningsDefinition = new(
        "sp_earnings",
        "intradaymarketcap",
        [
            "ticker",
            "companyshortname",
            "intradaymarketcap",
            "eventname",
            "startdatetime",
            "startdatetimetype",
            "epsestimate",
            "epsactual",
            "epssurprisepct"
        ]);

    private static readonly CalendarDefinition IpoDefinition = new(
        "ipo_info",
        "startdatetime",
        [
            "ticker",
            "companyshortname",
            "exchange_short_name",
            "filingdate",
            "startdatetime",
            "amendeddate",
            "pricefrom",
            "priceto",
            "offerprice",
            "currencyname",
            "shares",
            "dealtype"
        ]);

    private static readonly CalendarDefinition EconomicDefinition = new(
        "economic_event",
        "startdatetime",
        [
            "econ_release",
            "country_code",
            "startdatetime",
            "period",
            "after_release_actual",
            "consensus_estimate",
            "prior_release_actual",
            "originally_reported_actual"
        ]);

    private static readonly CalendarDefinition SplitsDefinition = new(
        "splits",
        "startdatetime",
        [
            "ticker",
            "companyshortname",
            "startdatetime",
            "optionable",
            "old_share_worth",
            "share_worth"
        ]);

    public GlobalCalendarsService(HttpClient client, IStockScreeningService stockScreeningService) : base(client)
    {
        _stockScreeningService = stockScreeningService ?? throw new ArgumentNullException(nameof(stockScreeningService));

        if (!Client.DefaultRequestHeaders.Contains("User-Agent"))
        {
            Client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        }
    }

    [RequiresDynamicCode("Uses JsonSerializer to materialize Yahoo Finance visualization responses")]
    [RequiresUnreferencedCode("Uses JsonSerializer to materialize Yahoo Finance visualization responses")]
    public async Task<List<EarningsCalendarEvent>> GetEarningsCalendarAsync(
        DateTime? start = null,
        DateTime? end = null,
        int limit = 12,
        int offset = 0,
        double? marketCap = null,
        bool filterMostActive = true)
    {
        ValidateLimitAndOffset(limit, offset);

        var (effectiveStart, effectiveEnd) = GetEffectiveRange(start, end);
        var query = And(
            Eq("region", "us"),
            Or(
                Eq("eventtype", "EAD"),
                Eq("eventtype", "ERA")),
            Gte("startdatetime", effectiveStart),
            Lte("startdatetime", effectiveEnd));

        if (marketCap.HasValue)
        {
            query.Append(Gte("intradaymarketcap", marketCap.Value));
        }

        if (filterMostActive && offset == 0)
        {
            var mostActive = await GetMostActiveQueryAsync(marketCap);
            if (mostActive.Operands.Count > 0)
            {
                query.Append(mostActive);
            }
        }

        var rows = await PostCalendarAsync(EarningsDefinition, query, limit, offset);
        return rows.Select(MapEarningsRow).ToList();
    }

    [RequiresDynamicCode("Uses JsonSerializer to materialize Yahoo Finance visualization responses")]
    [RequiresUnreferencedCode("Uses JsonSerializer to materialize Yahoo Finance visualization responses")]
    public async Task<List<IpoCalendarEvent>> GetIpoCalendarAsync(
        DateTime? start = null,
        DateTime? end = null,
        int limit = 12,
        int offset = 0)
    {
        ValidateLimitAndOffset(limit, offset);

        var (effectiveStart, effectiveEnd) = GetEffectiveRange(start, end);
        var query = Or(
            GtElt("startdatetime", effectiveStart, effectiveEnd),
            GtElt("filingdate", effectiveStart, effectiveEnd),
            GtElt("amendeddate", effectiveStart, effectiveEnd));

        var rows = await PostCalendarAsync(IpoDefinition, query, limit, offset);
        return rows.Select(MapIpoRow).ToList();
    }

    [RequiresDynamicCode("Uses JsonSerializer to materialize Yahoo Finance visualization responses")]
    [RequiresUnreferencedCode("Uses JsonSerializer to materialize Yahoo Finance visualization responses")]
    public async Task<List<EconomicEvent>> GetEconomicEventsCalendarAsync(
        DateTime? start = null,
        DateTime? end = null,
        int limit = 12,
        int offset = 0)
    {
        ValidateLimitAndOffset(limit, offset);

        var (effectiveStart, effectiveEnd) = GetEffectiveRange(start, end);
        var rows = await PostCalendarAsync(EconomicDefinition, BuildDateRangeQuery(effectiveStart, effectiveEnd), limit, offset);
        return rows.Select(MapEconomicRow).ToList();
    }

    [RequiresDynamicCode("Uses JsonSerializer to materialize Yahoo Finance visualization responses")]
    [RequiresUnreferencedCode("Uses JsonSerializer to materialize Yahoo Finance visualization responses")]
    public async Task<List<SplitCalendarEvent>> GetSplitsCalendarAsync(
        DateTime? start = null,
        DateTime? end = null,
        int limit = 12,
        int offset = 0)
    {
        ValidateLimitAndOffset(limit, offset);

        var (effectiveStart, effectiveEnd) = GetEffectiveRange(start, end);
        var rows = await PostCalendarAsync(SplitsDefinition, BuildDateRangeQuery(effectiveStart, effectiveEnd), limit, offset);
        return rows.Select(MapSplitsRow).ToList();
    }

    private async Task<List<List<JsonElement>>> PostCalendarAsync(
        CalendarDefinition definition,
        GlobalCalendarQueryNode query,
        int limit,
        int offset)
    {
        await EnsureAuthenticatedAsync("AAPL");

        var payload = new
        {
            sortType = "DESC",
            entityIdType = definition.CalendarType,
            sortField = definition.SortField,
            includeFields = definition.IncludeFields,
            size = Math.Min(limit, 100),
            offset,
            query,
        };

        var requestBody = JsonSerializer.Serialize(payload, _jsonOptions);
        var url = BuildCalendarUrl();

        try
        {
            var response = await Client.PostAsync(
                url,
                new StringContent(requestBody, Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.Unauthorized ||
                content.Contains("Invalid Crumb", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Crumb expired or invalid, refreshing...");
                _crumb = null;
                await RefreshAuthTokenAsync("AAPL");
                url = BuildCalendarUrl();

                response = await Client.PostAsync(
                    url,
                    new StringContent(requestBody, Encoding.UTF8, "application/json"));
                content = await response.Content.ReadAsStringAsync();
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Yahoo Global Calendars API returned {(int) response.StatusCode}: {content}");
            }

            var parsed = JsonSerializer.Deserialize<GlobalCalendarResponse>(content, _jsonOptions);
            if (parsed?.Finance?.Error is not null)
            {
                throw new Exception($"Yahoo Global Calendars API returned an error: {parsed.Finance.Error}");
            }

            return parsed?.Finance?.Result?.FirstOrDefault()?.Documents?.FirstOrDefault()?.Rows ?? [];
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Failed to retrieve {definition.CalendarType} calendar: {ex.Message}", ex);
        }
    }

    private string BuildCalendarUrl()
    {
        if (string.IsNullOrWhiteSpace(_crumb))
        {
            return CalendarUrl;
        }

        return CalendarUrl + "&crumb=" + Uri.EscapeDataString(_crumb);
    }

    private async Task<GlobalCalendarQueryNode> GetMostActiveQueryAsync(double? marketCap)
    {
        var result = await _stockScreeningService.ScreenPredefinedAsync(PredefinedScreenersCatalogItem.MostActives, count: 200);
        var operands = new List<object>();

        foreach (var stock in result?.Quotes ?? [])
        {
            if (string.IsNullOrWhiteSpace(stock.Symbol))
            {
                continue;
            }

            if (marketCap.HasValue && stock.MarketCap.HasValue && stock.MarketCap.Value < marketCap.Value)
            {
                continue;
            }

            operands.Add(Eq("ticker", stock.Symbol));
        }

        return new GlobalCalendarQueryNode("OR", operands);
    }

    private static (string Start, string End) GetEffectiveRange(DateTime? start, DateTime? end)
    {
        var effectiveStart = (start ?? DateTime.UtcNow.Date).ToString(DateFormat, CultureInfo.InvariantCulture);
        var effectiveEnd = (end ?? DateTime.ParseExact(effectiveStart, DateFormat, CultureInfo.InvariantCulture).AddDays(7))
            .ToString(DateFormat, CultureInfo.InvariantCulture);
        return (effectiveStart, effectiveEnd);
    }

    private static void ValidateLimitAndOffset(int limit, int offset)
    {
        if (limit < 1 || limit > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), "Yahoo limits calendar size to range [1, 100].");
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset), "Offset must be non-negative.");
        }
    }

    private static GlobalCalendarQueryNode BuildDateRangeQuery(string start, string end)
    {
        return And(
            Gte("startdatetime", start),
            Lte("startdatetime", end));
    }

    private static GlobalCalendarQueryNode Eq(string field, object value) =>
        new("EQ", [field, value]);

    private static GlobalCalendarQueryNode Gte(string field, object value) =>
        new("GTE", [field, value]);

    private static GlobalCalendarQueryNode Lte(string field, object value) =>
        new("LTE", [field, value]);

    private static GlobalCalendarQueryNode GtElt(string field, object lower, object upper) =>
        new("GTELT", [field, lower, upper]);

    private static GlobalCalendarQueryNode And(params GlobalCalendarQueryNode[] nodes) =>
        new("AND", nodes.Cast<object>());

    private static GlobalCalendarQueryNode Or(params GlobalCalendarQueryNode[] nodes) =>
        new("OR", nodes.Cast<object>());

    private static EarningsCalendarEvent MapEarningsRow(List<JsonElement> row)
    {
        return new EarningsCalendarEvent
        {
            Symbol = ReadString(row, 0),
            Company = ReadString(row, 1),
            MarketCap = ReadDouble(row, 2),
            EventName = ReadString(row, 3),
            EventStartDate = ReadDateTime(row, 4),
            Timing = ReadString(row, 5),
            EpsEstimate = ReadDouble(row, 6),
            ReportedEps = ReadDouble(row, 7),
            SurprisePercent = ReadDouble(row, 8),
        };
    }

    private static IpoCalendarEvent MapIpoRow(List<JsonElement> row)
    {
        return new IpoCalendarEvent
        {
            Symbol = ReadString(row, 0),
            Company = ReadString(row, 1),
            Exchange = ReadString(row, 2),
            FilingDate = ReadDateTime(row, 3),
            Date = ReadDateTime(row, 4),
            AmendedDate = ReadDateTime(row, 5),
            PriceFrom = ReadDouble(row, 6),
            PriceTo = ReadDouble(row, 7),
            Price = ReadDouble(row, 8),
            Currency = ReadString(row, 9),
            Shares = ReadDouble(row, 10),
            DealType = ReadString(row, 11),
        };
    }

    private static EconomicEvent MapEconomicRow(List<JsonElement> row)
    {
        return new EconomicEvent
        {
            Event = ReadString(row, 0),
            Region = ReadString(row, 1),
            EventTime = ReadDateTime(row, 2),
            Period = ReadString(row, 3),
            Actual = ReadDouble(row, 4),
            Expected = ReadDouble(row, 5),
            Last = ReadDouble(row, 6),
            Revised = ReadDouble(row, 7),
        };
    }

    private static SplitCalendarEvent MapSplitsRow(List<JsonElement> row)
    {
        return new SplitCalendarEvent
        {
            Symbol = ReadString(row, 0),
            Company = ReadString(row, 1),
            PayableOn = ReadDateTime(row, 2),
            Optionable = ReadBoolean(row, 3),
            OldShareWorth = ReadDouble(row, 4),
            ShareWorth = ReadDouble(row, 5),
        };
    }

    private static string? ReadString(IReadOnlyList<JsonElement> row, int index)
    {
        if (index >= row.Count)
        {
            return null;
        }

        var element = row[index];
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.GetRawText(),
            JsonValueKind.True => bool.TrueString,
            JsonValueKind.False => bool.FalseString,
            JsonValueKind.Null => null,
            JsonValueKind.Undefined => null,
            _ => element.GetRawText(),
        };
    }

    private static double? ReadDouble(IReadOnlyList<JsonElement> row, int index)
    {
        if (index >= row.Count)
        {
            return null;
        }

        var element = row[index];
        return element.ValueKind switch
        {
            JsonValueKind.Number when element.TryGetDouble(out var number) => number,
            JsonValueKind.String when double.TryParse(element.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed) => parsed,
            _ => null,
        };
    }

    private static bool? ReadBoolean(IReadOnlyList<JsonElement> row, int index)
    {
        if (index >= row.Count)
        {
            return null;
        }

        var element = row[index];
        return element.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String when bool.TryParse(element.GetString(), out var parsed) => parsed,
            _ => null,
        };
    }

    private static DateTime? ReadDateTime(IReadOnlyList<JsonElement> row, int index)
    {
        if (index >= row.Count)
        {
            return null;
        }

        var element = row[index];
        if (element.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        var value = element.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed)
            ? parsed
            : null;
    }

    private sealed record CalendarDefinition(string CalendarType, string SortField, string[] IncludeFields);
}