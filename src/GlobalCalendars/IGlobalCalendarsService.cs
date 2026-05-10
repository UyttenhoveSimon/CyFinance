using CyFinance.Models.GlobalCalendars;

namespace CyFinance.Services.GlobalCalendars;

public interface IGlobalCalendarsService
{
    Task<List<EarningsCalendarEvent>> GetEarningsCalendarAsync(
        DateTime? start = null,
        DateTime? end = null,
        int limit = 12,
        int offset = 0,
        double? marketCap = null,
        bool filterMostActive = true);

    Task<List<IpoCalendarEvent>> GetIpoCalendarAsync(
        DateTime? start = null,
        DateTime? end = null,
        int limit = 12,
        int offset = 0);

    Task<List<EconomicEvent>> GetEconomicEventsCalendarAsync(
        DateTime? start = null,
        DateTime? end = null,
        int limit = 12,
        int offset = 0);

    Task<List<SplitCalendarEvent>> GetSplitsCalendarAsync(
        DateTime? start = null,
        DateTime? end = null,
        int limit = 12,
        int offset = 0);
}