using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CyFinance.Models.HistoricalData;

namespace CyFinance.Services.HistoricalData;

/// <summary>
/// Provides price history, dividends and splits.
/// </summary>
public interface IHistoricalDataService
{
    /// <summary>
    /// Gets the chart payload as Yahoo returns it, including the events and metadata that
    /// <see cref="GetHistoricalPricesAsync" /> discards.
    /// </summary>
    /// <param name="startDate">Inclusive start date. Defaults to one year ago.</param>
    /// <param name="endDate">Inclusive end date. Defaults to now.</param>
    Task<ChartResponse> GetHistoricalDataAsync(
        string ticker,
        DateTime? startDate = null,
        DateTime? endDate = null,
        ChartInterval interval = ChartInterval.OneDay,
        bool includeDividends = true,
        bool includeSplits = true);

    /// <summary>
    /// Gets one OHLCV record per candle.
    /// </summary>
    /// <param name="startDate">Inclusive start date. Defaults to one year ago.</param>
    /// <param name="endDate">Inclusive end date. Defaults to now.</param>
    Task<List<HistoricalPrice>> GetHistoricalPricesAsync(
        string ticker,
        DateTime? startDate = null,
        DateTime? endDate = null,
        ChartInterval interval = ChartInterval.OneDay,
        bool includeDividends = true,
        bool includeSplits = true);

    /// <summary>
    /// Reads the dividend events out of a chart payload. Empty unless it was fetched with
    /// <c>includeDividends</c>.
    /// </summary>
    List<DividendInfo> GetDividends(ChartResponse chartResponse);

    /// <summary>
    /// Reads the split events out of a chart payload. Empty unless it was fetched with
    /// <c>includeSplits</c>.
    /// </summary>
    List<SplitInfo> GetSplits(ChartResponse chartResponse);
}
