using CyFinance.Models.MarketSummary;

namespace CyFinance.Services.MarketSummary;

public interface IMarketSummaryService
{
    Task<Dictionary<string, MarketSummaryItem>?> GetMarketSummaryAsync(string market = "US");

    Task<MarketStatus?> GetMarketStatusAsync(string market = "US");

    Task<MarketSnapshot?> GetMarketSnapshotAsync(string market = "US");
}