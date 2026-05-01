using CyFinance.Models.EarningsCalendar;

namespace CyFinance.Services.EarningsCalendar;

/// <summary>
/// Interface for earnings calendar operations
/// </summary>
public interface IEarningsCalendarService
{
    /// <summary>
    /// Get upcoming earnings dates for a ticker
    /// </summary>
    /// <param name="ticker">Stock ticker symbol</param>
    /// <returns>Upcoming earnings data or null if not found</returns>
    Task<UpcomingEarnings?> GetUpcomingEarningsAsync(string ticker);

    /// <summary>
    /// Get historical earnings data for a ticker
    /// </summary>
    /// <param name="ticker">Stock ticker symbol</param>
    /// <returns>Historical earnings data or null if not found</returns>
    Task<HistoricalEarnings?> GetHistoricalEarningsAsync(string ticker);

    /// <summary>
    /// Get complete earnings calendar (upcoming dates and historical earnings)
    /// </summary>
    /// <param name="ticker">Stock ticker symbol</param>
    /// <returns>Complete earnings calendar summary or null if not found</returns>
    Task<EarningsCalendarSummary?> GetEarningsCalendarAsync(string ticker);
}
