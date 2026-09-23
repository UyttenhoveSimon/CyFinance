using CyFinance.Models.QuoteSummary;

namespace CyFinance.Models.EarningsCalendar;

/// <summary>
/// Earnings dates a company has announced but not yet reported.
/// </summary>
public class UpcomingEarnings
{
    public string? Ticker { get; set; }

    /// <summary>
    /// Announcement dates, as Unix timestamps in seconds.
    /// </summary>
    public List<YahooLongValue>? EarningsDates { get; set; }
}

/// <summary>
/// Earnings a company has already reported.
/// </summary>
public class HistoricalEarnings
{
    public string? Ticker { get; set; }

    /// <summary>
    /// Reported quarters, in the order Yahoo sent them.
    /// </summary>
    public List<QuarterlyEarnings>? Quarterly { get; set; }
}

/// <summary>
/// A company's announced and reported earnings together.
/// </summary>
public class EarningsCalendarSummary
{
    public string? Ticker { get; set; }

    /// <summary>
    /// Announcement dates, as Unix timestamps in seconds.
    /// </summary>
    public List<YahooLongValue>? UpcomingEarningsDates { get; set; }

    /// <summary>
    /// Reported quarters, in the order Yahoo sent them.
    /// </summary>
    public List<QuarterlyEarnings>? HistoricalEarnings { get; set; }

    /// <summary>
    /// Gets the soonest announced date, as a Unix timestamp in seconds.
    /// </summary>
    public long? GetNextEarningsDate()
    {
        return UpcomingEarningsDates?.FirstOrDefault()?.Raw;
    }

    /// <summary>
    /// Gets the date of the first reported quarter Yahoo listed, in its own format such as "4Q2024".
    /// </summary>
    public string? GetMostRecentEarningsDate()
    {
        return HistoricalEarnings?.FirstOrDefault()?.Date;
    }

    /// <summary>
    /// Gets the first reported quarter Yahoo listed.
    /// </summary>
    public QuarterlyEarnings? GetMostRecentEarnings()
    {
        return HistoricalEarnings?.FirstOrDefault();
    }

    /// <summary>
    /// Averages how far each reported quarter landed from its estimate, as a percentage.
    /// </summary>
    /// <remarks>
    /// Quarters without both values, and those estimated at zero or below, are left out of
    /// the average rather than counted as no surprise. Returns 0 when that leaves nothing,
    /// which is indistinguishable from a company that met every estimate exactly.
    /// </remarks>
    public double GetAverageEarningsSurprise()
    {
        if (HistoricalEarnings == null || HistoricalEarnings.Count == 0)
        {
            return 0;
        }

        var validEarnings = HistoricalEarnings
            .Where(e => e.Actual?.Raw.HasValue == true && e.Estimate?.Raw.HasValue == true && e.Estimate.Raw > 0)
            .ToList();

        if (validEarnings.Count == 0)
        {
            return 0;
        }

        var surprises = validEarnings.Select(e =>
            ((e.Actual!.Raw!.Value - e.Estimate!.Raw!.Value) / e.Estimate.Raw.Value) * 100
        );

        return surprises.Average();
    }

    /// <summary>
    /// The share of reported quarters that came in above estimate, from 0 to 100.
    /// </summary>
    /// <remarks>
    /// Meeting an estimate exactly does not count as a beat. Returns 0 when no quarter
    /// carries both values, which is indistinguishable from a company that never beat.
    /// </remarks>
    public double GetEarningsBeatRate()
    {
        if (HistoricalEarnings == null || HistoricalEarnings.Count == 0)
        {
            return 0;
        }

        var validEarnings = HistoricalEarnings
            .Where(e => e.Actual?.Raw.HasValue == true && e.Estimate?.Raw.HasValue == true)
            .ToList();

        if (validEarnings.Count == 0)
        {
            return 0;
        }

        var beats = validEarnings.Count(e => e.Actual!.Raw > e.Estimate!.Raw);
        return (beats / (double) validEarnings.Count) * 100;
    }
}
