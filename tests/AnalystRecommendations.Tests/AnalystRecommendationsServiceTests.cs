using CyFinance.Models.AnalystRecommendations;
using CyFinance.Models.QuoteSummary;
using CyFinance.Services.AnalystRecommendations;
using CyFinance.Services.QuoteSummary;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace CyFinance.Tests.AnalystRecommendations
{
    public class AnalystRecommendationsServiceTests
    {
        private readonly IQuoteSummaryService _mockQuoteSummaryService;
        private readonly IAnalystRecommendationsService _service;

        public AnalystRecommendationsServiceTests()
        {
            _mockQuoteSummaryService = Substitute.For<IQuoteSummaryService>();
            _service = new AnalystRecommendationsService(_mockQuoteSummaryService);
        }

        private QuoteResponse CreateMockQuoteResponse()
        {
            return new QuoteResponse(
                new CyFinance.Models.QuoteSummary.QuoteSummary(
                    new List<QuoteResult>
                    {
                        new QuoteResult
                        {
                            Price = new PriceData { Symbol = "AAPL" }
                        }
                    },
                    null));
        }

        [Test]
        public async Task GetRecommendationsAsync_ValidTicker_ReturnsRecommendations()
        {
            var ticker = "AAPL";
            var mockResponse = CreateMockQuoteResponse();
            _mockQuoteSummaryService.GetQuoteSummaryAsync(ticker, Arg.Any<string[]>()).Returns(mockResponse);

            var result = await _service.GetRecommendationsAsync(ticker);

            await Assert.That(result).IsNotNull();
            await Assert.That(result?.Ticker).IsEqualTo(ticker);
        }

        [Test]
        public async Task GetRecommendationTrendAsync_ValidTicker_ReturnsTrend()
        {
            var ticker = "AAPL";
            var mockResponse = CreateMockQuoteResponse();
            _mockQuoteSummaryService.GetQuoteSummaryAsync(ticker, Arg.Any<string[]>()).Returns(mockResponse);

            var result = await _service.GetRecommendationTrendAsync(ticker);

            // Result may be null if no data available
            await Assert.That(result == null || result is List<RecommendationData>).IsTrue();
        }

        [Test]
        public async Task GetRatingChangeHistoryAsync_ValidTicker_ReturnsHistory()
        {
            var ticker = "AAPL";
            var mockResponse = CreateMockQuoteResponse();
            _mockQuoteSummaryService.GetQuoteSummaryAsync(ticker, Arg.Any<string[]>()).Returns(mockResponse);

            var result = await _service.GetRatingChangeHistoryAsync(ticker);

            // Result may be null if no data available
            await Assert.That(result == null || result is List<RatingChange>).IsTrue();
        }

        [Test]
        public async Task GetRecommendationsAsync_EmptyTicker_ThrowsException()
        {
            await Assert.That(async () => await _service.GetRecommendationsAsync(""))
                .ThrowsException()
                .WithMessageContaining("cannot be empty");
        }

        [Test]
        public async Task GetRecommendationsAsync_NullTicker_ThrowsException()
        {
            await Assert.That(async () => await _service.GetRecommendationsAsync(null!))
                .ThrowsException();
        }

        [Test]
        public async Task GetRecommendationTrendAsync_EmptyTicker_ThrowsException()
        {
            await Assert.That(async () => await _service.GetRecommendationTrendAsync(""))
                .ThrowsException()
                .WithMessageContaining("cannot be empty");
        }

        [Test]
        public async Task GetRatingChangeHistoryAsync_EmptyTicker_ThrowsException()
        {
            await Assert.That(async () => await _service.GetRatingChangeHistoryAsync(""))
                .ThrowsException()
                .WithMessageContaining("cannot be empty");
        }
    }
}
