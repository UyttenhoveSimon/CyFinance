using CyFinance.Models.FundData;

namespace CyFinance.Services.FundData;

/// <summary>
/// Provides mutual fund and ETF data.
/// </summary>
public interface IFundDataService
{
    /// <summary>
    /// Gets the profile, holdings, sector weightings and returns in a single call.
    /// </summary>
    Task<FundSummary?> GetFundSummaryAsync(string ticker);

    /// <summary>
    /// Gets the fund family, category, expense ratio and managers.
    /// </summary>
    Task<FundProfile?> GetFundProfileAsync(string ticker);

    /// <summary>
    /// Gets the largest holdings Yahoo discloses, which is usually the top ten.
    /// </summary>
    Task<List<FundTopHolding>?> GetTopHoldingsAsync(string ticker);

    /// <summary>
    /// Gets the share of the fund in each sector.
    /// </summary>
    Task<List<FundSectorWeighting>?> GetSectorWeightingsAsync(string ticker);

    /// <summary>
    /// Gets returns over trailing periods, from one month to ten years, plus year to date.
    /// </summary>
    Task<FundTrailingSummary?> GetTrailingReturnsAsync(string ticker);

    /// <summary>
    /// Gets one return per calendar year.
    /// </summary>
    Task<List<FundAnnualReturnEntry>?> GetAnnualReturnsAsync(string ticker);
}
