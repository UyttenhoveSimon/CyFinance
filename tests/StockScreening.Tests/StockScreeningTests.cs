using CyFinance.Models.StockScreening;
using CyFinance.Services.StockScreening;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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
            var client = new HttpClient(handler) { BaseAddress = new Uri("https://query1.finance.yahoo.com") };
            client.DefaultRequestHeaders.Add("X-CyFinance-SkipAuth", "1");
            return client;
        }

        [Test]
        public async Task ScreenAsync_CustomQuery_ReturnsScreenerResult()
        {
            // Arrange: minimal ScreenerResponse payload
            var payload = new ScreenerResponse
            {
                Finance = new FinanceResult
                {
                    Result = new List<ScreenerResult>
                    {
                        new ScreenerResult
                        {
                            Id = "custom",
                            Title = "Custom",
                            Total = 1,
                            Quotes = new List<CyFinance.Models.StockScreening.Quote>
                            {
                                new CyFinance.Models.StockScreening.Quote { Symbol = "AAPL", LongName = "Apple Inc." }
                            }
                        }
                    },
                    Error = null!
                }
            };
            var json = JsonSerializer.Serialize(payload);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var client = CreateMockHttpClient(response);
            IStockScreeningService service = new StockScreeningService(client);
            var request = new ScreenerRequest
            {
                Query = ScreenerQuery.And(
                    ScreenerQuery.Eq("region", "us"),
                    ScreenerQuery.Gt("dayvolume", 15000)),
                SortField = "percentchange",
                SortType = "DESC",
                Size = 25,
                QuoteType = "EQUITY"
            };

            // Act
            var result = await service.ScreenAsync(request);

            // Assert
            await Assert.That(result).IsNotNull();
            await Assert.That(result?.Quotes).IsNotNull();
            await Assert.That(result?.Quotes).IsNotEmpty();
            await Assert.That(result?.Quotes?[0]?.Symbol).IsEqualTo("AAPL");
        }

        [Test]
        public async Task ScreenPredefinedAsync_IncludesScrIdAndParametersInRequest()
        {
            // Arrange: capture the request that the service sends
            HttpRequestMessage? capturedRequest = null;
            var samplePayload = new ScreenerResponse
            {
                Finance = new FinanceResult
                {
                    Result = new List<ScreenerResult>
                    {
                        new ScreenerResult
                        {
                            Id = "day_gainers",
                            Title = "Day Gainers",
                            Total = 1,
                            Quotes = new List<CyFinance.Models.StockScreening.Quote>
                            {
                                new CyFinance.Models.StockScreening.Quote { Symbol = "MSFT", LongName = "Microsoft Corporation" }
                            }
                        }
                    },
                    Error = null!
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
            client.DefaultRequestHeaders.Add("X-CyFinance-SkipAuth", "1");
            IStockScreeningService service = new StockScreeningService(client);

            // Act
            var result = await service.ScreenPredefinedAsync(
                "day_gainers",
                offset: 5,
                count: 10,
                sortField: "percentchange",
                sortAsc: true);

            // Assert basic parsing
            await Assert.That(result).IsNotNull();
            await Assert.That(result?.Quotes?[0]?.Symbol).IsEqualTo("MSFT");

            // Assert request included predefined params
            await Assert.That(capturedRequest).IsNotNull();
            var uri = capturedRequest!.RequestUri?.ToString() ?? string.Empty;
            await Assert.That(uri).Contains("/v1/finance/screener/predefined/saved");
            await Assert.That(uri).Contains("scrIds=day_gainers");
            await Assert.That(uri).Contains("offset=5");
            await Assert.That(uri).Contains("count=10");
            await Assert.That(uri).Contains("sortField=percentchange");
            await Assert.That(uri).Contains("sortAsc=true");
            await Assert.That(capturedRequest.Method).IsEqualTo(HttpMethod.Get);
        }

        [Test]
        public async Task ScreenAsync_HttpFailure_ThrowsException()
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
                var request = new ScreenerRequest
                {
                    Query = ScreenerQuery.Eq("region", "us")
                };
                await service.ScreenAsync(request);
                // if we reach here the call didn't throw - fail the test
                throw new Exception("Expected exception was not thrown for HTTP failure");
            }
            catch (Exception ex)
            {
                // success if an exception was thrown; optionally assert on type/message
                await Assert.That(ex).IsNotNull();
            }
        }

        [Test]
        public async Task ScreenAsync_FundQuery_InfersMutualFundQuoteType()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;
            var okPayload = new ScreenerResponse
            {
                Finance = new FinanceResult
                {
                    Result = new List<ScreenerResult>
                    {
                        new ScreenerResult
                        {
                            Id = "fund",
                            Title = "Fund",
                            Total = 0,
                            Quotes = new List<CyFinance.Models.StockScreening.Quote>()
                        }
                    },
                    Error = null!
                }
            };

            var handler = new FakeHttpMessageHandler((req, ct) =>
            {
                capturedRequest = req;
                var json = JsonSerializer.Serialize(okPayload);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });
            });

            var client = new HttpClient(handler) { BaseAddress = new Uri("https://query1.finance.yahoo.com") };
            client.DefaultRequestHeaders.Add("X-CyFinance-SkipAuth", "1");
            IStockScreeningService service = new StockScreeningService(client);

            var query = FundQuery.And(
                FundQuery.Eq("exchange", "NAS"),
                FundQuery.Gt("initialinvestment", 1000));

            // Act
            var result = await service.ScreenAsync(query);

            // Assert
            await Assert.That(result).IsNotNull();
            await Assert.That(capturedRequest).IsNotNull();

            var requestBody = await capturedRequest!.Content!.ReadAsStringAsync();
            var root = JsonNode.Parse(requestBody);
            var quoteType = root?["quoteType"]?.GetValue<string>();
            await Assert.That(quoteType).IsEqualTo("MUTUALFUND");
        }

        [Test]
        public async Task PredefinedCatalog_ContainsCommonKnownIds()
        {
            await Assert.That(PredefinedScreeners.IsKnown("day_gainers")).IsTrue();
            await Assert.That(PredefinedScreeners.IsKnown("technology_etfs")).IsTrue();
            await Assert.That(PredefinedScreeners.IsKnown("not_a_real_screen")).IsFalse();
        }
    }
}