using CyFinance.Models.HistoricalData;
using CyFinance.Services.Currency;
using System.Net;
using System.Text;
using TUnit;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace CyFinance.Tests.Currency;

public class CurrencyServiceTests
{
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

    private static HttpClient CreateMockHttpClient(string jsonPayload)
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
        };

        var handler = new FakeHttpMessageHandler((req, ct) => Task.FromResult(response));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://query2.finance.yahoo.com") };
        client.DefaultRequestHeaders.Add("X-CyFinance-SkipAuth", "1");
        return client;
    }

    [Test]
    public async Task GetExchangeRateAsync_ShouldReturnMappedQuote()
    {
        var sampleJson = @"
        {
          ""chart"": {
            ""result"": [
              {
                ""meta"": {
                  ""currency"": ""USD"",
                  ""symbol"": ""EURUSD=X"",
                  ""exchangeName"": ""CCY"",
                  ""instrumentType"": ""CURRENCY"",
                  ""firstTradeDate"": 0,
                  ""regularMarketTime"": 1714905600,
                  ""gmtoffset"": 0,
                  ""timezone"": ""UTC"",
                  ""exchangeTimezoneName"": ""UTC"",
                  ""regularMarketPrice"": 1.11,
                  ""chartPreviousClose"": 1.1,
                  ""previousClose"": 1.1,
                  ""scale"": 2,
                  ""priceHint"": 4,
                  ""currentTradingPeriod"": {
                    ""pre"": { ""timezone"": ""UTC"", ""start"": 0, ""end"": 0, ""gmtoffset"": 0 },
                    ""regular"": { ""timezone"": ""UTC"", ""start"": 0, ""end"": 0, ""gmtoffset"": 0 },
                    ""post"": { ""timezone"": ""UTC"", ""start"": 0, ""end"": 0, ""gmtoffset"": 0 }
                  },
                  ""tradingPeriods"": [[{ ""timezone"": ""UTC"", ""start"": 0, ""end"": 0, ""gmtoffset"": 0 }]],
                  ""dataGranularity"": ""1m"",
                  ""range"": ""1d"",
                  ""validRanges"": [""1d""]
                },
                ""timestamp"": [1714905540, 1714905600],
                ""indicators"": {
                  ""quote"": [
                    {
                      ""open"": [1.1, 1.11],
                      ""low"": [1.09, 1.1],
                      ""high"": [1.11, 1.12],
                      ""close"": [1.1, 1.11],
                      ""volume"": [0, 0]
                    }
                  ],
                  ""adjclose"": [{ ""adjclose"": [1.1, 1.11] }]
                },
                ""events"": {
                  ""dividends"": {},
                  ""splits"": {}
                }
              }
            ],
            ""error"": null
          }
        }";

        var service = new CurrencyService(CreateMockHttpClient(sampleJson));

        var result = await service.GetExchangeRateAsync("eur", "usd");

        await Assert.That(result).IsNotNull();
        await Assert.That(result?.Symbol).IsEqualTo("EURUSD=X");
        await Assert.That(result?.Rate).IsEqualTo(1.11);
        await Assert.That(Math.Round(result?.Change ?? 0, 2)).IsEqualTo(0.01);
    }

    [Test]
    public async Task GetHistoricalRatesAsync_ShouldMapSeries()
    {
        var sampleJson = @"
        {
          ""chart"": {
            ""result"": [
              {
                ""meta"": {
                  ""currency"": ""USD"",
                  ""symbol"": ""EURUSD=X"",
                  ""exchangeName"": ""CCY"",
                  ""instrumentType"": ""CURRENCY"",
                  ""firstTradeDate"": 0,
                  ""regularMarketTime"": 1714905600,
                  ""gmtoffset"": 0,
                  ""timezone"": ""UTC"",
                  ""exchangeTimezoneName"": ""UTC"",
                  ""regularMarketPrice"": 1.11,
                  ""chartPreviousClose"": 1.1,
                  ""previousClose"": 1.1,
                  ""scale"": 2,
                  ""priceHint"": 4,
                  ""currentTradingPeriod"": {
                    ""pre"": { ""timezone"": ""UTC"", ""start"": 0, ""end"": 0, ""gmtoffset"": 0 },
                    ""regular"": { ""timezone"": ""UTC"", ""start"": 0, ""end"": 0, ""gmtoffset"": 0 },
                    ""post"": { ""timezone"": ""UTC"", ""start"": 0, ""end"": 0, ""gmtoffset"": 0 }
                  },
                  ""tradingPeriods"": [[{ ""timezone"": ""UTC"", ""start"": 0, ""end"": 0, ""gmtoffset"": 0 }]],
                  ""dataGranularity"": ""1d"",
                  ""range"": ""5d"",
                  ""validRanges"": [""5d""]
                },
                ""timestamp"": [1714819200, 1714905600],
                ""indicators"": {
                  ""quote"": [
                    {
                      ""open"": [1.09, 1.1],
                      ""low"": [1.08, 1.09],
                      ""high"": [1.11, 1.12],
                      ""close"": [1.1, 1.11],
                      ""volume"": [0, 0]
                    }
                  ],
                  ""adjclose"": [{ ""adjclose"": [1.1, 1.11] }]
                },
                ""events"": {
                  ""dividends"": {},
                  ""splits"": {}
                }
              }
            ],
            ""error"": null
          }
        }";

        var service = new CurrencyService(CreateMockHttpClient(sampleJson));

        var result = await service.GetHistoricalRatesAsync("EUR", "USD", interval: ChartInterval.OneDay);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Count).IsEqualTo(2);
        await Assert.That(result[0].Close).IsEqualTo(1.1);
        await Assert.That(result[1].Close).IsEqualTo(1.11);
    }

    [Test]
    public async Task ConvertAsync_ShouldMultiplyByCurrentRate()
    {
        var sampleJson = @"
        {
          ""chart"": {
            ""result"": [
              {
                ""meta"": {
                  ""currency"": ""USD"",
                  ""symbol"": ""EURUSD=X"",
                  ""exchangeName"": ""CCY"",
                  ""instrumentType"": ""CURRENCY"",
                  ""firstTradeDate"": 0,
                  ""regularMarketTime"": 1714905600,
                  ""gmtoffset"": 0,
                  ""timezone"": ""UTC"",
                  ""exchangeTimezoneName"": ""UTC"",
                  ""regularMarketPrice"": 1.2,
                  ""chartPreviousClose"": 1.1,
                  ""previousClose"": 1.1,
                  ""scale"": 2,
                  ""priceHint"": 4,
                  ""currentTradingPeriod"": {
                    ""pre"": { ""timezone"": ""UTC"", ""start"": 0, ""end"": 0, ""gmtoffset"": 0 },
                    ""regular"": { ""timezone"": ""UTC"", ""start"": 0, ""end"": 0, ""gmtoffset"": 0 },
                    ""post"": { ""timezone"": ""UTC"", ""start"": 0, ""end"": 0, ""gmtoffset"": 0 }
                  },
                  ""tradingPeriods"": [[{ ""timezone"": ""UTC"", ""start"": 0, ""end"": 0, ""gmtoffset"": 0 }]],
                  ""dataGranularity"": ""1m"",
                  ""range"": ""1d"",
                  ""validRanges"": [""1d""]
                },
                ""timestamp"": [1714905600],
                ""indicators"": {
                  ""quote"": [
                    {
                      ""open"": [1.2],
                      ""low"": [1.2],
                      ""high"": [1.2],
                      ""close"": [1.2],
                      ""volume"": [0]
                    }
                  ],
                  ""adjclose"": [{ ""adjclose"": [1.2] }]
                },
                ""events"": {
                  ""dividends"": {},
                  ""splits"": {}
                }
              }
            ],
            ""error"": null
          }
        }";

        var service = new CurrencyService(CreateMockHttpClient(sampleJson));

        var converted = await service.ConvertAsync(10, "EUR", "USD");

        await Assert.That(converted).IsEqualTo(12.0);
    }

    [Test]
    public async Task GetExchangeRateAsync_InvalidCurrencyCode_ShouldThrow()
    {
        var service = new CurrencyService(CreateMockHttpClient("{}"));

        await Assert.That(async () => await service.GetExchangeRateAsync("EURO", "USD"))
            .ThrowsException()
            .WithMessageContaining("3-letter ISO code");
    }
}