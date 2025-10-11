namespace CyFinance.Services.StockScreening
{
    using CyFinance.Models.StockScreening;
    using System.Threading.Tasks;

    public interface IStockScreeningService
    {
        Task<QuoteResponse?> GetStockScreeningAsync(string ticker, params string[] modules);
    }
}