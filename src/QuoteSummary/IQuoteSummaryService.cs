using System.Threading.Tasks;
using CyFinance.Models.QuoteSummary;

namespace CyFinance.Services.QuoteSummary;

/// <summary>
/// Provides raw access to Yahoo's quote summary modules, which most other services build on.
/// </summary>
public interface IQuoteSummaryService
{
    /// <summary>
    /// Gets the named quote summary modules for a ticker.
    /// </summary>
    /// <param name="modules">
    /// Yahoo module names. Passing none requests <c>price</c>, <c>summaryDetail</c>,
    /// <c>assetProfile</c> and <c>financialData</c>.
    /// </param>
    Task<QuoteResponse?> GetQuoteSummaryAsync(string ticker, params string[] modules);
}
