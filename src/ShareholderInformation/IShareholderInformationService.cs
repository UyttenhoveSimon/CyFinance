using CyFinance.Models.QuoteSummary;
using CyFinance.Models.ShareholderInformation;

namespace CyFinance.Services.ShareholderInformation;

/// <summary>
/// Interface for shareholder information operations.
/// </summary>
public interface IShareholderInformationService
{
    /// <summary>
    /// Gets combined shareholder information for a ticker.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <returns>The shareholder summary, or null when unavailable.</returns>
    Task<ShareholderInformationSummary?> GetShareholderInformationAsync(string ticker);

    /// <summary>
    /// Gets major holders breakdown values for a ticker.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <returns>The major holders breakdown, or null when unavailable.</returns>
    Task<MajorHoldersBreakdown?> GetMajorHoldersBreakdownAsync(string ticker);

    /// <summary>
    /// Gets institutional ownership entries for a ticker.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <returns>A list of institutional ownership entries, or null when unavailable.</returns>
    Task<List<OwnershipEntry>?> GetInstitutionalOwnershipAsync(string ticker);

    /// <summary>
    /// Gets fund ownership entries for a ticker.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <returns>A list of fund ownership entries, or null when unavailable.</returns>
    Task<List<OwnershipEntry>?> GetFundOwnershipAsync(string ticker);

    /// <summary>
    /// Gets insider holder entries for a ticker.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <returns>A list of insider holder entries, or null when unavailable.</returns>
    Task<List<InsiderHolderEntry>?> GetInsiderHoldersAsync(string ticker);

    /// <summary>
    /// Gets insider transaction entries for a ticker.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <returns>A list of insider transaction entries, or null when unavailable.</returns>
    Task<List<InsiderTransactionEntry>?> GetInsiderTransactionsAsync(string ticker);
}
