using CyFinance.Models.HistoricalData;
using System.Net.Http.Json;

namespace CyFinance.Services.HistoricalData
{
    public class HistoricalDataService : BaseService, IHistoricalDataService
    {
        public HistoricalDataService(HttpClient client) : base(client)
        {
            if (!Client.DefaultRequestHeaders.Contains("User-Agent"))
            {
                Client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            }
        }
        private const string BASE_URL = "https://query2.finance.yahoo.com";

        public async Task<ChartResponse> GetHistoricalDataAsync(
            string ticker,
            DateTime? startDate = null,
            DateTime? endDate = null,
            ChartInterval interval = ChartInterval.OneDay,
            bool includeDividends = true,
            bool includeSplits = true)
        {
            try
            {
                await EnsureAuthenticatedAsync(ticker);
                var url = BuildChartUrl(ticker, startDate, endDate, interval, includeDividends, includeSplits);
                return await Client.GetFromJsonAsync<ChartResponse>(url, _jsonOptions);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve chart data for {ticker}: {ex.Message}", ex);
            }
        }
        public List<DividendInfo> GetDividends(ChartResponse chartResponse)
        {
            var dividends = chartResponse?.Chart?.Result?.FirstOrDefault()?.Events?.Dividends;
            if (dividends == null) return new List<DividendInfo>();

            return dividends
                .Select(kvp => kvp.Value)
                .Select(d => new DividendInfo
                {
                    Date = DateTimeOffset.FromUnixTimeSeconds(d.Date).DateTime,
                    Amount = d.Amount
                })
                .ToList();
        }
        public async Task<List<HistoricalPrice>> GetHistoricalPricesAsync(
            string ticker,
            DateTime? startDate = null,
            DateTime? endDate = null,
            ChartInterval interval = ChartInterval.OneDay,
            bool includeDividends = true,
            bool includeSplits = true)
        {
            var chartData = await GetHistoricalDataAsync(ticker, startDate, endDate, interval, includeDividends, includeSplits);
            return ConvertToHistoricalPrices(chartData);
        }

        public static StockMetaData GetMetadata(ChartResponse chartResponse)
        {
            return chartResponse?.Chart?.Result?.FirstOrDefault()?.Meta;
        }

        public List<SplitInfo> GetSplits(ChartResponse chartResponse)
        {
            var splits = chartResponse?.Chart?.Result?.FirstOrDefault()?.Events?.Splits;
            if (splits == null) return new List<SplitInfo>();

            return splits
                .Select(kvp => kvp.Value)
                .Select(s => new SplitInfo
                {
                    Date = DateTimeOffset.FromUnixTimeSeconds(s.Date).DateTime,
                    Numerator = s.Numerator,
                    Denominator = s.Denominator,
                    SplitRatio = s.SplitRatio
                })
                .ToList();
        }

        private string BuildChartUrl(
            string ticker,
            DateTime? startDate,
            DateTime? endDate,
            ChartInterval interval,
            bool includeDividends,
            bool includeSplits)
        {
            var url = $"{BASE_URL}/v8/finance/chart/{ticker}";
            var parameters = new List<string>();

            // Date range
            if (startDate.HasValue)
                parameters.Add($"period1={((DateTimeOffset)startDate.Value).ToUnixTimeSeconds()}");
            else
                parameters.Add($"period1={((DateTimeOffset)DateTime.Now.AddYears(-1)).ToUnixTimeSeconds()}"); // Default 1 year

            if (endDate.HasValue)
                parameters.Add($"period2={((DateTimeOffset)endDate.Value).ToUnixTimeSeconds()}");
            else
                parameters.Add($"period2={((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds()}"); // Default now

            // Interval
            parameters.Add($"interval={GetIntervalString(interval)}");

            // Events
            var events = new List<string>();
            if (includeDividends) events.Add("div");
            if (includeSplits) events.Add("split");
            if (events.Any())
                parameters.Add($"events={string.Join(",", events)}");

            // Additional parameters
            parameters.Add("includePrePost=true");
            parameters.Add("includeTimestamps=true");

            return parameters.Any() ? $"{url}?{string.Join("&", parameters)}" : url;
        }

        private List<HistoricalPrice> ConvertToHistoricalPrices(ChartResponse chartResponse)
        {
            var result = new List<HistoricalPrice>();

            if (chartResponse?.Chart?.Result?.FirstOrDefault() is not ChartResult chartResult)
                return result;

            var timestamps = chartResult.Timestamp;
            var quotes = chartResult.Indicators?.Quote?.FirstOrDefault();
            var adjClose = chartResult.Indicators?.AdjClose?.FirstOrDefault()?.AdjustedClose;

            if (timestamps == null || quotes == null) return result;

            for (int i = 0; i < timestamps.Count; i++)
            {
                var dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamps[i]).DateTime;

                result.Add(new HistoricalPrice
                {
                    Date = dateTime,
                    Open = GetValueAtIndex(quotes.Open, i),
                    High = GetValueAtIndex(quotes.High, i),
                    Low = GetValueAtIndex(quotes.Low, i),
                    Close = GetValueAtIndex(quotes.Close, i),
                    AdjustedClose = GetValueAtIndex(adjClose, i),
                    Volume = GetLongValueAtIndex(quotes.Volume, i)
                });
            }

            return result.OrderBy(x => x.Date).ToList();
        }

        private string GetIntervalString(ChartInterval interval)
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

        private static long? GetLongValueAtIndex(List<long?> list, int index)
        {
            return list != null && index < list.Count ? list[index] : null;
        }

        private static double? GetValueAtIndex(List<double?> list, int index)
        {
            return list != null && index < list.Count ? list[index] : null;
        }

    }
}