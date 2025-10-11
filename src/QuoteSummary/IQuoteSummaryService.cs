using CyFinance.Models.QuoteSummary;
using System.Threading.Tasks;

namespace CyFinance.Services.QuoteSummary
{

    public interface IQuoteSummaryService
    {
        Task<QuoteResponse?> GetQuoteSummaryAsync(string ticker, params string[] modules);
    }
}