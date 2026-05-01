using CyFinance.Models.EarningsCalendar;
using CyFinance.Services.QuoteSummary;

namespace CyFinance.Services.EarningsCalendar;

/// <summary>
/// Earnings Calendar Service for Yahoo Finance
/// Retrieves upcoming earnings dates and historical earnings data
/// </summary>
public class EarningsCalendarService : IEarningsCalendarService
{
    private readonly IQuoteSummaryService _quoteSummaryService;

    public EarningsCalendarService(IQuoteSummaryService quoteSummaryService)
    {
        _quoteSummaryService = quoteSummaryService ?? throw new ArgumentNullException(nameof(quoteSummaryService));
    }

    /// <summary>
    /// Get upcoming earnings dates for a ticker
    /// </summary>
    public async Task<UpcomingEarnings?> GetUpcomingEarningsAsync(string ticker)
    {
        if (string.IsNullOrWhiteSpace(ticker))
            throw new ArgumentException("Ticker cannot be empty", nameof(ticker));

        try
        {
            var response = await _quoteSummaryService.GetQuoteSummaryAsync(ticker, "calendarEvents");

            if (response?.QuoteSummary?.Result == null || response.QuoteSummary.Result.Count == 0)
                return null;

            var result = response.QuoteSummary.Result[0];
            var earningsCalendar = result.CalendarEvents?.Earnings;

            if (earningsCalendar?.EarningsDate == null || earningsCalendar.EarningsDate.Count == 0)
                return null;

            return new UpcomingEarnings
            {
                Ticker = ticker,
                EarningsDates = earningsCalendar.EarningsDate
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get upcoming earnings for {ticker}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Get historical earnings data for a ticker
    /// </summary>
    public async Task<HistoricalEarnings?> GetHistoricalEarningsAsync(string ticker)
    {
        if (string.IsNullOrWhiteSpace(ticker))
            throw new ArgumentException("Ticker cannot be empty", nameof(ticker));

        try
        {
            var response = await _quoteSummaryService.GetQuoteSummaryAsync(ticker, "earnings");

            if (response?.QuoteSummary?.Result == null || response.QuoteSummary.Result.Count == 0)
                return null;

            var result = response.QuoteSummary.Result[0];
            var earnings = result.Earnings?.EarningsChart?.Quarterly;

            if (earnings == null || earnings.Count == 0)
                return null;

            return new HistoricalEarnings
            {
                Ticker = ticker,
                Quarterly = earnings
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get historical earnings for {ticker}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Get complete earnings calendar (upcoming dates and historical earnings)
    /// </summary>
    public async Task<EarningsCalendarSummary?> GetEarningsCalendarAsync(string ticker)
    {
        if (string.IsNullOrWhiteSpace(ticker))
            throw new ArgumentException("Ticker cannot be empty", nameof(ticker));

        try
        {
            var response = await _quoteSummaryService.GetQuoteSummaryAsync(ticker,
                "calendarEvents", "earnings");

            if (response?.QuoteSummary?.Result == null || response.QuoteSummary.Result.Count == 0)
                return null;

            var result = response.QuoteSummary.Result[0];

            return new EarningsCalendarSummary
            {
                Ticker = ticker,
                UpcomingEarningsDates = result.CalendarEvents?.Earnings?.EarningsDate,
                HistoricalEarnings = result.Earnings?.EarningsChart?.Quarterly
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get earnings calendar for {ticker}: {ex.Message}", ex);
        }
    }
}
