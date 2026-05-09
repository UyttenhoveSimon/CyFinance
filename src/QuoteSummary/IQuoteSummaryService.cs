using System.Threading.Tasks;
using CyFinance.Models.QuoteSummary;

namespace CyFinance.Services.QuoteSummary;


public interface IQuoteSummaryService
{
    Task<QuoteResponse?> GetQuoteSummaryAsync(string ticker, params string[] modules);
}
