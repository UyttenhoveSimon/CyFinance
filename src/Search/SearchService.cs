using System.Text.Json;
using CyFinance.Models.Search;

namespace CyFinance.Services.Search
{
    /// <summary>
    /// Yahoo Finance Search API Service
    /// Implements search functionality similar to yfinance
    /// </summary>
    public class SearchService : BaseService, ISearchService
    {
        private const string BASE_URL = "https://query2.finance.yahoo.com";

        public SearchService(HttpClient client) : base(client)
        {
            if (!Client.DefaultRequestHeaders.Contains("User-Agent"))
            {
                Client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            }
        }

        /// <summary>
        /// Search for tickers, news, and other financial data
        /// Equivalent to yfinance.Ticker.search(query)
        /// </summary>
        public async Task<SearchResponse?> SearchAsync(string query, int quotesCount = 8, int newsCount = 4)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentException("Search query cannot be empty", nameof(query));
            }

            if (quotesCount < 0 || newsCount < 0)
            {
                throw new ArgumentException("Counts must be non-negative");
            }

            try
            {
                var url = BuildSearchUrl(query, quotesCount, newsCount);
                var response = await Client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Yahoo API returned {(int)response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<SearchResponse>(content, _jsonOptions);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to search for '{query}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Search for quotes/tickers only
        /// </summary>
        public async Task<List<SearchQuote>?> SearchQuotesAsync(string query, int quotesCount = 8)
        {
            var result = await SearchAsync(query, quotesCount, 0);
            return result?.Quotes;
        }

        /// <summary>
        /// Search for news only
        /// </summary>
        public async Task<List<SearchNews>?> SearchNewsAsync(string query, int newsCount = 4)
        {
            var result = await SearchAsync(query, 0, newsCount);
            return result?.News;
        }

        /// <summary>
        /// Builds the search API URL with proper parameters
        /// </summary>
        private string BuildSearchUrl(string query, int quotesCount, int newsCount)
        {
            var url = $"{BASE_URL}/v1/finance/search?q={Uri.EscapeDataString(query)}";

            if (quotesCount > 0)
            {
                url += $"&quotesCount={quotesCount}";
            }

            if (newsCount > 0)
            {
                url += $"&newsCount={newsCount}";
            }

            // Optional: Include research and nav results
            url += "&enableFuzzyQuery=false&enableNewsDedup=true&enableResearchReports=true&enableCb=true&researchReportsCount=0";

            return url;
        }
    }
}
