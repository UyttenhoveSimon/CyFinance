using CyFinance.Models.FundData;

namespace CyFinance.Services.FundData;

/// <summary>
/// Service contract for mutual fund and ETF data.
/// </summary>
public interface IFundDataService
{
    /// <summary>Returns an aggregated summary of a fund or ETF.</summary>
    Task<FundSummary?> GetFundSummaryAsync(string ticker);

    /// <summary>Returns profile / metadata (family, category, expense ratio, manager).</summary>
    Task<FundProfile?> GetFundProfileAsync(string ticker);

    /// <summary>Returns the top holdings list for a fund or ETF.</summary>
    Task<List<FundTopHolding>?> GetTopHoldingsAsync(string ticker);

    /// <summary>Returns sector allocation weightings for a fund or ETF.</summary>
    Task<List<FundSectorWeighting>?> GetSectorWeightingsAsync(string ticker);

    /// <summary>Returns trailing-period returns (1m, 3m, YTD, 1y, 3y, 5y, 10y).</summary>
    Task<FundTrailingSummary?> GetTrailingReturnsAsync(string ticker);

    /// <summary>Returns calendar-year annual returns.</summary>
    Task<List<FundAnnualReturnEntry>?> GetAnnualReturnsAsync(string ticker);
}
