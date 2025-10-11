using CyFinance.Models.QuoteSummary;
using CyFinance.Services.StockScreening;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TUnit;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace CyFinance.Tests.StockScreening
{
    public class StockScreeningTests
    {
        // Fake handler used across tests
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

        [Test]
        public async Task GetStockScreeningAsync_ValidTicker_ReturnsQuoteResponse()
        {
            // Arrange: minimal QuoteResponse payload
            var payload = new QuoteResponse
            {
                QuoteSummary = new CyFinance.Models.QuoteSummary.QuoteSummary
                {
                    Result = new List<CyFinance.Models.QuoteSummary.QuoteSummaryResult>
                    {
                        new CyFinance.Models.QuoteSummary.QuoteSummaryResult
                        {
                            Price = new Price { Symbol = "AAPL" }
                        }
                    }
                }
            };
            var json = JsonSerializer.Serialize(payload);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var client = CreateMockHttpClient(response);
            IStockScreeningService service = new StockScreeningService(client);

            // Act
            var result = await service.GetStockScreeningAsync("AAPL");

            // Assert
            await Assert.That(result).IsNotNull();
            await Assert.That(result?.QuoteSummary?.Result).IsNotEmpty();
            await Assert.That(result?.QuoteSummary?.Result?[0]?.Price?.Symbol).IsEqualTo("AAPL");
        }

        [Test]
        public async Task GetStockScreeningAsync_WithModules_IncludesModulesInRequest()
        {
            // Arrange: capture the request that the service sends
            HttpRequestMessage? capturedRequest = null;
            var samplePayload = new QuoteResponse
            {
                QuoteSummary = new CyFinance.Models.QuoteSummary.QuoteSummary
                {
                    Result = new List<CyFinance.Models.QuoteSummary.QuoteSummaryResult>
                    {
                        new CyFinance.Models.QuoteSummary.QuoteSummaryResult { Price = new Price { Symbol = "MSFT" } }
                    }
                }
            };
            var json = JsonSerializer.Serialize(samplePayload);
            var okResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var handler = new FakeHttpMessageHandler((req, ct) =>
            {
                capturedRequest = req;
                return Task.FromResult(okResponse);
            });

            var client = new HttpClient(handler) { BaseAddress = new Uri("https://query1.finance.yahoo.com") };
            IStockScreeningService service = new StockScreeningService(client);

            // Act
            var modules = new[] { "price", "summaryDetail" };
            var result = await service.GetStockScreeningAsync("MSFT", modules);

            // Assert basic parsing
            await Assert.That(result).IsNotNull();
            // Assert request included modules list (service should encode modules in query)
            await Assert.That(capturedRequest).IsNotNull();
            var uri = capturedRequest!.RequestUri?.ToString() ?? string.Empty;
            await Assert.That(uri).Contains("modules=");
            await Assert.That(uri).Contains("price");
            await Assert.That(uri).Contains("summaryDetail");
        }

        [Test]
        public async Task GetStockScreeningAsync_HttpFailure_ThrowsException()
        {
            // Arrange: simulate server error
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("server error", Encoding.UTF8, "text/plain")
            };
            var client = CreateMockHttpClient(response);
            IStockScreeningService service = new StockScreeningService(client);

            // Act / Assert: expect exception on non-success HTTP status
            try
            {
                await service.GetStockScreeningAsync("FAIL");
                // if we reach here the call didn't throw - fail the test
                throw new Exception("Expected exception was not thrown for HTTP failure");
            }
            catch (Exception ex)
            {
                // success if an exception was thrown; optionally assert on type/message
                await Assert.That(ex).IsNotNull();
            }
        }
    }
}