using CyFinance.Models.AnalystRecommendations;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace CyFinance.Tests.AnalystRecommendations;

/// <summary>
/// Yahoo sends recommendationTrend newest first, labelling the current period "0m" and the
/// three before it "-1m" to "-3m". Reading from the wrong end silently reports a consensus
/// that is three months old, so these pin the end down.
/// </summary>
public class AnalystRecommendationsSummaryTests
{
    private static AnalystRecommendationsSummary TrendAsYahooSendsIt() => new()
    {
        RecommendationTrend =
        [
            new RecommendationData { Period = "0m", StrongBuy = 10, Buy = 0, Hold = 0, Sell = 0, StrongSell = 0 },
            new RecommendationData { Period = "-1m", StrongBuy = 0, Buy = 0, Hold = 0, Sell = 0, StrongSell = 10 }
        ]
    };

    [Test]
    public async Task LatestRecommendation_TakesTheCurrentPeriod()
    {
        var latest = TrendAsYahooSendsIt().LatestRecommendation;

        await Assert.That(latest?.Period).IsEqualTo("0m");
    }

    [Test]
    public async Task LatestRecommendation_FallsBackToTheFirstEntryWithoutTheLabel()
    {
        var summary = new AnalystRecommendationsSummary
        {
            RecommendationTrend =
            [
                new RecommendationData { Period = null, StrongBuy = 1 },
                new RecommendationData { Period = "-1m", StrongBuy = 2 }
            ]
        };

        await Assert.That(summary.LatestRecommendation?.StrongBuy).IsEqualTo(1);
    }

    [Test]
    public async Task GetConsensusRating_ReadsTheCurrentPeriodNotTheOldest()
    {
        await Assert.That(TrendAsYahooSendsIt().GetConsensusRating()).IsEqualTo("Strong Buy");
    }

    [Test]
    public async Task LatestRatingChange_TakesTheNewestChange()
    {
        var summary = new AnalystRecommendationsSummary
        {
            RatingChangeHistory =
            [
                new RatingChange { Firm = "Newest", EpochGradeDate = 200 },
                new RatingChange { Firm = "Older", EpochGradeDate = 100 }
            ]
        };

        await Assert.That(summary.LatestRatingChange?.Firm).IsEqualTo("Newest");
    }
}
