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

namespace CyFinance.Specs.QuoteSummary
{
    public class SmokeTests
    {
        private readonly HttpClient _httpClient;
        private readonly QuoteSummaryService _service;
        public SmokeTests()
        {
            _httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            });

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
            await Assert.That(ticker).IsEqualTo(result?.QuoteSummary?.Result?.FirstOrDefault().Price?.Symbol);
        }
    }
}