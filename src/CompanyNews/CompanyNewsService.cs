using System.Text.Json;
using CyFinance.Models.CompanyNews;

namespace CyFinance.Services.CompanyNews;

/// <summary>
/// Company News Service for Yahoo Finance.
/// Retrieves ticker-related news similarly to yfinance ticker news workflows.
/// </summary>
public class CompanyNewsService : BaseService, ICompanyNewsService
{
    private const string BaseUrl = "https://query2.finance.yahoo.com";

    public CompanyNewsService(HttpClient client) : base(client)
    {
        if (!Client.DefaultRequestHeaders.Contains("User-Agent"))
        {
            Client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        }
    }

    /// <summary>
    /// Get ticker-specific company news.
    /// </summary>
    public async Task<List<CompanyNewsItem>?> GetCompanyNewsAsync(string ticker, int newsCount = 10)
    {
        if (string.IsNullOrWhiteSpace(ticker))
            throw new ArgumentException("Ticker cannot be empty", nameof(ticker));

        if (newsCount < 0)
            throw new ArgumentException("newsCount must be non-negative", nameof(newsCount));

        try
        {
            var url = BuildCompanyNewsUrl(ticker, newsCount);
            var response = await Client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Yahoo API returned {(int)response.StatusCode}");

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CompanyNewsResponse>(content, _jsonOptions);
            return result?.News;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Failed to get company news for '{ticker}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Get the latest news article for a ticker.
    /// </summary>
    public async Task<CompanyNewsItem?> GetLatestCompanyNewsAsync(string ticker)
    {
        var news = await GetCompanyNewsAsync(ticker, 1);
        return news?.FirstOrDefault();
    }

    /// <summary>
    /// Get news newer than a given Unix timestamp.
    /// </summary>
    public async Task<List<CompanyNewsItem>?> GetCompanyNewsSinceAsync(string ticker, long sinceUnixTime, int newsCount = 25)
    {
        if (sinceUnixTime < 0)
            throw new ArgumentException("sinceUnixTime must be non-negative", nameof(sinceUnixTime));

        var news = await GetCompanyNewsAsync(ticker, newsCount);
        return news?
            .Where(n => (n.ProviderPublishTime ?? 0) >= sinceUnixTime)
            .ToList();
    }

    private static string BuildCompanyNewsUrl(string ticker, int newsCount)
    {
        return $"{BaseUrl}/v1/finance/search?q={Uri.EscapeDataString(ticker)}&quotesCount=0&newsCount={newsCount}&enableFuzzyQuery=false&enableNewsDedup=true";
    }
}
