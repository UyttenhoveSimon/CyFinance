using System.Net;
using System.Text;
using System.Text.Json;
using CyFinance.Models.StockScreening;

namespace CyFinance.Services.StockScreening
{
    public class StockScreeningService : BaseService, IStockScreeningService
    {
        private const string BASE_URL = "https://query2.finance.yahoo.com";
        private const string ScreenerUrl = $"{BASE_URL}/v1/finance/screener";
        private const string PredefinedUrl = $"{ScreenerUrl}/predefined/saved";

        public StockScreeningService(HttpClient client) : base(client)
        {
            if (!Client.DefaultRequestHeaders.Contains("User-Agent"))
            {
                Client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            }
        }

        /// <summary>
        /// Executes a custom screener query (equivalent to yfinance screen(query=...)).
        /// </summary>
        public async Task<ScreenerResult?> ScreenAsync(ScreenerRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            ValidateRange(request.Size, nameof(request.Size));

            var payload = JsonSerializer.Serialize(request, _jsonOptions);
            var url = BuildScreenerUrl();

            try
            {
                var response = await Client.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"));
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Unauthorized ||
                    responseBody.Contains("Invalid Crumb", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Crumb expired or invalid, refreshing...");
                    _crumb = null;
                    await RefreshAuthTokenAsync("AAPL");

                    response = await Client.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json"));
                    responseBody = await response.Content.ReadAsStringAsync();
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Yahoo Screener API returned {(int)response.StatusCode}: {responseBody}");
                }

                var parsed = JsonSerializer.Deserialize<ScreenerResponse>(responseBody, _jsonOptions);
                return parsed?.Finance?.Result?.FirstOrDefault();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to execute screener query: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Executes a predefined screener query by id (equivalent to yfinance screen("day_gainers")).
        /// </summary>
        public async Task<ScreenerResult?> ScreenPredefinedAsync(
            string screenId,
            int? offset = null,
            int? count = null,
            string? sortField = null,
            bool? sortAsc = null,
            string? userId = null,
            string? userIdType = null)
        {
            if (string.IsNullOrWhiteSpace(screenId))
                throw new ArgumentException("Screen id cannot be null or empty", nameof(screenId));

            if (count.HasValue)
                ValidateRange(count.Value, nameof(count));

            var query = new List<string>
            {
                "corsDomain=finance.yahoo.com",
                "formatted=false",
                "lang=en-US",
                "region=US",
                $"scrIds={Uri.EscapeDataString(screenId)}"
            };

            if (offset.HasValue) query.Add($"offset={offset.Value}");
            if (count.HasValue) query.Add($"count={count.Value}");
            if (!string.IsNullOrWhiteSpace(sortField)) query.Add($"sortField={Uri.EscapeDataString(sortField)}");
            if (sortAsc.HasValue) query.Add($"sortAsc={sortAsc.Value.ToString().ToLowerInvariant()}");
            if (!string.IsNullOrWhiteSpace(userId)) query.Add($"userId={Uri.EscapeDataString(userId)}");
            if (!string.IsNullOrWhiteSpace(userIdType)) query.Add($"userIdType={Uri.EscapeDataString(userIdType)}");

            var url = $"{PredefinedUrl}?{string.Join("&", query)}";

            try
            {
                var response = await Client.GetAsync(url);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Unauthorized ||
                    responseBody.Contains("Invalid Crumb", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Crumb expired or invalid, refreshing...");
                    _crumb = null;
                    await RefreshAuthTokenAsync("AAPL");

                    response = await Client.GetAsync(url);
                    responseBody = await response.Content.ReadAsStringAsync();
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Yahoo Screener predefined API returned {(int)response.StatusCode}: {responseBody}");
                }

                var parsed = JsonSerializer.Deserialize<ScreenerResponse>(responseBody, _jsonOptions);
                return parsed?.Finance?.Result?.FirstOrDefault();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to execute predefined screener query '{screenId}': {ex.Message}", ex);
            }
        }

        private string BuildScreenerUrl()
        {
            return $"{ScreenerUrl}?corsDomain=finance.yahoo.com&formatted=false&lang=en-US&region=US";
        }

        private static void ValidateRange(int value, string parameterName)
        {
            if (value < 1 || value > 250)
            {
                throw new ArgumentOutOfRangeException(parameterName, "Yahoo limits count/size to range [1, 250].");
            }
        }
    }
}