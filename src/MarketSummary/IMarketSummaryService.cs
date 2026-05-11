using CyFinance.Models.MarketSummary;

namespace CyFinance.Services.MarketSummary;

public interface IMarketSummaryService
{
    /// <summary>
    /// Gets market summary values keyed by exchange for a market code.
    /// </summary>
    /// <param name="market">The market code (for example, US).</param>
    /// <returns>A dictionary of summary items by exchange, or null when unavailable.</returns>
    Task<Dictionary<string, MarketSummaryItem>?> GetMarketSummaryAsync(string market = "US");

    /// <summary>
    /// Gets open and close status metadata for a market code.
    /// </summary>
    /// <param name="market">The market code (for example, US).</param>
    /// <returns>The market status, or null when unavailable.</returns>
    Task<MarketStatus?> GetMarketStatusAsync(string market = "US");

    /// <summary>
    /// Gets a combined market snapshot containing both summary and status data.
    /// </summary>
    /// <param name="market">The market code (for example, US).</param>
    /// <returns>The combined snapshot, or null when unavailable.</returns>
    Task<MarketSnapshot?> GetMarketSnapshotAsync(string market = "US");
}