
using System.Threading.Tasks;
using CyFinance.Models.StockScreening;

namespace CyFinance.Services.StockScreening;

public interface IStockScreeningService
{
    /// <summary>
    /// Executes a custom screener request payload.
    /// </summary>
    /// <param name="request">The screener request body.</param>
    /// <returns>The screener result, or null when no result is returned.</returns>
    Task<ScreenerResult?> ScreenAsync(ScreenerRequest request);

    /// <summary>
    /// Executes a custom screener query built from typed query helpers.
    /// </summary>
    /// <param name="query">The query expression.</param>
    /// <param name="size">Maximum number of rows to return.</param>
    /// <param name="offset">Paging offset.</param>
    /// <param name="sortField">Sort field name.</param>
    /// <param name="sortAsc">Whether to sort ascending.</param>
    /// <param name="userId">Optional user id forwarded to Yahoo.</param>
    /// <param name="userIdType">Optional user id type.</param>
    /// <returns>The screener result, or null when no result is returned.</returns>
    Task<ScreenerResult?> ScreenAsync(
        QueryBase query,
        int size = 25,
        int offset = 0,
        string sortField = "ticker",
        bool sortAsc = false,
        string userId = "",
        string userIdType = "guid");

    /// <summary>
    /// Executes a predefined screener by its Yahoo screener id.
    /// </summary>
    /// <param name="screenId">The predefined screener identifier.</param>
    /// <param name="offset">Optional paging offset.</param>
    /// <param name="count">Optional row count.</param>
    /// <param name="sortField">Optional sort field name.</param>
    /// <param name="sortAsc">Optional ascending sort flag.</param>
    /// <param name="userId">Optional user id forwarded to Yahoo.</param>
    /// <param name="userIdType">Optional user id type.</param>
    /// <returns>The screener result, or null when no result is returned.</returns>
    Task<ScreenerResult?> ScreenPredefinedAsync(
        string screenId,
        int? offset = null,
        int? count = null,
        string? sortField = null,
        bool? sortAsc = null,
        string? userId = null,
        string? userIdType = null);

    /// <summary>
    /// Executes a predefined screener using a typed catalog value.
    /// </summary>
    /// <param name="screen">The predefined screener enum value.</param>
    /// <param name="offset">Optional paging offset.</param>
    /// <param name="count">Optional row count.</param>
    /// <param name="sortField">Optional sort field name.</param>
    /// <param name="sortAsc">Optional ascending sort flag.</param>
    /// <param name="userId">Optional user id forwarded to Yahoo.</param>
    /// <param name="userIdType">Optional user id type.</param>
    /// <returns>The screener result, or null when no result is returned.</returns>
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
