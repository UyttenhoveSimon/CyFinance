using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using CyFinance.Models.StockScreening;
using CyFinance.Services.GlobalCalendars;
using CyFinance.Services.StockScreening;
using NSubstitute;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace CyFinance.Tests.GlobalCalendars;

public class GlobalCalendarsServiceTests
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

    private static HttpClient CreateMockHttpClient(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder)
    {
      var client = new HttpClient(new FakeHttpMessageHandler(responder));
      client.DefaultRequestHeaders.Add("X-CyFinance-SkipAuth", "1");
      return client;
    }

    [Test]
    public async Task GetEarningsCalendarAsync_MapsRowsAndAddsMostActiveFilter()
    {
        HttpRequestMessage? capturedRequest = null;
      var client = CreateMockHttpClient((request, _) =>
        {
            capturedRequest = request;
            const string payload = """
            {
              "finance": {
                "result": [
                  {
                    "documents": [
                      {
                        "rows": [
                          ["AAPL", "Apple Inc", 3000000000000, "Apple Earnings", "2026-05-12T20:00:00Z", "AMC", 1.5, 1.6, 6.7]
                        ]
                      }
                    ]
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
          });

        var stockScreening = Substitute.For<IStockScreeningService>();
        stockScreening.ScreenPredefinedAsync(PredefinedScreenersCatalogItem.MostActives, count: 200)
            .Returns(new ScreenerResult
            {
                Id = "most_actives",
                Title = "Most Actives",
                Total = 1,
                Quotes =
                [
                  new CyFinance.Models.StockScreening.Quote
                    {
                        Symbol = "AAPL",
                        MarketCap = 3_000_000_000_000
                    }
                ]
            });

        var service = new GlobalCalendarsService(client, stockScreening);

        var result = await service.GetEarningsCalendarAsync(
            start: new DateTime(2026, 5, 10),
            end: new DateTime(2026, 5, 20),
            limit: 5,
            marketCap: 1_000_000_000,
            filterMostActive: true);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Count).IsEqualTo(1);
        await Assert.That(result[0].Symbol).IsEqualTo("AAPL");
        await Assert.That(result[0].Timing).IsEqualTo("AMC");
        await Assert.That(result[0].ReportedEps).IsEqualTo(1.6);

        var body = JsonNode.Parse(await capturedRequest!.Content!.ReadAsStringAsync());
        await Assert.That(body?["entityIdType"]?.GetValue<string>()).IsEqualTo("sp_earnings");
        await Assert.That(body?["query"]?["operands"]?.ToJsonString()).Contains("AAPL");
        await Assert.That(body?["query"]?["operands"]?.ToJsonString()).Contains("intradaymarketcap");
    }

    [Test]
    public async Task GetIpoCalendarAsync_MapsRows()
    {
      var client = CreateMockHttpClient((_, _) =>
        {
            const string payload = """
            {
              "finance": {
                "result": [
                  {
                    "documents": [
                      {
                        "rows": [
                          ["CYFI", "CyFinance Holdings", "NAS", "2026-05-01", "2026-05-15", "2026-05-08", 10.0, 12.0, 11.0, "USD", 5000000, "IPO"]
                        ]
                      }
                    ]
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
          });

        var stockScreening = Substitute.For<IStockScreeningService>();
        var service = new GlobalCalendarsService(client, stockScreening);

        var result = await service.GetIpoCalendarAsync(limit: 3);

        await Assert.That(result.Count).IsEqualTo(1);
        await Assert.That(result[0].Symbol).IsEqualTo("CYFI");
        await Assert.That(result[0].Exchange).IsEqualTo("NAS");
        await Assert.That(result[0].Price).IsEqualTo(11.0);
    }

    [Test]
    public async Task GetEconomicEventsCalendarAsync_MapsRows()
    {
      var client = CreateMockHttpClient((_, _) =>
        {
            const string payload = """
            {
              "finance": {
                "result": [
                  {
                    "documents": [
                      {
                        "rows": [
                          ["CPI YoY", "US", "2026-05-10T12:30:00Z", "APR", 3.1, 3.0, 3.2, 3.3]
                        ]
                      }
                    ]
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
          });

        var service = new GlobalCalendarsService(client, Substitute.For<IStockScreeningService>());

        var result = await service.GetEconomicEventsCalendarAsync(limit: 3);

        await Assert.That(result.Count).IsEqualTo(1);
        await Assert.That(result[0].Event).IsEqualTo("CPI YoY");
        await Assert.That(result[0].Region).IsEqualTo("US");
        await Assert.That(result[0].Expected).IsEqualTo(3.0);
    }

    [Test]
    public async Task GetSplitsCalendarAsync_MapsRows()
    {
      var client = CreateMockHttpClient((_, _) =>
        {
            const string payload = """
            {
              "finance": {
                "result": [
                  {
                    "documents": [
                      {
                        "rows": [
                          ["NVDA", "NVIDIA Corp", "2026-06-01", true, 1.0, 10.0]
                        ]
                      }
                    ]
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
          });

        var service = new GlobalCalendarsService(client, Substitute.For<IStockScreeningService>());

        var result = await service.GetSplitsCalendarAsync(limit: 3);

        await Assert.That(result.Count).IsEqualTo(1);
        await Assert.That(result[0].Symbol).IsEqualTo("NVDA");
        await Assert.That(result[0].Optionable).IsTrue();
        await Assert.That(result[0].ShareWorth).IsEqualTo(10.0);
    }

    [Test]
    public async Task GetSplitsCalendarAsync_InvalidLimit_ThrowsException()
    {
        var service = new GlobalCalendarsService(
        CreateMockHttpClient((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))),
            Substitute.For<IStockScreeningService>());

        await Assert.That(async () => await service.GetSplitsCalendarAsync(limit: 0))
            .ThrowsException()
            .WithMessageContaining("range [1, 100]");
    }
}