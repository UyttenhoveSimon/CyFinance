using CyFinance.Models.QuoteSummary;
using CyFinance.Models.ShareholderInformation;

namespace CyFinance.Services.ShareholderInformation;

/// <summary>
/// Interface for shareholder information operations.
/// </summary>
public interface IShareholderInformationService
{
    Task<ShareholderInformationSummary?> GetShareholderInformationAsync(string ticker);
    Task<MajorHoldersBreakdown?> GetMajorHoldersBreakdownAsync(string ticker);
    Task<List<OwnershipEntry>?> GetInstitutionalOwnershipAsync(string ticker);
    Task<List<OwnershipEntry>?> GetFundOwnershipAsync(string ticker);
    Task<List<InsiderHolderEntry>?> GetInsiderHoldersAsync(string ticker);
    Task<List<InsiderTransactionEntry>?> GetInsiderTransactionsAsync(string ticker);
}
