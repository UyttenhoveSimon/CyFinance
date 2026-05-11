using CyFinance.Models.CompanyNews;

namespace CyFinance.Services.CompanyNews;

/// <summary>
/// Interface for retrieving ticker-specific company news.
/// </summary>
public interface ICompanyNewsService
{
    /// <summary>
    /// Gets recent company news items for a ticker.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <param name="newsCount">The maximum number of items to return.</param>
    /// <returns>A list of news items, or null when no data is available.</returns>
    Task<List<CompanyNewsItem>?> GetCompanyNewsAsync(string ticker, int newsCount = 10);

    /// <summary>
    /// Gets the latest available company news item for a ticker.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <returns>The latest news item, or null when no data is available.</returns>
    Task<CompanyNewsItem?> GetLatestCompanyNewsAsync(string ticker);

    /// <summary>
    /// Gets company news items published on or after a Unix timestamp.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <param name="sinceUnixTime">The minimum publish time (Unix seconds).</param>
    /// <param name="newsCount">The maximum number of items to inspect.</param>
    /// <returns>A filtered list of news items, or null when no data is available.</returns>
    Task<List<CompanyNewsItem>?> GetCompanyNewsSinceAsync(string ticker, long sinceUnixTime, int newsCount = 25);
}
