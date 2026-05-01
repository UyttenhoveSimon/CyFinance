using CyFinance.Models.ShareholderInformation;
using CyFinance.Services.QuoteSummary;

namespace CyFinance.Services.ShareholderInformation;

/// <summary>
/// Shareholder Information Service for Yahoo Finance.
/// Provides major holders, institutional/fund ownership, and insider ownership data.
/// </summary>
public class ShareholderInformationService : IShareholderInformationService
{
    private readonly IQuoteSummaryService _quoteSummaryService;

    public ShareholderInformationService(IQuoteSummaryService quoteSummaryService)
    {
        _quoteSummaryService = quoteSummaryService ?? throw new ArgumentNullException(nameof(quoteSummaryService));
    }

    public async Task<ShareholderInformationSummary?> GetShareholderInformationAsync(string ticker)
    {
        if (string.IsNullOrWhiteSpace(ticker))
            throw new ArgumentException("Ticker cannot be empty", nameof(ticker));

        try
        {
            var response = await _quoteSummaryService.GetQuoteSummaryAsync(ticker,
                "majorHoldersBreakdown",
                "institutionOwnership",
                "fundOwnership",
                "insiderHolders",
                "insiderTransactions");

            if (response?.QuoteSummary?.Result == null || response.QuoteSummary.Result.Count == 0)
                return null;

            var result = response.QuoteSummary.Result[0];

            return new ShareholderInformationSummary
            {
                Ticker = ticker,
                MajorHoldersBreakdown = result.MajorHoldersBreakdown,
                InstitutionalOwnership = result.InstitutionOwnership?.OwnershipList,
                FundOwnership = result.FundOwnership?.OwnershipList,
                InsiderHolders = result.InsiderHolders?.Holders,
                InsiderTransactions = result.InsiderTransactions?.Transactions
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get shareholder information for {ticker}: {ex.Message}", ex);
        }
    }

    public async Task<CyFinance.Models.QuoteSummary.MajorHoldersBreakdown?> GetMajorHoldersBreakdownAsync(string ticker)
    {
        var summary = await GetShareholderInformationAsync(ticker);
        return summary?.MajorHoldersBreakdown;
    }

    public async Task<List<CyFinance.Models.QuoteSummary.OwnershipEntry>?> GetInstitutionalOwnershipAsync(string ticker)
    {
        var summary = await GetShareholderInformationAsync(ticker);
        return summary?.InstitutionalOwnership;
    }

    public async Task<List<CyFinance.Models.QuoteSummary.OwnershipEntry>?> GetFundOwnershipAsync(string ticker)
    {
        var summary = await GetShareholderInformationAsync(ticker);
        return summary?.FundOwnership;
    }

    public async Task<List<CyFinance.Models.QuoteSummary.InsiderHolderEntry>?> GetInsiderHoldersAsync(string ticker)
    {
        var summary = await GetShareholderInformationAsync(ticker);
        return summary?.InsiderHolders;
    }

    public async Task<List<CyFinance.Models.QuoteSummary.InsiderTransactionEntry>?> GetInsiderTransactionsAsync(string ticker)
    {
        var summary = await GetShareholderInformationAsync(ticker);
        return summary?.InsiderTransactions;
    }
}
