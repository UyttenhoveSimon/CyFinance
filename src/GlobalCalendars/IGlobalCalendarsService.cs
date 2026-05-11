using CyFinance.Models.GlobalCalendars;

namespace CyFinance.Services.GlobalCalendars;

public interface IGlobalCalendarsService
{
    /// <summary>
    /// Gets upcoming earnings calendar events.
    /// </summary>
    /// <param name="start">Optional inclusive start date.</param>
    /// <param name="end">Optional inclusive end date.</param>
    /// <param name="limit">The maximum number of results to return.</param>
    /// <param name="offset">The paging offset.</param>
    /// <param name="marketCap">Optional minimum market capitalization filter.</param>
    /// <param name="filterMostActive">Whether to intersect with most-active symbols when possible.</param>
    /// <returns>A list of earnings events.</returns>
    Task<List<EarningsCalendarEvent>> GetEarningsCalendarAsync(
        DateTime? start = null,
        DateTime? end = null,
        int limit = 12,
        int offset = 0,
        double? marketCap = null,
        bool filterMostActive = true);

    /// <summary>
    /// Gets IPO calendar events.
    /// </summary>
    /// <param name="start">Optional inclusive start date.</param>
    /// <param name="end">Optional inclusive end date.</param>
    /// <param name="limit">The maximum number of results to return.</param>
    /// <param name="offset">The paging offset.</param>
    /// <returns>A list of IPO events.</returns>
    Task<List<IpoCalendarEvent>> GetIpoCalendarAsync(
        DateTime? start = null,
        DateTime? end = null,
        int limit = 12,
        int offset = 0);

    /// <summary>
    /// Gets economic events calendar entries.
    /// </summary>
    /// <param name="start">Optional inclusive start date.</param>
    /// <param name="end">Optional inclusive end date.</param>
    /// <param name="limit">The maximum number of results to return.</param>
    /// <param name="offset">The paging offset.</param>
    /// <returns>A list of economic events.</returns>
    Task<List<EconomicEvent>> GetEconomicEventsCalendarAsync(
        DateTime? start = null,
        DateTime? end = null,
        int limit = 12,
        int offset = 0);

    /// <summary>
    /// Gets stock split calendar events.
    /// </summary>
    /// <param name="start">Optional inclusive start date.</param>
    /// <param name="end">Optional inclusive end date.</param>
    /// <param name="limit">The maximum number of results to return.</param>
    /// <param name="offset">The paging offset.</param>
    /// <returns>A list of split events.</returns>
    Task<List<SplitCalendarEvent>> GetSplitsCalendarAsync(
        DateTime? start = null,
        DateTime? end = null,
        int limit = 12,
        int offset = 0);
}