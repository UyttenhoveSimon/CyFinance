using CyFinance.Models.MarketSummary;

namespace CyFinance.Services.MarketSummary;

/// <summary>
/// Provides index levels and trading hours for a market.
/// </summary>
public interface IMarketSummaryService
{
    /// <summary>
    /// Gets the market's summary values, keyed by exchange.
    /// </summary>
    /// <param name="market">A market code, for example US.</param>
    Task<Dictionary<string, MarketSummaryItem>?> GetMarketSummaryAsync(string market = "US");

    /// <summary>
    /// Gets the market's session times and time zone.
    /// </summary>
    /// <param name="market">A market code, for example US.</param>
    Task<MarketStatus?> GetMarketStatusAsync(string market = "US");

    /// <summary>
    /// Gets the summary and the status in a single call.
    /// </summary>
    /// <param name="market">A market code, for example US.</param>
    Task<MarketSnapshot?> GetMarketSnapshotAsync(string market = "US");
}
