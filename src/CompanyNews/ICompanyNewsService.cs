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
    /// Yahoo deduplicates the news it selected rather than selecting that many distinct
    /// stories, so fewer than <paramref name="newsCount" /> items commonly come back, and
    /// for a heavily covered ticker a small request can come back empty. Ask for more than
    /// you need rather than treating the count as a promise.
    /// </remarks>
    Task<List<CompanyNewsItem>?> GetCompanyNewsAsync(string ticker, int newsCount = 10);

    /// <summary>
    /// Gets the latest available company news item for a ticker.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <returns>The latest news item, or null when no data is available.</returns>
    /// <remarks>
    /// Requests a small window and returns its first item, because Yahoo answers a request
    /// for a single item with nothing at all often enough to matter. See
    /// <see cref="GetCompanyNewsAsync" />.
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
    /// The window is filtered client side, and Yahoo may return fewer items than
    /// <paramref name="newsCount" /> asked for, so an empty result means "nothing recent in
    /// what Yahoo sent", not "nothing published since". See <see cref="GetCompanyNewsAsync" />.
    /// </remarks>
    Task<List<CompanyNewsItem>?> GetCompanyNewsSinceAsync(string ticker, long sinceUnixTime, int newsCount = 25);
}
