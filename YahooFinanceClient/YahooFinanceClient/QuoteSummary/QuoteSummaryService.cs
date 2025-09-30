using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using YahooFinanceClient.Models.QuoteSummary;

namespace YahooFinanceClient.QuoteSummary
{
    public class QuoteSummaryService : BaseService
    {
        private const string BASE_URL = "https://query2.finance.yahoo.com";
        private const string COOKIE_URL = "https://fc.yahoo.com";
        private string? _crumb;
        private DateTime _crumbExpiry = DateTime.MinValue;
        private static readonly TimeSpan CrumbValidDuration = TimeSpan.FromHours(1);

        public QuoteSummaryService(HttpClient client) : base(client)
        {
            if (!Client.DefaultRequestHeaders.Contains("User-Agent"))
            {
                Client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            }
        }

        /// <summary>
        /// Fetches cookies and crumb from Yahoo Finance (mimics yfinance approach)
        /// </summary>
        private async Task RefreshAuthTokenAsync(string ticker = "AAPL")
        {
            try
            {
                // Step 1: Get cookies from fc.yahoo.com (like yfinance does)
                var cookieResponse = await Client.GetAsync(COOKIE_URL);
                
                if (!cookieResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Warning: fc.yahoo.com returned {cookieResponse.StatusCode}. Continuing anyway...");
                }

                // Step 2: Visit a quote page to get the crumb from HTML
                var quoteUrl = $"https://finance.yahoo.com/quote/{ticker}";
                var response = await Client.GetAsync(quoteUrl);
                response.EnsureSuccessStatusCode();

                var html = await response.Content.ReadAsStringAsync();

                // Step 3: Extract crumb using multiple regex patterns (Yahoo's structure changes)
                _crumb = ExtractCrumb(html);

                if (string.IsNullOrEmpty(_crumb))
                {
                    throw new Exception("Failed to extract crumb from Yahoo HTML. Structure may have changed.");
                }

                _crumbExpiry = DateTime.UtcNow.Add(CrumbValidDuration);
                Console.WriteLine($"Successfully obtained crumb: {_crumb.Substring(0, Math.Min(10, _crumb.Length))}...");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to refresh auth token: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Extracts crumb using multiple patterns to handle Yahoo's changing HTML structure
        /// </summary>
        private string? ExtractCrumb(string html)
        {
            // Pattern 1: CrumbStore object
            var patterns = new[]
            {
                @"""CrumbStore"":\s*\{\s*""crumb"":\s*""([^""]+)""",
                @"""crumb"":\s*""([^""]+)""",
                @"CrumbStore"":\{""crumb"":""([^""]+)""",
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
                if (match.Success && match.Groups.Count > 1)
                {
                    var crumb = match.Groups[1].Value;
                    // Unescape unicode sequences
                    crumb = Regex.Replace(crumb, @"\\u([0-9A-Fa-f]{4})", 
                        m => ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString());
                    return crumb.Trim();
                }
            }

            return null;
        }

        /// <summary>
        /// Ensures we have a valid crumb before making API calls
        /// </summary>
        private async Task EnsureAuthenticatedAsync(string ticker)
        {
            // Refresh if we don't have a crumb or if it's expired
            if (string.IsNullOrEmpty(_crumb) || DateTime.UtcNow >= _crumbExpiry)
            {
                await RefreshAuthTokenAsync(ticker);
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