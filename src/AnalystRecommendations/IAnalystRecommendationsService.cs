using CyFinance.Models.AnalystRecommendations;

namespace CyFinance.Services.AnalystRecommendations
{
    /// <summary>
    /// Interface for Yahoo Finance Analyst Recommendations API
    /// Provides access to analyst ratings and rating change history
    /// </summary>
    public interface IAnalystRecommendationsService
    {
        /// <summary>
        /// Get analyst recommendations for a ticker
        /// </summary>
        /// <param name="ticker">The stock ticker symbol</param>
        /// <returns>AnalystRecommendationsSummary with current recommendations and history</returns>
        Task<AnalystRecommendationsSummary?> GetRecommendationsAsync(string ticker);

        /// <summary>
        /// Get recommendation trend data only
        /// </summary>
        /// <param name="ticker">The stock ticker symbol</param>
        /// <returns>List of recommendation data points over time</returns>
        Task<List<RecommendationData>?> GetRecommendationTrendAsync(string ticker);

        /// <summary>
        /// Get rating change history
        /// </summary>
        /// <param name="ticker">The stock ticker symbol</param>
        /// <returns>List of rating changes (upgrades/downgrades)</returns>
        Task<List<RatingChange>?> GetRatingChangeHistoryAsync(string ticker);
    }
}
