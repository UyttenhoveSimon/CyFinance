using CyFinance.Models.QuoteSummary;

namespace CyFinance.Models.ShareholderInformation;

/// <summary>
/// Aggregates shareholder and insider ownership information for a ticker.
/// </summary>
public class ShareholderInformationSummary
{
    public string? Ticker { get; set; }
    public MajorHoldersBreakdown? MajorHoldersBreakdown { get; set; }
    public List<OwnershipEntry>? InstitutionalOwnership { get; set; }
    public List<OwnershipEntry>? FundOwnership { get; set; }
    public List<InsiderHolderEntry>? InsiderHolders { get; set; }
    public List<InsiderTransactionEntry>? InsiderTransactions { get; set; }

    public double? GetInstitutionalOwnershipPercent()
    {
        return MajorHoldersBreakdown?.InstitutionsPercentHeld?.Raw * 100;
    }

    public double? GetInsiderOwnershipPercent()
    {
        return MajorHoldersBreakdown?.InsidersPercentHeld?.Raw * 100;
    }

    public OwnershipEntry? GetLargestInstitutionalHolder()
    {
        return InstitutionalOwnership?
            .OrderByDescending(x => x.Position?.Raw ?? 0)
            .FirstOrDefault();
    }
}
