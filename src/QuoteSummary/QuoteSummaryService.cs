using System.Net;
using System.Text.Json;
using CyFinance.Models.QuoteSummary;

namespace CyFinance.QuoteSummary
{
    public class QuoteSummaryService : BaseService
    {
        private const string BASE_URL = "https://query2.finance.yahoo.com";
        public QuoteSummaryService(HttpClient client) : base(client)
        {
            if (!Client.DefaultRequestHeaders.Contains("User-Agent"))
            {
                Client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            }
        }

        /// <summary>
        /// Fetches quote summary data from Yahoo Finance for a given ticker and modules
        /// </summary>
        public async Task<QuoteResponse?> GetQuoteSummaryAsync(
            string ticker, params string[] modules)
        {
            // Ensure we have a valid crumb
            await EnsureAuthenticatedAsync(ticker);

            var modulesList = modules.Length > 0
                ? modules
                : new[] { "price", "summaryDetail", "assetProfile", "financialData" };

            var url = BuildApiUrl(ticker, modulesList);

            try
            {
                var response = await Client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                // Check for invalid crumb error
                if (response.StatusCode == HttpStatusCode.Unauthorized ||
                    content.Contains("Invalid Crumb", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Crumb expired or invalid, refreshing...");

                    // Force refresh and retry once
                    _crumb = null;
                    await RefreshAuthTokenAsync(ticker);
                    url = BuildApiUrl(ticker, modulesList);

                    response = await Client.GetAsync(url);
                    content = await response.Content.ReadAsStringAsync();
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Yahoo API returned {(int)response.StatusCode}: {content}");
                }

                return JsonSerializer.Deserialize<QuoteResponse>(content, _jsonOptions);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to get quote summary for {ticker}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Builds the API URL with crumb parameter
        /// </summary>
        private string BuildApiUrl(string ticker, string[] modules)
        {
            var url = $"{BASE_URL}/v10/finance/quoteSummary/{ticker}?modules={string.Join(",", modules)}";

            if (!string.IsNullOrEmpty(_crumb))
            {
                url += $"&crumb={Uri.EscapeDataString(_crumb)}";
            }

            return url;
        }
    }
}