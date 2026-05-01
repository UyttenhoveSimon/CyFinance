using CyFinance.Models.AnalystRecommendations;
using CyFinance.Services.QuoteSummary;

namespace CyFinance.Services.AnalystRecommendations
{
    /// <summary>
    /// Analyst Recommendations Service for Yahoo Finance
    /// Retrieves analyst ratings and rating change history
    /// </summary>
    public class AnalystRecommendationsService : IAnalystRecommendationsService
    {
        private readonly IQuoteSummaryService _quoteSummaryService;

        public AnalystRecommendationsService(IQuoteSummaryService quoteSummaryService)
        {
            _quoteSummaryService = quoteSummaryService ?? throw new ArgumentNullException(nameof(quoteSummaryService));
        }

        /// <summary>
        /// Get analyst recommendations for a ticker
        /// </summary>
        public async Task<AnalystRecommendationsSummary?> GetRecommendationsAsync(string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker))
                throw new ArgumentException("Ticker cannot be empty", nameof(ticker));

            try
            {
                var response = await _quoteSummaryService.GetQuoteSummaryAsync(ticker,
                    "recommendationTrend", "upgradeDowngradeHistory");

                if (response?.QuoteSummary?.Result == null || response.QuoteSummary.Result.Count == 0)
                    return null;

                var result = response.QuoteSummary.Result[0];

                // Extract recommendation data from the QuoteResult
                var summary = new AnalystRecommendationsSummary
                {
                    Ticker = ticker
                };

                // The QuoteResult doesn't directly have recommendation fields,
                // so we need to check what's available and parse accordingly
                // For now, return the basic structure
                return summary;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get analyst recommendations for {ticker}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get recommendation trend data only
        /// </summary>
        public async Task<List<RecommendationData>?> GetRecommendationTrendAsync(string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker))
                throw new ArgumentException("Ticker cannot be empty", nameof(ticker));

            try
            {
                var recommendations = await GetRecommendationsAsync(ticker);
                return recommendations?.RecommendationTrend;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get recommendation trend for {ticker}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get rating change history
        /// </summary>
        public async Task<List<RatingChange>?> GetRatingChangeHistoryAsync(string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker))
                throw new ArgumentException("Ticker cannot be empty", nameof(ticker));

            try
            {
                var recommendations = await GetRecommendationsAsync(ticker);
                return recommendations?.RatingChangeHistory;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get rating change history for {ticker}: {ex.Message}", ex);
            }
        }
    }
}
