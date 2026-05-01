using CyFinance.Models.Search;
using CyFinance.Services.Search;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace CyFinance.Tests.Search
{
    public class SearchTests
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
            var client = new HttpClient(handler) { BaseAddress = new Uri("https://query2.finance.yahoo.com") };
            client.DefaultRequestHeaders.Add("X-CyFinance-SkipAuth", "1");
            return client;
        }

        private readonly ISearchService _service;

        public SearchTests()
        {
            // Prepare a minimal SearchResponse payload
            var mockPayload = new SearchResponse
            {
                Quotes = new List<SearchQuote>
                {
                    new SearchQuote
                    {
                        Symbol = "AAPL",
                        ShortName = "Apple Inc.",
                        LongName = "Apple Inc.",
                        QuoteType = "EQUITY",
                        Exchange = "NASDAQ"
                    }
                },
                News = new List<SearchNews>
                {
                    new SearchNews
                    {
                        Uuid = "test-uuid",
                        Title = "Apple announces new product",
                        Publisher = "Reuters",
                        Link = "https://example.com",
                        Source = "reuters"
                    }
                },
                Count = 2
            };
            var json = JsonSerializer.Serialize(mockPayload);
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
            var client = CreateMockHttpClient(mockResponse);

            _service = new SearchService(client);
        }

        [Test]
        public async Task SearchAsync_ValidQuery_ReturnsSearchResponse()
        {
            var query = "Apple";
            var result = await _service.SearchAsync(query);

            await Assert.That(result).IsNotNull();
            await Assert.That(result?.Quotes).IsNotNull();
            await Assert.That(result?.Quotes).IsNotEmpty();
        }

        [Test]
        public async Task SearchAsync_WithCounts_ReturnsSearchResponse()
        {
            var query = "Apple";
            var result = await _service.SearchAsync(query, quotesCount: 5, newsCount: 3);

            await Assert.That(result).IsNotNull();
            await Assert.That(result?.Quotes).IsNotEmpty();
        }

        [Test]
        public async Task SearchQuotesAsync_ValidQuery_ReturnsQuotes()
        {
            var query = "Apple";
            var result = await _service.SearchQuotesAsync(query);

            await Assert.That(result).IsNotNull();
            await Assert.That(result).IsNotEmpty();
            await Assert.That(result?[0]?.Symbol).IsEqualTo("AAPL");
        }

        [Test]
        public async Task SearchNewsAsync_ValidQuery_ReturnsNews()
        {
            var query = "Apple";
            var result = await _service.SearchNewsAsync(query);

            await Assert.That(result).IsNotNull();
            await Assert.That(result).IsNotEmpty();
        }

        [Test]
        public async Task SearchAsync_EmptyQuery_ThrowsException()
        {
            await Assert.That(async () => await _service.SearchAsync(""))
                .ThrowsException()
                .WithMessageContaining("cannot be empty");
        }

        [Test]
        public async Task SearchAsync_NegativeQuotesCount_ThrowsException()
        {
            await Assert.That(async () => await _service.SearchAsync("Apple", quotesCount: -1))
                .ThrowsException()
                .WithMessageContaining("non-negative");
        }

        [Test]
        public async Task SearchAsync_NegativeNewsCount_ThrowsException()
        {
            await Assert.That(async () => await _service.SearchAsync("Apple", newsCount: -1))
                .ThrowsException()
                .WithMessageContaining("non-negative");
        }
    }
}
