using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NSubstitute;
using TUnit.Core;
using TUnit.Assertions;
using YahooFinanceClient.QuoteSummary;
using TUnit.Assertions.Extensions;

namespace YahooFinanceClient.Specs.QuoteSummary
{
    public class QuoteSummaryTests
    {
        private readonly HttpClient _httpClient;
        private readonly QuoteSummaryService _service;
        public QuoteSummaryTests()
        {
            _httpClient = new HttpClient();
            _service = new QuoteSummaryService(_httpClient);
        }

        [Test] 
        public async Task GetQuoteSummaryAsync_ValidTicker_ReturnsQuoteResponse()
        {
            // Arrange
            var ticker = "AAPL";

            // Act
            var result = await _service.GetQuoteSummaryAsync(ticker);

            // Assert
            await Assert.That(result).IsNotNull();
            await Assert.That(result?.QuoteSummary).IsNotNull();
            await Assert.That(result?.QuoteSummary?.Result).IsNotNull();
            await Assert.That(result?.QuoteSummary?.Result).IsNotEmpty();
            await Assert.That(ticker).IsEqualTo( result?.QuoteSummary?.Result?.FirstOrDefault().Price?.Symbol);
        }
    }
}