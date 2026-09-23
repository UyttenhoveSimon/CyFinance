using CyFinance.Models.AnalystRecommendations;

namespace CyFinance.Services.AnalystRecommendations;

/// <summary>
/// Provides analyst ratings and rating-change history.
/// </summary>
public interface IAnalystRecommendationsService
{
    /// <summary>
    /// Gets the current recommendations together with their history.
    /// </summary>
    Task<AnalystRecommendationsSummary?> GetRecommendationsAsync(string ticker);

    /// <summary>
    /// Gets the analyst counts, from strong buy through strong sell, for each period Yahoo reports.
    /// </summary>
    Task<List<RecommendationData>?> GetRecommendationTrendAsync(string ticker);

    /// <summary>
    /// Gets the upgrades and downgrades published for the ticker.
    /// </summary>
    Task<List<RatingChange>?> GetRatingChangeHistoryAsync(string ticker);
}
