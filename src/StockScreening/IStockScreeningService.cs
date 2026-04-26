namespace CyFinance.Services.StockScreening
{
    using CyFinance.Models.StockScreening;
    using System.Threading.Tasks;

    public interface IStockScreeningService
    {
        Task<ScreenerResult?> ScreenAsync(ScreenerRequest request);
        Task<ScreenerResult?> ScreenPredefinedAsync(
            string screenId,
            int? offset = null,
            int? count = null,
            string? sortField = null,
            bool? sortAsc = null,
            string? userId = null,
            string? userIdType = null);
    }
}