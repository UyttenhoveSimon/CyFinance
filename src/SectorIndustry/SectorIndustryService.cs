using CyFinance.Models.SectorIndustry;
using CyFinance.Models.StockScreening;
using CyFinance.Services.QuoteSummary;
using CyFinance.Services.StockScreening;

namespace CyFinance.Services.SectorIndustry;

/// <summary>
/// Sector / Industry API backed by Yahoo Finance quoteSummary (assetProfile) and the screener.
/// </summary>
public class SectorIndustryService : ISectorIndustryService
{
    private readonly IQuoteSummaryService _quoteSummaryService;
    private readonly IStockScreeningService _screeningService;

    public SectorIndustryService(
        IQuoteSummaryService quoteSummaryService,
        IStockScreeningService screeningService)
    {
        _quoteSummaryService = quoteSummaryService
            ?? throw new ArgumentNullException(nameof(quoteSummaryService));
        _screeningService = screeningService
            ?? throw new ArgumentNullException(nameof(screeningService));
    }

    public async Task<SectorIndustryInfo?> GetSectorInfoAsync(string ticker)
    {
        if (string.IsNullOrWhiteSpace(ticker))
            throw new ArgumentException("Ticker cannot be empty", nameof(ticker));

        try
        {
            var response = await _quoteSummaryService.GetQuoteSummaryAsync(ticker, "assetProfile");

            if (response?.QuoteSummary?.Result == null || response.QuoteSummary.Result.Count == 0)
                return null;

            var profile = response.QuoteSummary.Result[0].AssetProfile;
            if (profile is null) return null;

            return new SectorIndustryInfo
            {
                Ticker = ticker,
                Sector = profile.Sector,
                SectorKey = profile.SectorKey,
                Industry = profile.Industry,
                IndustryKey = profile.IndustryKey,
                Website = profile.Website,
                Country = profile.Country,
                FullTimeEmployees = profile.FullTimeEmployees
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get sector info for {ticker}: {ex.Message}", ex);
        }
    }

    public async Task<List<SectorScreenerEntry>> GetStocksInSectorAsync(string sector, int size = 25)
    {
        if (string.IsNullOrWhiteSpace(sector))
            throw new ArgumentException("Sector cannot be empty", nameof(sector));

        try
        {
            var request = new ScreenerRequest
            {
                Size = size,
                SortField = "marketcap",
                SortType = "DESC",
                QuoteType = "EQUITY",
                Query = ScreenerQuery.Eq("sector", sector)
            };

            var result = await _screeningService.ScreenAsync(request);
            return MapQuotes(result);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get stocks in sector '{sector}': {ex.Message}", ex);
        }
    }

    public async Task<List<SectorScreenerEntry>> GetStocksInIndustryAsync(string industry, int size = 25)
    {
        if (string.IsNullOrWhiteSpace(industry))
            throw new ArgumentException("Industry cannot be empty", nameof(industry));

        try
        {
            var request = new ScreenerRequest
            {
                Size = size,
                SortField = "marketcap",
                SortType = "DESC",
                QuoteType = "EQUITY",
                Query = ScreenerQuery.Eq("industry", industry)
            };

            var result = await _screeningService.ScreenAsync(request);
            return MapQuotes(result);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get stocks in industry '{industry}': {ex.Message}", ex);
        }
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private static List<SectorScreenerEntry> MapQuotes(ScreenerResult? result)
    {
        if (result?.Quotes is null) return new List<SectorScreenerEntry>();

        return result.Quotes
            .Select(q => new SectorScreenerEntry
            {
                Symbol = q.Symbol,
                Name = q.LongName,
                Exchange = q.Exchange,
                Price = q.RegularMarketPrice,
                Change = q.RegularMarketChange,
                ChangePercent = q.RegularMarketChangePercent,
                MarketCap = q.MarketCap,
                Volume = q.RegularMarketVolume
            })
            .ToList();
    }
}
