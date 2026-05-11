using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CyFinance.Models.HistoricalData;

namespace CyFinance.Services.HistoricalData;

public interface IHistoricalDataService
{
    /// <summary>
    /// Gets raw chart response data for a ticker.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <param name="startDate">Optional inclusive start date.</param>
    /// <param name="endDate">Optional inclusive end date.</param>
    /// <param name="interval">The chart interval.</param>
    /// <param name="includeDividends">Whether to include dividend events.</param>
    /// <param name="includeSplits">Whether to include split events.</param>
    /// <returns>The full chart response payload.</returns>
    Task<ChartResponse> GetHistoricalDataAsync(
        string ticker,
        DateTime? startDate = null,
        DateTime? endDate = null,
        ChartInterval interval = ChartInterval.OneDay,
        bool includeDividends = true,
        bool includeSplits = true);

    /// <summary>
    /// Gets normalized historical price records for a ticker.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <param name="startDate">Optional inclusive start date.</param>
    /// <param name="endDate">Optional inclusive end date.</param>
    /// <param name="interval">The chart interval.</param>
    /// <param name="includeDividends">Whether to include dividend events.</param>
    /// <param name="includeSplits">Whether to include split events.</param>
    /// <returns>A list of historical prices.</returns>
    Task<List<HistoricalPrice>> GetHistoricalPricesAsync(
        string ticker,
        DateTime? startDate = null,
        DateTime? endDate = null,
        ChartInterval interval = ChartInterval.OneDay,
        bool includeDividends = true,
        bool includeSplits = true);

    /// <summary>
    /// Extracts dividend events from a chart response.
    /// </summary>
    /// <param name="chartResponse">The chart response to inspect.</param>
    /// <returns>A list of dividend entries.</returns>
    List<DividendInfo> GetDividends(ChartResponse chartResponse);

    /// <summary>
    /// Extracts split events from a chart response.
    /// </summary>
    /// <param name="chartResponse">The chart response to inspect.</param>
    /// <returns>A list of split entries.</returns>
    List<SplitInfo> GetSplits(ChartResponse chartResponse);
}
