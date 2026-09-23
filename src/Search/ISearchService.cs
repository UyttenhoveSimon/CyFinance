using CyFinance.Models.Search;

namespace CyFinance.Services.Search;

/// <summary>
/// Searches Yahoo Finance by free text, the equivalent of the site's search box.
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// Searches for quotes, news, research reports and navigation links at once.
    /// </summary>
    /// <param name="query">A ticker, a company name, or anything else you would type in the search box.</param>
    Task<SearchResponse?> SearchAsync(string query, int quotesCount = 8, int newsCount = 4);

    /// <summary>
    /// Searches for matching instruments only.
    /// </summary>
    /// <param name="query">A ticker, a company name, or anything else you would type in the search box.</param>
    Task<List<SearchQuote>?> SearchQuotesAsync(string query, int quotesCount = 8);

    /// <summary>
    /// Searches for matching news only.
    /// </summary>
    /// <param name="query">A ticker, a company name, or anything else you would type in the search box.</param>
    Task<List<SearchNews>?> SearchNewsAsync(string query, int newsCount = 4);
}
