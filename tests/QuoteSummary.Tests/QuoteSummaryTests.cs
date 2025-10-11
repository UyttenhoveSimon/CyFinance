using CyFinance.Models.QuoteSummary;
using CyFinance.Services.QuoteSummary;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace CyFinance.Tests.QuoteSummary
{
    public class QuoteSummaryTests
    {
        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _responder;
            public FakeHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder) => _responder = responder;
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => _responder(request, cancellationToken);
        }

        private HttpClient CreateMockHttpClient(HttpResponseMessage response)
        {
            var handler = new FakeHttpMessageHandler((req, ct) => Task.FromResult(response));
            return new HttpClient(handler) { BaseAddress = new Uri("https://query1.finance.yahoo.com") };
        }

        private readonly IQuoteSummaryService _service;

        public QuoteSummaryTests()
        {
            // prepare a minimal QuoteResponse payload to avoid network
            var mockPayload = new QuoteResponse
            {
                QuoteSummary = new Models.QuoteSummary.QuoteSummary
                {
                    Result = new List<Models.QuoteSummary.QuoteSummaryResult>
                    {
                        new Models.QuoteSummary.QuoteSummaryResult
                        {
                            Price = new Models.QuoteSummary.Price { Symbol = "AAPL" }
                        }
                    }
                }
            };
            var json = JsonSerializer.Serialize(mockPayload);
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
            var client = CreateMockHttpClient(mockResponse);

            // typed as interface but concrete implementation created for these tests
            _service = new QuoteSummaryService(client);
        }

        [Test]
        public async Task GetQuoteSummaryAsync_ValidTicker_ReturnsQuoteResponse()
        {
            var ticker = "AAPL";
            var result = await _service.GetQuoteSummaryAsync(ticker);

            await Assert.That(result).IsNotNull();
            await Assert.That(result?.QuoteSummary?.Result).IsNotEmpty();
            await Assert.That(ticker).IsEqualTo(result?.QuoteSummary?.Result?.FirstOrDefault()?.Price?.Symbol);
        }
    }
}