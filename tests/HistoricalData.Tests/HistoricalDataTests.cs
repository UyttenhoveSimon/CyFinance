using CyFinance.HistoricalData;
using CyFinance.Models.HistoricalData;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TUnit;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace CyFinance.Tests.HistoricalData
{
    public class HistoricalDataServiceTests
    {
        // Simple fake handler to simulate HttpClient responses
        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _responder;

            public FakeHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder)
            {
                _responder = responder;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => _responder(request, cancellationToken);
        }

        private HttpClient CreateMockHttpClient(HttpResponseMessage response)
        {
            var handler = new FakeHttpMessageHandler((req, ct) => Task.FromResult(response));
            return new HttpClient(handler)
            {
                BaseAddress = new Uri("https://query2.finance.yahoo.com")
            };
        }

        [Test]
        public async Task GetDividends_ShouldReturnEmptyList_WhenNoDividends()
        {
            // Arrange
            var service = new HistoricalDataService(new HttpClient());
            var chart = new ChartResponse(
                Chart: new Chart(
                    Result: new List<ChartResult>
                    {
                        new ChartResult(
                            Meta: null!,
                            Timestamp: new List<long>(),
                            Indicators: null!,
                            Events: new Events(
                                Dividends: new Dictionary<string, Dividend>(),
                                Splits: new Dictionary<string, Split>()
                            )
                        )
                    },
                    Error: null!
                )
            );

            // Act
            var result = service.GetDividends(chart);

            // Assert
            await Assert.That(result).IsNotNull();
            await Assert.That(result.Count).IsEqualTo(0);
        }

        [Test]
        public async Task GetDividends_ShouldMapDividendsCorrectly()
        {
            // Arrange
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var service = new HistoricalDataService(new HttpClient());
            var chart = new ChartResponse(
                Chart: new Chart(
                    Result: new List<ChartResult>
                    {
                        new ChartResult(
                            Meta: null!,
                            Timestamp: new List<long>(),
                            Indicators: null!,
                            Events: new Events(
                                Dividends: new Dictionary<string, Dividend>
                                {
                                    ["1"] = new Dividend(Amount: 1.23, Date: now)
                                },
                                Splits: new Dictionary<string, Split>()
                            )
                        )
                    },
                    Error: null!
                )
            );

            // Act
            var result = service.GetDividends(chart);

            // Assert
            await Assert.That(result).IsNotNull();
            await Assert.That(result.Count).IsEqualTo(1);
            await Assert.That(result[0].Amount).IsEqualTo(1.23);
        }

        [Test]
        public async Task GetSplits_ShouldReturnSplits()
        {
            // Arrange
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var service = new HistoricalDataService(new HttpClient());
            var chart = new ChartResponse(
                Chart: new Chart(
                    Result: new List<ChartResult>
                    {
                        new ChartResult(
                            Meta: null!,
                            Timestamp: new List<long>(),
                            Indicators: null!,
                            Events: new Events(
                                Dividends: new Dictionary<string, Dividend>(),
                                Splits: new Dictionary<string, Split>
                                {
                                    ["s1"] = new Split(
                                        Date: now,
                                        Numerator: 2,
                                        Denominator: 1,
                                        SplitRatio: "2:1"
                                    )
                                }
                            )
                        )
                    },
                    Error: null!
                )
            );

            // Act
            var result = service.GetSplits(chart);

            // Assert
            await Assert.That(result).IsNotNull();
            await Assert.That(result.Count).IsEqualTo(1);
            await Assert.That(result[0].SplitRatio).IsEqualTo("2:1");
        }

        [Test]
        public async Task GetMetadata_ShouldReturnMeta()
        {
            // Arrange
            var meta = new StockMetaData(
                Currency: "USD",
                Symbol: "AAPL",
                ExchangeName: "NMS",
                InstrumentType: "EQUITY",
                FirstTradeDate: 0,
                RegularMarketTime: 0,
                Gmtoffset: 0,
                Timezone: "EST",
                ExchangeTimezoneName: "America/New_York",
                RegularMarketPrice: 0,
                ChartPreviousClose: 0,
                PreviousClose: 0,
                Scale: 0,
                PriceHint: 0,
                CurrentTradingPeriod: null!,
                TradingPeriods: null!,
                DataGranularity: "1d",
                Range: "1mo",
                ValidRanges: null!
            );
            var response = new ChartResponse(
                Chart: new Chart(
                    Result: new List<ChartResult>
                    {
                        new ChartResult(
                            Meta: meta,
                            Timestamp: new List<long>(),
                            Indicators: null!,
                            Events: null!
                        )
                    },
                    Error: null!
                )
            );

            // Act
            var result = HistoricalDataService.GetMetadata(response);

            // Assert
            await Assert.That(result).IsNotNull();
            await Assert.That(result).IsEqualTo(meta);
            await Assert.That(result.Symbol).IsEqualTo("AAPL");
        }

        [Test]
        public async Task GetHistoricalDataAsync_ShouldReturnParsedResponse()
        {
            // Arrange
            var expected = new ChartResponse(
                Chart: new Chart(
                    Result: new List<ChartResult>
                    {
                        new ChartResult(
                            Meta: null!,
                            Timestamp: new List<long>(),
                            Indicators: null!,
                            Events: null!
                        )
                    },
                    Error: null!)
                );
            var json = JsonSerializer.Serialize(expected);
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };

            var client = CreateMockHttpClient(mockResponse);
            var service = new HistoricalDataService(client);

            // Act
            var result = await service.GetHistoricalDataAsync("AAPL");

            // Assert
            await Assert.That(result).IsNotNull();
            await Assert.That(result.Chart).IsNotNull();
        }

        [Test]
        public async Task GetHistoricalDataAsync_ShouldThrow_OnHttpFailure()
        {
            // Arrange
            var mockResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(string.Empty)
            };

            var client = CreateMockHttpClient(mockResponse);
            var service = new HistoricalDataService(client);

            // Act + Assert
            await Assert.That(async () => await service.GetHistoricalDataAsync("FAIL"))
                .ThrowsException();
        }

        [Test]
        public async Task BuildChartUrl_ShouldIncludeTickerAndParams()
        {
            // Arrange
            var service = new HistoricalDataService(new HttpClient());
            var start = new DateTime(2024, 01, 01);
            var end = new DateTime(2024, 12, 31);

            // Access private method via reflection
            var method = typeof(HistoricalDataService).GetMethod(
                "BuildChartUrl",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );
            await Assert.That(method).IsNotNull();

            // Act
            var url = (string)method!.Invoke(service, new object[] { "AAPL", start, end, ChartInterval.OneDay, true, true })!;

            // Assert
            await Assert.That(url).Contains("AAPL");
            await Assert.That(url).Contains("interval=1d");
            await Assert.That(url).Contains("events=div,split");
            await Assert.That(url).Contains("period1=");
            await Assert.That(url).Contains("period2=");
        }
    }
}