using System.Threading.Tasks;
using CyFinance.Models.QuoteSummary;

namespace CyFinance.Services.QuoteSummary;


public interface IQuoteSummaryService
{
    /// <summary>
    /// Gets Yahoo Finance quote summary modules for a ticker.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <param name="modules">The quote summary modules to request.</param>
    /// <returns>The quote summary response, or null when unavailable.</returns>
    Task<QuoteResponse?> GetQuoteSummaryAsync(string ticker, params string[] modules);
}
