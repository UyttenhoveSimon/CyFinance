using CyFinance.Models.QuoteSummary;
using CyFinance.Models.ShareholderInformation;

namespace CyFinance.Services.ShareholderInformation;

/// <summary>
/// Provides who owns a company and what the insiders have been doing with their shares.
/// </summary>
public interface IShareholderInformationService
{
    /// <summary>
    /// Gets everything the other methods return, in a single call.
    /// </summary>
    Task<ShareholderInformationSummary?> GetShareholderInformationAsync(string ticker);

    /// <summary>
    /// Gets the split between insiders, institutions and everyone else.
    /// </summary>
    Task<MajorHoldersBreakdown?> GetMajorHoldersBreakdownAsync(string ticker);

    /// <summary>
    /// Gets the largest institutional holders.
    /// </summary>
    Task<List<OwnershipEntry>?> GetInstitutionalOwnershipAsync(string ticker);

    /// <summary>
    /// Gets the largest fund holders.
    /// </summary>
    Task<List<OwnershipEntry>?> GetFundOwnershipAsync(string ticker);

    /// <summary>
    /// Gets the insiders and what they hold.
    /// </summary>
    Task<List<InsiderHolderEntry>?> GetInsiderHoldersAsync(string ticker);

    /// <summary>
    /// Gets the insiders' recent buys and sells.
    /// </summary>
    Task<List<InsiderTransactionEntry>?> GetInsiderTransactionsAsync(string ticker);
}
