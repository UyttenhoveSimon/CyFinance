using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace CyFinance;

public abstract class BaseService
{
    private const string SkipAuthHeader = "X-CyFinance-SkipAuth";
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
    /// Obtains a fresh crumb, the token Yahoo's data endpoints require alongside a session
    /// cookie. It is not served by an API: the cookie comes from one host and the crumb has
    /// to be scraped out of a quote page, which is the same dance yfinance performs.
    /// </summary>
    protected async Task RefreshAuthTokenAsync(string ticker = "AAPL")
    {
        try
        {
            // The cookie has to come from this host, not from finance.yahoo.com.
            var cookieResponse = await Client.GetAsync(COOKIE_URL);

            if (!cookieResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"Warning: fc.yahoo.com returned {cookieResponse.StatusCode}. Continuing anyway...");
            }

            // The crumb is only in the HTML of a quote page.
            var quoteUrl = $"https://finance.yahoo.com/quote/{ticker}";
            var response = await Client.GetAsync(quoteUrl);
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();

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
    /// Pulls the crumb out of a quote page, trying each known shape of Yahoo's markup in
    /// turn. Yahoo rewrites that page often enough that one pattern is not enough.
    /// </summary>
    protected string? ExtractCrumb(string html)
    {
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
                    m => ((char) Convert.ToInt32(m.Groups[1].Value, 16)).ToString());
                return crumb.Trim();
            }
        }

        return null;
    }

    /// <summary>
    /// Fetches a crumb if there is none or the cached one has expired.
    /// </summary>
    protected async Task EnsureAuthenticatedAsync(string ticker)
    {
        // Test-only bypass: mocked HttpClient instances can opt out of live Yahoo auth flow.
        if (Client.DefaultRequestHeaders.Contains(SkipAuthHeader))
        {
            return;
        }

        if (string.IsNullOrEmpty(_crumb) || DateTime.UtcNow >= _crumbExpiry)
        {
            await RefreshAuthTokenAsync(ticker);
        }
    }

}
