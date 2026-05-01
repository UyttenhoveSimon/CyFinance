using CyFinance.Models.HistoricalData;
using CyFinance.Services.Crypto;
using System.Net;
using System.Text;
using TUnit;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace CyFinance.Tests.Crypto;

public class CryptoServiceTests
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
    public async Task GetCryptoQuoteAsync_ShouldReturnMappedQuote()
    {
        var sampleJson = @"
        {
          ""chart"": {
            ""result"": [
              {
                ""meta"": {
                  ""currency"": ""USD"",
                  ""symbol"": ""BTC-USD"",
                  ""exchangeName"": ""CCC"",
                  ""instrumentType"": ""CRYPTOCURRENCY"",
                  ""firstTradeDate"": 0,
                  ""regularMarketTime"": 1714905600,
                  ""gmtoffset"": 0,
                  ""timezone"": ""UTC"",
                  ""exchangeTimezoneName"": ""UTC"",
                  ""regularMarketPrice"": 62000,
                  ""chartPreviousClose"": 60000,
                  ""previousClose"": 60000,
                  ""scale"": 2,
                  ""priceHint"": 2,
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
                      ""open"": [61000, 61800],
                      ""low"": [60800, 61700],
                      ""high"": [61900, 62100],
                      ""close"": [61800, 62000],
                      ""volume"": [1200, 1300]
                    }
                  ],
                  ""adjclose"": [{ ""adjclose"": [61800, 62000] }]
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

        var service = new CryptoService(CreateMockHttpClient(sampleJson));

        var result = await service.GetCryptoQuoteAsync("btc");

        await Assert.That(result).IsNotNull();
        await Assert.That(result?.Symbol).IsEqualTo("BTC-USD");
        await Assert.That(result?.Price).IsEqualTo(62000);
        await Assert.That(result?.Change).IsEqualTo(2000);
    }

    [Test]
    public async Task GetHistoricalPricesAsync_ShouldMapSeries()
    {
        var sampleJson = @"
        {
          ""chart"": {
            ""result"": [
              {
                ""meta"": {
                  ""currency"": ""USD"",
                  ""symbol"": ""BTC-USD"",
                  ""exchangeName"": ""CCC"",
                  ""instrumentType"": ""CRYPTOCURRENCY"",
                  ""firstTradeDate"": 0,
                  ""regularMarketTime"": 1714905600,
                  ""gmtoffset"": 0,
                  ""timezone"": ""UTC"",
                  ""exchangeTimezoneName"": ""UTC"",
                  ""regularMarketPrice"": 62000,
                  ""chartPreviousClose"": 60000,
                  ""previousClose"": 60000,
                  ""scale"": 2,
                  ""priceHint"": 2,
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
                      ""open"": [60000, 61000],
                      ""low"": [59000, 60000],
                      ""high"": [61500, 62500],
                      ""close"": [61000, 62000],
                      ""volume"": [4200, 5300]
                    }
                  ],
                  ""adjclose"": [{ ""adjclose"": [61000, 62000] }]
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

        var service = new CryptoService(CreateMockHttpClient(sampleJson));

        var result = await service.GetHistoricalPricesAsync("BTC", interval: ChartInterval.OneDay);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Count).IsEqualTo(2);
        await Assert.That(result[0].Close).IsEqualTo(61000);
        await Assert.That(result[1].Close).IsEqualTo(62000);
    }

    [Test]
    public async Task GetCryptoQuoteAsync_InvalidTicker_ShouldThrow()
    {
        var service = new CryptoService(CreateMockHttpClient("{}"));

        await Assert.That(async () => await service.GetCryptoQuoteAsync("BTC$"))
            .ThrowsException()
            .WithMessageContaining("letters, digits, and hyphens");
    }
}