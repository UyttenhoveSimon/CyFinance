using CyFinance.Models.EarningsCalendar;
using CyFinance.Models.QuoteSummary;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace CyFinance.Tests.EarningsCalendar;

/// <summary>
/// Yahoo sends earningsChart.quarterly oldest first, the opposite of the other modules.
/// Reading from the wrong end returns a quarter that is a year stale.
/// </summary>
public class EarningsCalendarSummaryTests
{
    private static EarningsCalendarSummary QuartersAsYahooSendsThem() => new()
    {
        HistoricalEarnings =
        [
            new QuarterlyEarnings("3Q2025", new YahooValue(1.0, "1.00"), new YahooValue(2.0, "2.00")),
            new QuarterlyEarnings("2Q2026", new YahooValue(4.0, "4.00"), new YahooValue(2.0, "2.00"))
        ]
    };

    [Test]
    public async Task GetMostRecentEarnings_TakesTheNewestQuarter()
    {
        await Assert.That(QuartersAsYahooSendsThem().GetMostRecentEarnings()?.Date).IsEqualTo("2Q2026");
    }

    [Test]
    public async Task GetMostRecentEarningsDate_TakesTheNewestQuarter()
    {
        await Assert.That(QuartersAsYahooSendsThem().GetMostRecentEarningsDate()).IsEqualTo("2Q2026");
    }

    [Test]
    public async Task GetEarningsBeatRate_CountsOnlyQuartersAboveEstimate()
    {
        await Assert.That(QuartersAsYahooSendsThem().GetEarningsBeatRate()).IsEqualTo(50d);
    }

    [Test]
    public async Task GetAverageEarningsSurprise_AveragesThePercentageGap()
    {
        await Assert.That(QuartersAsYahooSendsThem().GetAverageEarningsSurprise()).IsEqualTo(25d);
    }
}
