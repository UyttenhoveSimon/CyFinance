using CyFinance.Models.GlobalCalendars;

namespace CyFinance.Services.GlobalCalendars;

/// <summary>
/// Provides market-wide calendars, as opposed to the per-ticker
/// <see cref="CyFinance.Services.EarningsCalendar.IEarningsCalendarService" />.
/// </summary>
/// <remarks>
/// Every method covers the week starting today unless given a date range, and pages through
/// results with <c>limit</c> and <c>offset</c>.
/// </remarks>
public interface IGlobalCalendarsService
{
    /// <summary>
    /// Gets companies reporting earnings in the window.
    /// </summary>
    /// <param name="marketCap">A minimum intraday market capitalisation to filter on.</param>
    /// <param name="filterMostActive">
    /// Narrows the results to the most actively traded symbols, which keeps a week of
    /// earnings down to the names most people are watching. Ignored past the first page.
    /// </param>
    Task<List<EarningsCalendarEvent>> GetEarningsCalendarAsync(
        DateTime? start = null,
        DateTime? end = null,
        int limit = 12,
        int offset = 0,
        double? marketCap = null,
        bool filterMostActive = true);

    /// <summary>
    /// Gets companies going public in the window.
    /// </summary>
    Task<List<IpoCalendarEvent>> GetIpoCalendarAsync(
        DateTime? start = null,
        DateTime? end = null,
        int limit = 12,
        int offset = 0);

    /// <summary>
    /// Gets macroeconomic releases in the window.
    /// </summary>
    Task<List<EconomicEvent>> GetEconomicEventsCalendarAsync(
        DateTime? start = null,
        DateTime? end = null,
        int limit = 12,
        int offset = 0);

    /// <summary>
    /// Gets stock splits taking effect in the window.
    /// </summary>
    Task<List<SplitCalendarEvent>> GetSplitsCalendarAsync(
        DateTime? start = null,
        DateTime? end = null,
        int limit = 12,
        int offset = 0);
}
