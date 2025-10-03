using CyFinance.HistoricalData;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace CyFinance.Tests.HistoricalData
{
    public class HistoricalDataTests
    {
        private readonly HttpClient _httpClient;
        private readonly HistoricalDataService _service;
        public HistoricalDataTests()
        {
            _httpClient = new HttpClient();
            _service = new HistoricalDataService(_httpClient);
        }

        [Test] 
        public async Task GetQuoteSummaryAsync_ValidTicker_ReturnsQuoteResponse()
        {
            // Arrange
            var ticker = "AAPL";

            // Act
            //var result = await _service. (ticker);

            // Assert
            await Assert.That(result).IsNotNull();
            await Assert.That(result?.QuoteSummary).IsNotNull();
            await Assert.That(result?.QuoteSummary?.Result).IsNotNull();
            await Assert.That(result?.QuoteSummary?.Result).IsNotEmpty();
            await Assert.That(ticker).IsEqualTo( result?.QuoteSummary?.Result?.FirstOrDefault().Price?.Symbol);
        }
    }
}