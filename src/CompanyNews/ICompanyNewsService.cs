using CyFinance.Models.CompanyNews;

namespace CyFinance.Services.CompanyNews;

/// <summary>
/// Provides news for a ticker.
/// </summary>
public interface ICompanyNewsService
{
    /// <summary>
    /// Gets recent news, in the order Yahoo sent it.
    /// </summary>
    /// <param name="newsCount">
    /// An upper bound. Yahoo trims the result to that many items and only then drops the
    /// stories it considers duplicates, so fewer usually come back, and for a heavily
    /// covered ticker a small request can come back empty. Ask for more than you need.
    /// </param>
    Task<List<CompanyNewsItem>?> GetCompanyNewsAsync(string ticker, int newsCount = 10);

    /// <summary>
    /// Gets the most recent news item.
    /// </summary>
    /// <remarks>
    /// Asks Yahoo for several items and returns the first, since a request for one item
    /// often comes back empty. See <see cref="GetCompanyNewsAsync" />.
    /// </remarks>
    Task<CompanyNewsItem?> GetLatestCompanyNewsAsync(string ticker);

    /// <summary>
    /// Gets the news published at or after a point in time.
    /// </summary>
    /// <param name="sinceUnixTime">The cutoff, as a Unix timestamp in seconds.</param>
    /// <param name="newsCount">
    /// How many items to fetch before filtering. Filtering happens in memory over whatever
    /// Yahoo returned, which is usually fewer, so an empty result means nothing recent was
    /// in that window rather than nothing published since. See <see cref="GetCompanyNewsAsync" />.
    /// </param>
    Task<List<CompanyNewsItem>?> GetCompanyNewsSinceAsync(string ticker, long sinceUnixTime, int newsCount = 25);
}
