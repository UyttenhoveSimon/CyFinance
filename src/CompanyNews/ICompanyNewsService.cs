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
    /// <remarks>
    /// <paramref name="newsCount" /> is an upper bound. Yahoo trims the result to that many
    /// items and only then drops the stories it considers duplicates, so fewer usually come
    /// back, and for a heavily covered ticker a small request can come back empty. Ask for
    /// more than you need.
    /// </remarks>
    Task<List<CompanyNewsItem>?> GetCompanyNewsAsync(string ticker, int newsCount = 10);

    /// <summary>
    /// Gets the latest available company news item for a ticker.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <returns>The latest news item, or null when no data is available.</returns>
    /// <remarks>
    /// Asks Yahoo for several items and returns the first, since a request for one item
    /// often comes back empty. See <see cref="GetCompanyNewsAsync" />.
    /// </remarks>
    Task<CompanyNewsItem?> GetLatestCompanyNewsAsync(string ticker);

    /// <summary>
    /// Gets company news items published on or after a Unix timestamp.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <param name="sinceUnixTime">The minimum publish time (Unix seconds).</param>
    /// <param name="newsCount">The maximum number of items to inspect.</param>
    /// <returns>A filtered list of news items, or null when no data is available.</returns>
    /// <remarks>
    /// Filtering happens in memory over whatever Yahoo returned, which is usually fewer
    /// items than <paramref name="newsCount" /> asked for. An empty result therefore means
    /// nothing recent was in that window, not that nothing was published since. See
    /// <see cref="GetCompanyNewsAsync" />.
    /// </remarks>
    Task<List<CompanyNewsItem>?> GetCompanyNewsSinceAsync(string ticker, long sinceUnixTime, int newsCount = 25);
}
