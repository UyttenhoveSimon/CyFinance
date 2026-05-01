using CyFinance.Models.CompanyNews;
using CyFinance.Services.CompanyNews;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace CyFinance.Tests.CompanyNews;

public class CompanyNewsServiceTests
{
    private class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _responder;
        public FakeHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder) => _responder = responder;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => _responder(request, cancellationToken);
    }

    private static HttpClient CreateMockHttpClient(HttpResponseMessage response)
    {
        var handler = new FakeHttpMessageHandler((req, ct) => Task.FromResult(response));
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://query2.finance.yahoo.com") };
        client.DefaultRequestHeaders.Add("X-CyFinance-SkipAuth", "1");
        return client;
    }

    private static HttpResponseMessage CreateNewsResponse()
    {
        var payload = new CompanyNewsResponse
        {
            News =
            [
                new CompanyNewsItem
                {
                    Uuid = "n1",
                    Title = "Apple launches new platform",
                    Publisher = "Reuters",
                    Link = "https://example.com/apple-1",
                    ProviderPublishTime = 1714500000,
                    Type = "STORY",
                    RelatedTickers = ["AAPL"]
                },
                new CompanyNewsItem
                {
                    Uuid = "n2",
                    Title = "Apple earnings expectations rise",
                    Publisher = "Bloomberg",
                    Link = "https://example.com/apple-2",
                    ProviderPublishTime = 1714600000,
                    Type = "STORY",
                    RelatedTickers = ["AAPL", "MSFT"]
                }
            ]
        };

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };
    }

    [Test]
    public async Task GetCompanyNewsAsync_ValidTicker_ReturnsNews()
    {
        var client = CreateMockHttpClient(CreateNewsResponse());
        var service = new CompanyNewsService(client);

        var result = await service.GetCompanyNewsAsync("AAPL", 5);

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsNotEmpty();
        await Assert.That(result?[0].Title).IsEqualTo("Apple launches new platform");
    }

    [Test]
    public async Task GetLatestCompanyNewsAsync_ValidTicker_ReturnsFirstNewsItem()
    {
        var client = CreateMockHttpClient(CreateNewsResponse());
        var service = new CompanyNewsService(client);

        var result = await service.GetLatestCompanyNewsAsync("AAPL");

        await Assert.That(result).IsNotNull();
        await Assert.That(result?.Uuid).IsEqualTo("n1");
    }

    [Test]
    public async Task GetCompanyNewsSinceAsync_FiltersByPublishTime()
    {
        var client = CreateMockHttpClient(CreateNewsResponse());
        var service = new CompanyNewsService(client);

        var result = await service.GetCompanyNewsSinceAsync("AAPL", 1714550000, 10);

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsNotEmpty();
        await Assert.That(result?.Count).IsEqualTo(1);
        await Assert.That(result?[0].Uuid).IsEqualTo("n2");
    }

    [Test]
    public async Task GetCompanyNewsAsync_EmptyTicker_ThrowsException()
    {
        var client = CreateMockHttpClient(CreateNewsResponse());
        var service = new CompanyNewsService(client);

        await Assert.That(async () => await service.GetCompanyNewsAsync("", 5))
            .ThrowsException()
            .WithMessageContaining("cannot be empty");
    }

    [Test]
    public async Task GetCompanyNewsAsync_NegativeCount_ThrowsException()
    {
        var client = CreateMockHttpClient(CreateNewsResponse());
        var service = new CompanyNewsService(client);

        await Assert.That(async () => await service.GetCompanyNewsAsync("AAPL", -1))
            .ThrowsException()
            .WithMessageContaining("non-negative");
    }

    [Test]
    public async Task GetCompanyNewsSinceAsync_NegativeTimestamp_ThrowsException()
    {
        var client = CreateMockHttpClient(CreateNewsResponse());
        var service = new CompanyNewsService(client);

        await Assert.That(async () => await service.GetCompanyNewsSinceAsync("AAPL", -1, 5))
            .ThrowsException()
            .WithMessageContaining("non-negative");
    }

    [Test]
    public async Task GetCompanyNewsAsync_HttpError_ThrowsException()
    {
        var errorResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("bad request")
        };
        var client = CreateMockHttpClient(errorResponse);
        var service = new CompanyNewsService(client);

        await Assert.That(async () => await service.GetCompanyNewsAsync("AAPL", 5))
            .ThrowsException()
            .WithMessageContaining("Yahoo API returned");
    }
}
