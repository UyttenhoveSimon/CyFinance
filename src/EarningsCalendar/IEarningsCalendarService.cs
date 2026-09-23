using CyFinance.Models.EarningsCalendar;

namespace CyFinance.Services.EarningsCalendar;

/// <summary>
/// Provides past and upcoming earnings dates.
/// </summary>
public interface IEarningsCalendarService
{
    /// <summary>
    /// Gets the announced earnings dates that have not happened yet.
    /// </summary>
    Task<UpcomingEarnings?> GetUpcomingEarningsAsync(string ticker);

    /// <summary>
    /// Gets the quarterly earnings already reported, with the estimates they were measured against.
    /// </summary>
    Task<HistoricalEarnings?> GetHistoricalEarningsAsync(string ticker);

    /// <summary>
    /// Gets the upcoming dates and the reported earnings in a single call.
    /// </summary>
    Task<EarningsCalendarSummary?> GetEarningsCalendarAsync(string ticker);
}
