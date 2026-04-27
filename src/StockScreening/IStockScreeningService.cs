namespace CyFinance.Services.StockScreening
{
    using CyFinance.Models.StockScreening;
    using System.Threading.Tasks;

    public interface IStockScreeningService
    {
        Task<ScreenerResult?> ScreenAsync(ScreenerRequest request);
        Task<ScreenerResult?> ScreenAsync(
            QueryBase query,
            int size = 25,
            int offset = 0,
            string sortField = "ticker",
            bool sortAsc = false,
            string userId = "",
            string userIdType = "guid");

        Task<ScreenerResult?> ScreenPredefinedAsync(
            string screenId,
            int? offset = null,
            int? count = null,
            string? sortField = null,
            bool? sortAsc = null,
            string? userId = null,
            string? userIdType = null);

        Task<ScreenerResult?> ScreenPredefinedAsync(
            PredefinedScreenersCatalogItem screen,
            int? offset = null,
            int? count = null,
            string? sortField = null,
            bool? sortAsc = null,
            string? userId = null,
            string? userIdType = null);
    }

    public enum PredefinedScreenersCatalogItem
    {
        AggressiveSmallCaps,
        DayGainers,
        DayLosers,
        MostActives,
        MostShortedStocks,
        GrowthTechnologyStocks,
        SmallCapGainers,
        UndervaluedGrowthStocks,
        UndervaluedLargeCaps,
        ConservativeForeignFunds,
        HighYieldBond,
        PortfolioAnchors,
        SolidLargeBlendFunds,
        SolidLargeGrowthFunds,
        SolidMidcapGrowthFunds,
        TopMutualFunds,
        TopEtfsUs,
        TopPerformingEtfs,
        TechnologyEtfs,
        BondEtfs,
    }
}