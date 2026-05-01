using System.Text.Json.Serialization;

namespace CyFinance.Models.AnalystRecommendations
{
    /// <summary>
    /// Container for analyst recommendation data
    /// </summary>
    public class AnalystRecommendationsResponse
    {
        [JsonPropertyName("recommendationTrend")]
        public RecommendationTrend? RecommendationTrend { get; set; }

        [JsonPropertyName("upgradeDowngradeHistory")]
        public UpgradeDowngradeHistory? UpgradeDowngradeHistory { get; set; }
    }

    /// <summary>
    /// Trend of analyst recommendations over time
    /// </summary>
    public class RecommendationTrend
    {
        [JsonPropertyName("trend")]
        public List<RecommendationData>? Trend { get; set; }
    }

    /// <summary>
    /// Individual recommendation data point
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
    /// History of rating changes (upgrades/downgrades)
    /// </summary>
    public class UpgradeDowngradeHistory
    {
        [JsonPropertyName("history")]
        public List<RatingChange>? History { get; set; }
    }

    /// <summary>
    /// Individual rating change event
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
    /// Summary of analyst recommendations for a ticker
    /// </summary>
    public class AnalystRecommendationsSummary
    {
        public string? Ticker { get; set; }

        /// <summary>
        /// Current recommendation trend data
        /// </summary>
        public List<RecommendationData>? RecommendationTrend { get; set; }

        /// <summary>
        /// History of rating changes
        /// </summary>
        public List<RatingChange>? RatingChangeHistory { get; set; }

        /// <summary>
        /// Most recent rating change (if available)
        /// </summary>
        public RatingChange? LatestRatingChange => RatingChangeHistory?.FirstOrDefault();

        /// <summary>
        /// Consensus recommendation count
        /// </summary>
        public RecommendationData? LatestRecommendation => RecommendationTrend?.LastOrDefault();

        /// <summary>
        /// Calculate consensus rating
        /// </summary>
        public string GetConsensusRating()
        {
            if (LatestRecommendation == null)
                return "N/A";

            var strongBuy = LatestRecommendation.StrongBuy ?? 0;
            var buy = LatestRecommendation.Buy ?? 0;
            var hold = LatestRecommendation.Hold ?? 0;
            var sell = LatestRecommendation.Sell ?? 0;
            var strongSell = LatestRecommendation.StrongSell ?? 0;

            var total = strongBuy + buy + hold + sell + strongSell;
            if (total == 0)
                return "N/A";

            // Calculate weighted consensus
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
        /// Get recommendation percentages
        /// </summary>
        public Dictionary<string, double> GetRecommendationPercentages()
        {
            var result = new Dictionary<string, double>();

            if (LatestRecommendation == null)
                return result;

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
}
