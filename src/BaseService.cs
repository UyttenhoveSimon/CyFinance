using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace CyFinance
{
    public abstract class BaseService
    {
        protected readonly HttpClient Client;
        protected readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private const string COOKIE_URL = "https://fc.yahoo.com";
        protected string? _crumb;
        private DateTime _crumbExpiry = DateTime.MinValue;
        private static readonly TimeSpan CrumbValidDuration = TimeSpan.FromHours(1);

        protected BaseService(HttpClient client)
        {
            Client = client;
        }

        /// <summary>
        /// Fetches cookies and crumb from Yahoo Finance (mimics yfinance approach)
        /// </summary>
        protected async Task RefreshAuthTokenAsync(string ticker = "AAPL")
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
        protected string? ExtractCrumb(string html)
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
        protected async Task EnsureAuthenticatedAsync(string ticker)
        {
            if (string.IsNullOrEmpty(_crumb) || DateTime.UtcNow >= _crumbExpiry)
            {
                await RefreshAuthTokenAsync(ticker);
            }
        }

    }
}