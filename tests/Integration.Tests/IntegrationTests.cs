using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NSubstitute;
using TUnit.Core;
using TUnit.Assertions;
using CyFinance.QuoteSummary;
using TUnit.Assertions.Extensions;
using System.Net;
using CyFinance.HistoricalData;
using CyFinance.Models.HistoricalData;

namespace CyFinance.Tests.Integration
{
    public class IntegrationTests
    {
        private readonly HttpClient _httpClient;
        private readonly QuoteSummaryService _qsService;
        private readonly HistoricalDataService _hdService;

        public IntegrationTests()
        {
            _httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            });

            _qsService = new QuoteSummaryService(_httpClient);
            _hdService = new HistoricalDataService(_httpClient);
        }

        [Test]
        public async Task GetQuoteSummaryAsync_ValidTicker_ReturnsQuoteResponse()
        {
            // Arrange
            var ticker = "AAPL";

            // Act
            var result = await _qsService.GetQuoteSummaryAsync(ticker);

            // Assert
            await Assert.That(result).IsNotNull();
            await Assert.That(result?.QuoteSummary).IsNotNull();
            await Assert.That(result?.QuoteSummary?.Result).IsNotNull();
            await Assert.That(result?.QuoteSummary?.Result).IsNotEmpty();
            await Assert.That(ticker).IsEqualTo(result?.QuoteSummary?.Result?.FirstOrDefault().Price?.Symbol);
        }

        [Test]
        public async Task GetHistoryDataAsync_ValidTicker_ReturnsHistoricalDataResponse()
        {
            // Arrange
            var ticker = "AMZN";

            // Act
            var result = await _hdService.GetHistoricalDataAsync(ticker, DateTime.UtcNow.AddMonths(-1), DateTime.Now, ChartInterval.OneDay);

            // Assert
            await Assert.That(result).IsNotNull();
            await Assert.That(result?.Chart).IsNotNull();
            await Assert.That(result?.Chart?.Result).IsNotNull();
            await Assert.That(result?.Chart?.Result).IsNotEmpty();
            await Assert.That(ticker).IsEqualTo(result?.Chart?.Result?.FirstOrDefault().Meta?.Symbol);
        }
    }
}