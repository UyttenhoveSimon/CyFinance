using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CyFinance.Services.MarketSummary;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace CyFinance.Tests.MarketSummary;

public class MarketSummaryServiceTests
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

    [Test]
    public async Task GetMarketSummaryAsync_ReturnsDictionaryByExchange()
    {
        HttpRequestMessage? capturedRequest = null;
        var client = new HttpClient(new FakeHttpMessageHandler((request, _) =>
        {
            capturedRequest = request;
            const string payload = """
            {
              "marketSummaryResponse": {
                "result": [
                  {
                    "exchange": "DJI",
                    "shortName": "Dow Jones Industrial Average",
                    "regularMarketPrice": { "raw": 39000.5, "fmt": "39,000.5" },
                    "regularMarketChange": { "raw": 120.25, "fmt": "120.25" },
                    "regularMarketChangePercent": { "raw": 0.31, "fmt": "0.31%" }
                  }
                ],
                "error": null
              }
            }
            """;

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            });
        }));

        var service = new MarketSummaryService(client);

        var result = await service.GetMarketSummaryAsync("US");

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.ContainsKey("DJI")).IsTrue();
        await Assert.That(result["DJI"].ShortName).IsEqualTo("Dow Jones Industrial Average");
        await Assert.That(result["DJI"].RegularMarketPrice).IsEqualTo(39000.5);
        await Assert.That(capturedRequest).IsNotNull();
        await Assert.That(capturedRequest!.RequestUri!.ToString()).Contains("market=US");
        await Assert.That(capturedRequest.RequestUri!.ToString()).Contains("fields=shortName%2CregularMarketPrice%2CregularMarketChange%2CregularMarketChangePercent");
    }

    [Test]
    public async Task GetMarketStatusAsync_ReturnsParsedStatus()
    {
        var client = new HttpClient(new FakeHttpMessageHandler((request, _) =>
        {
            const string payload = """
            {
              "finance": {
                "marketTimes": [
                  {
                    "marketTime": [
                      {
                        "open": "2026-05-10T09:30:00-04:00",
                        "close": "2026-05-10T16:00:00-04:00",
                        "timezone": [
                          {
                            "name": "America/New_York",
                            "short": "EDT",
                            "gmtoffset": -14400000
                          }
                        ]
                      }
                    ]
                  }
                ]
              }
            }
            """;

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            });
        }));

        var service = new MarketSummaryService(client);

        var result = await service.GetMarketStatusAsync("US");

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.TimezoneName).IsEqualTo("America/New_York");
        await Assert.That(result.TimezoneShortName).IsEqualTo("EDT");
        await Assert.That(result.GmtOffsetMilliseconds).IsEqualTo(-14400000);
        await Assert.That(result.Open).IsNotNull();
        await Assert.That(result.Close).IsNotNull();
    }

    [Test]
    public async Task GetMarketSnapshotAsync_EmptyMarket_ThrowsException()
    {
        var client = new HttpClient(new FakeHttpMessageHandler((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))));
        var service = new MarketSummaryService(client);

        await Assert.That(async () => await service.GetMarketSnapshotAsync(" "))
            .ThrowsException()
            .WithMessageContaining("Market cannot be empty");
    }
}