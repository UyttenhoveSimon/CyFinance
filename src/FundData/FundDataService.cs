using CyFinance.Models.FundData;
using CyFinance.Models.QuoteSummary;
using CyFinance.Services.QuoteSummary;

namespace CyFinance.Services.FundData;

/// <summary>
/// Mutual fund / ETF data service backed by Yahoo Finance quoteSummary API.
/// Fetches <c>topHoldings</c>, <c>fundProfile</c>, <c>fundPerformance</c>, and <c>price</c> modules.
/// </summary>
public class FundDataService : IFundDataService
{
    private readonly IQuoteSummaryService _quoteSummaryService;

    public FundDataService(IQuoteSummaryService quoteSummaryService)
    {
        _quoteSummaryService = quoteSummaryService
            ?? throw new ArgumentNullException(nameof(quoteSummaryService));
    }

    public async Task<FundSummary?> GetFundSummaryAsync(string ticker)
    {
        ValidateTicker(ticker);

        try
        {
            var response = await _quoteSummaryService.GetQuoteSummaryAsync(
                ticker, "price", "topHoldings", "fundProfile", "fundPerformance");

            var result = GetResult(response);
            if (result is null) return null;

            return new FundSummary
            {
                Ticker = ticker,
                Name = result.Price?.LongName ?? result.Price?.ShortName,
                FundFamily = result.FundProfile?.Family,
                Category = result.FundProfile?.CategoryName,
                LegalType = result.FundProfile?.LegalType,
                Price = result.Price?.RegularMarketPrice?.Raw,
                ExpenseRatio = result.FundProfile?.FeesExpensesInvestment?.AnnualReportExpenseRatio?.Raw,
                StockPosition = result.TopHoldings?.StockPosition?.Raw,
                BondPosition = result.TopHoldings?.BondPosition?.Raw,
                TopHoldings = MapHoldings(result.TopHoldings),
                SectorWeightings = MapSectorWeightings(result.TopHoldings),
                TrailingReturns = MapTrailingReturns(result.FundPerformance)
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get fund summary for {ticker}: {ex.Message}", ex);
        }
    }

    public async Task<FundProfile?> GetFundProfileAsync(string ticker)
    {
        ValidateTicker(ticker);

        try
        {
            var response = await _quoteSummaryService.GetQuoteSummaryAsync(
                ticker, "fundProfile");

            var result = GetResult(response);
            if (result?.FundProfile is null) return null;

            var fp = result.FundProfile;
            return new FundProfile
            {
                Ticker = ticker,
                FundFamily = fp.Family,
                Category = fp.CategoryName,
                LegalType = fp.LegalType,
                ManagerName = fp.ManagementInfo?.ManagerName,
                ExpenseRatio = fp.FeesExpensesInvestment?.AnnualReportExpenseRatio?.Raw,
                GrossExpenseRatio = fp.FeesExpensesInvestment?.GrossExpRatio?.Raw,
                FrontEndSalesLoad = fp.FeesExpensesInvestment?.FrontEndSalesLoad?.Raw
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get fund profile for {ticker}: {ex.Message}", ex);
        }
    }

    public async Task<List<FundTopHolding>?> GetTopHoldingsAsync(string ticker)
    {
        ValidateTicker(ticker);

        try
        {
            var response = await _quoteSummaryService.GetQuoteSummaryAsync(
                ticker, "topHoldings");

            var result = GetResult(response);
            return result is null ? null : MapHoldings(result.TopHoldings);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get top holdings for {ticker}: {ex.Message}", ex);
        }
    }

    public async Task<List<FundSectorWeighting>?> GetSectorWeightingsAsync(string ticker)
    {
        ValidateTicker(ticker);

        try
        {
            var response = await _quoteSummaryService.GetQuoteSummaryAsync(
                ticker, "topHoldings");

            var result = GetResult(response);
            return result is null ? null : MapSectorWeightings(result.TopHoldings);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get sector weightings for {ticker}: {ex.Message}", ex);
        }
    }

    public async Task<FundTrailingSummary?> GetTrailingReturnsAsync(string ticker)
    {
        ValidateTicker(ticker);

        try
        {
            var response = await _quoteSummaryService.GetQuoteSummaryAsync(
                ticker, "fundPerformance");

            var result = GetResult(response);
            return result is null ? null : MapTrailingReturns(result.FundPerformance);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get trailing returns for {ticker}: {ex.Message}", ex);
        }
    }

    public async Task<List<FundAnnualReturnEntry>?> GetAnnualReturnsAsync(string ticker)
    {
        ValidateTicker(ticker);

        try
        {
            var response = await _quoteSummaryService.GetQuoteSummaryAsync(
                ticker, "fundPerformance");

            var result = GetResult(response);
            var returns = result?.FundPerformance?.AnnualTotalReturns?.Returns;
            if (returns is null) return null;

            return returns
                .Select(r => new FundAnnualReturnEntry
                {
                    Year = r.Year,
                    ReturnPercent = r.AnnualValue?.Raw
                })
                .ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get annual returns for {ticker}: {ex.Message}", ex);
        }
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private static void ValidateTicker(string ticker)
    {
        if (string.IsNullOrWhiteSpace(ticker))
            throw new ArgumentException("Ticker cannot be empty", nameof(ticker));
    }

    private static QuoteResult? GetResult(QuoteResponse? response)
    {
        if (response?.QuoteSummary?.Result == null || response.QuoteSummary.Result.Count == 0)
            return null;
        return response.QuoteSummary.Result[0];
    }

    private static List<FundTopHolding>? MapHoldings(TopHoldingsData? data)
    {
        if (data?.Holdings is null) return null;
        return data.Holdings
            .Select(h => new FundTopHolding
            {
                Symbol = h.Symbol,
                Name = h.HoldingName,
                Percent = h.HoldingPercent?.Raw
            })
            .ToList();
    }

    private static List<FundSectorWeighting>? MapSectorWeightings(TopHoldingsData? data)
    {
        if (data?.SectorWeightings is null) return null;

        var list = new List<FundSectorWeighting>();
        foreach (var dict in data.SectorWeightings)
        {
            foreach (var kv in dict)
            {
                list.Add(new FundSectorWeighting
                {
                    Sector = kv.Key,
                    Weight = kv.Value?.Raw
                });
            }
        }

        return list;
    }

    private static FundTrailingSummary? MapTrailingReturns(FundPerformanceData? data)
    {
        var tr = data?.TrailingReturns;
        if (tr is null) return null;

        return new FundTrailingSummary
        {
            OneMonth = tr.OneMonth?.Raw,
            ThreeMonth = tr.ThreeMonth?.Raw,
            Ytd = tr.Ytd?.Raw,
            OneYear = tr.OneYear?.Raw,
            ThreeYear = tr.ThreeYear?.Raw,
            FiveYear = tr.FiveYear?.Raw,
            TenYear = tr.TenYear?.Raw
        };
    }
}
