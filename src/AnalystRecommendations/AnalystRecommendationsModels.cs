using System.Text.Json.Serialization;

namespace CyFinance.Models.AnalystRecommendations;

/// <summary>
/// The two quote summary modules analyst data comes from.
/// </summary>
public class AnalystRecommendationsResponse
{
    [JsonPropertyName("recommendationTrend")]
    public RecommendationTrend? RecommendationTrend { get; set; }

    [JsonPropertyName("upgradeDowngradeHistory")]
    public UpgradeDowngradeHistory? UpgradeDowngradeHistory { get; set; }
}

/// <summary>
/// One entry per period Yahoo reports.
/// </summary>
public class RecommendationTrend
{
    [JsonPropertyName("trend")]
    public List<RecommendationData>? Trend { get; set; }
}

/// <summary>
/// How many analysts sat in each bucket over one period.
/// </summary>
public class RecommendationData
{
    [JsonPropertyName("period")]
    public string? Period { get; set; }

    [JsonPropertyName("strongBuy")]
    public int? StrongBuy { get; set; }

    [JsonPropertyName("buy")]
    public int? Buy { get; set; }

    [JsonPropertyName("hold")]
    public int? Hold { get; set; }

    [JsonPropertyName("sell")]
    public int? Sell { get; set; }

    [JsonPropertyName("strongSell")]
    public int? StrongSell { get; set; }
}

/// <summary>
/// The upgrades and downgrades Yahoo has on file.
/// </summary>
public class UpgradeDowngradeHistory
{
    [JsonPropertyName("history")]
    public List<RatingChange>? History { get; set; }
}

/// <summary>
/// One firm changing its rating, with the grades it moved between.
/// </summary>
public class RatingChange
{
    [JsonPropertyName("epochGradeDate")]
    public long? EpochGradeDate { get; set; }

    [JsonPropertyName("firm")]
    public string? Firm { get; set; }

    [JsonPropertyName("toGrade")]
    public string? ToGrade { get; set; }

    [JsonPropertyName("fromGrade")]
    public string? FromGrade { get; set; }

    [JsonPropertyName("action")]
    public string? Action { get; set; }
}

/// <summary>
/// What analysts think of a ticker, and how that has changed.
/// </summary>
public class AnalystRecommendationsSummary
{
    public string? Ticker { get; set; }

    public List<RecommendationData>? RecommendationTrend { get; set; }

    public List<RatingChange>? RatingChangeHistory { get; set; }

    /// <summary>
    /// The newest change. Yahoo sends this history newest first.
    /// </summary>
    public RatingChange? LatestRatingChange => RatingChangeHistory?.FirstOrDefault();

    /// <summary>
    /// The current period, which Yahoo labels "0m". Falls back to the first entry if it ever
    /// stops sending that label.
    /// </summary>
    public RecommendationData? LatestRecommendation =>
        RecommendationTrend?.FirstOrDefault(period => period.Period == "0m")
        ?? RecommendationTrend?.FirstOrDefault();

    /// <summary>
    /// Reduces the current period's counts to a single label, from "Strong Buy" to
    /// "Strong Sell". Returns "N/A" when no analyst covers the ticker.
    /// </summary>
    public string GetConsensusRating()
    {
        if (LatestRecommendation == null)
        {
            return "N/A";
        }

        var strongBuy = LatestRecommendation.StrongBuy ?? 0;
        var buy = LatestRecommendation.Buy ?? 0;
        var hold = LatestRecommendation.Hold ?? 0;
        var sell = LatestRecommendation.Sell ?? 0;
        var strongSell = LatestRecommendation.StrongSell ?? 0;

        var total = strongBuy + buy + hold + sell + strongSell;
        if (total == 0)
        {
            return "N/A";
        }

        // Runs from +2 when every analyst says strong buy to -2 when they all say strong sell.
        var score = (strongBuy * 2.0 + buy * 1.0 - sell * 1.0 - strongSell * 2.0) / total;

        return score switch
        {
            > 1.0 => "Strong Buy",
            > 0.5 => "Buy",
            >= -0.5 => "Hold",
            > -1.0 => "Sell",
            _ => "Strong Sell"
        };
    }

    /// <summary>
    /// The share of analysts in each bucket for the current period, as percentages that
    /// sum to 100. Empty when no analyst covers the ticker.
    /// </summary>
    public Dictionary<string, double> GetRecommendationPercentages()
    {
        var result = new Dictionary<string, double>();

        if (LatestRecommendation == null)
        {
            return result;
        }

        var strongBuy = LatestRecommendation.StrongBuy ?? 0;
        var buy = LatestRecommendation.Buy ?? 0;
        var hold = LatestRecommendation.Hold ?? 0;
        var sell = LatestRecommendation.Sell ?? 0;
        var strongSell = LatestRecommendation.StrongSell ?? 0;

        var total = strongBuy + buy + hold + sell + strongSell;

        if (total > 0)
        {
            result["Strong Buy"] = (strongBuy * 100.0) / total;
            result["Buy"] = (buy * 100.0) / total;
            result["Hold"] = (hold * 100.0) / total;
            result["Sell"] = (sell * 100.0) / total;
            result["Strong Sell"] = (strongSell * 100.0) / total;
        }

        return result;
    }
}
