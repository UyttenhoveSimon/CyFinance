using CyFinance.Models.Search;

namespace CyFinance.Services.Search
{
    /// <summary>
    /// Interface for Yahoo Finance Search API
    /// Provides search functionality for tickers, news, and other financial data
    /// Similar to yfinance.Ticker.info['search']
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// Search for tickers, news, and other financial data
        /// </summary>
        /// <param name="query">The search query string</param>
        /// <param name="quotesCount">Number of quote results to return (default: 8)</param>
        /// <param name="newsCount">Number of news results to return (default: 4)</param>
        /// <returns>SearchResponse containing quotes, news, research, and nav results</returns>
        Task<SearchResponse?> SearchAsync(string query, int quotesCount = 8, int newsCount = 4);

        /// <summary>
        /// Search for quotes/tickers only
        /// </summary>
        /// <param name="query">The search query string</param>
        /// <param name="quotesCount">Number of quote results to return (default: 8)</param>
        /// <returns>List of matching quotes</returns>
        Task<List<SearchQuote>?> SearchQuotesAsync(string query, int quotesCount = 8);

        /// <summary>
        /// Search for news only
        /// </summary>
        /// <param name="query">The search query string</param>
        /// <param name="newsCount">Number of news results to return (default: 4)</param>
        /// <returns>List of matching news items</returns>
        Task<List<SearchNews>?> SearchNewsAsync(string query, int newsCount = 4);
    }
}
