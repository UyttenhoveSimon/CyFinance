using System.Threading.Tasks;
using CyFinance.Models.StockScreening;

namespace CyFinance.Services.StockScreening;

/// <summary>
/// Runs Yahoo's stock screener, either with your own query or with one of its saved screens.
/// </summary>
public interface IStockScreeningService
{
    /// <summary>
    /// Runs a screener request you have built yourself.
    /// </summary>
    Task<ScreenerResult?> ScreenAsync(ScreenerRequest request);

    /// <summary>
    /// Runs a screener query built from <see cref="EquityQuery" /> or <see cref="ScreenerQuery" />.
    /// </summary>
    /// <param name="sortField">A Yahoo field name, such as <c>ticker</c> or <c>intradaymarketcap</c>.</param>
    /// <param name="userId">Forwarded to Yahoo as-is. Screening works without it.</param>
    /// <param name="userIdType">Forwarded to Yahoo as-is. Screening works without it.</param>
    Task<ScreenerResult?> ScreenAsync(
        QueryBase query,
        int size = 25,
        int offset = 0,
        string sortField = "ticker",
        bool sortAsc = false,
        string userId = "",
        string userIdType = "guid");

    /// <summary>
    /// Runs one of Yahoo's saved screens by id.
    /// </summary>
    /// <param name="screenId">
    /// A Yahoo screen id, such as <c>day_gainers</c>. <see cref="PredefinedScreeners" /> holds
    /// the known ones, and the overload taking <see cref="PredefinedScreenersCatalogItem" />
    /// avoids the string altogether.
    /// </param>
    /// <param name="userId">Forwarded to Yahoo as-is. Screening works without it.</param>
    /// <param name="userIdType">Forwarded to Yahoo as-is. Screening works without it.</param>
    Task<ScreenerResult?> ScreenPredefinedAsync(
        string screenId,
        int? offset = null,
        int? count = null,
        string? sortField = null,
        bool? sortAsc = null,
        string? userId = null,
        string? userIdType = null);

    /// <summary>
    /// Runs one of Yahoo's saved screens.
    /// </summary>
    /// <param name="userId">Forwarded to Yahoo as-is. Screening works without it.</param>
    /// <param name="userIdType">Forwarded to Yahoo as-is. Screening works without it.</param>
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
