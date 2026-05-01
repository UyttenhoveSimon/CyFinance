using CyFinance.Models.QuoteSummary;

namespace CyFinance.Models.EarningsCalendar;

/// <summary>
/// Represents upcoming earnings dates for a company
/// </summary>
public class UpcomingEarnings
{
    /// <summary>
    /// Ticker symbol
    /// </summary>
    public string? Ticker { get; set; }

    /// <summary>
    /// List of upcoming earnings dates (Unix timestamps)
    /// </summary>
    public List<YahooLongValue>? EarningsDates { get; set; }
}

/// <summary>
/// Represents historical earnings data for a company
/// </summary>
public class HistoricalEarnings
{
    /// <summary>
    /// Ticker symbol
    /// </summary>
    public string? Ticker { get; set; }

    /// <summary>
    /// List of quarterly earnings with actual and estimate values
    /// </summary>
    public List<QuarterlyEarnings>? Quarterly { get; set; }
}

/// <summary>
/// Represents complete earnings calendar data (upcoming dates and historical earnings)
/// </summary>
public class EarningsCalendarSummary
{
    /// <summary>
    /// Ticker symbol
    /// </summary>
    public string? Ticker { get; set; }

    /// <summary>
    /// Upcoming earnings dates
    /// </summary>
    public List<YahooLongValue>? UpcomingEarningsDates { get; set; }

    /// <summary>
    /// Historical quarterly earnings data
    /// </summary>
    public List<QuarterlyEarnings>? HistoricalEarnings { get; set; }

    /// <summary>
    /// Get the next earnings date if available
    /// </summary>
    /// <returns>Next earnings date as Unix timestamp or null</returns>
    public long? GetNextEarningsDate()
    {
        return UpcomingEarningsDates?.FirstOrDefault()?.Raw;
    }

    /// <summary>
    /// Get the most recent earnings date from historical data if available
    /// </summary>
    /// <returns>Most recent earnings date or null</returns>
    public string? GetMostRecentEarningsDate()
    {
        return HistoricalEarnings?.FirstOrDefault()?.Date;
    }

    /// <summary>
    /// Get the most recent earnings data with actual and estimate
    /// </summary>
    /// <returns>Most recent quarterly earnings or null</returns>
    public QuarterlyEarnings? GetMostRecentEarnings()
    {
        return HistoricalEarnings?.FirstOrDefault();
    }

    /// <summary>
    /// Calculate average earnings surprise (difference between actual and estimate)
    /// </summary>
    /// <returns>Average surprise percentage or 0 if no data</returns>
    public double GetAverageEarningsSurprise()
    {
        if (HistoricalEarnings == null || HistoricalEarnings.Count == 0)
            return 0;

        var validEarnings = HistoricalEarnings
            .Where(e => e.Actual?.Raw.HasValue == true && e.Estimate?.Raw.HasValue == true && e.Estimate.Raw > 0)
            .ToList();

        if (validEarnings.Count == 0)
            return 0;

        var surprises = validEarnings.Select(e =>
            ((e.Actual!.Raw!.Value - e.Estimate!.Raw!.Value) / e.Estimate.Raw.Value) * 100
        );

        return surprises.Average();
    }

    /// <summary>
    /// Get earnings beat rate (percentage of times actual beat estimate)
    /// </summary>
    /// <returns>Beat rate as percentage (0-100) or 0 if no data</returns>
    public double GetEarningsBeatRate()
    {
        if (HistoricalEarnings == null || HistoricalEarnings.Count == 0)
            return 0;

        var validEarnings = HistoricalEarnings
            .Where(e => e.Actual?.Raw.HasValue == true && e.Estimate?.Raw.HasValue == true)
            .ToList();

        if (validEarnings.Count == 0)
            return 0;

        var beats = validEarnings.Count(e => e.Actual!.Raw > e.Estimate!.Raw);
        return (beats / (double)validEarnings.Count) * 100;
    }
}
