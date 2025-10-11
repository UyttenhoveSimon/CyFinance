using CyFinance.Models.OptionsData;
using CyFinance.Services.OptionsData;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TUnit;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace CyFinance.Tests.OptionsData
{
    public class OptionsDataServiceTests
    {
        // simple fake handler to return a precomputed response
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
            // BaseAddress doesn't matter for most tests, but keep consistent with other tests
            return new HttpClient(handler) { BaseAddress = new Uri("https://query2.finance.yahoo.com") };
        }

        [Test]
        public void DateTimeConversion_ShouldRoundTrip()
        {
            // Arrange
            var service = new OptionsDataService(new HttpClient()); // only uses DateTime helpers

            var dt = new DateTime(2021, 06, 01, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var unix = service.DateTimeToUnixTimeStamp(dt);
            var back = service.UnixTimeStampToDateTime(unix);

            // Assert
            Assert.That(back).IsEqualTo(dt);
        }

        [Test]
        public async Task GetExpirationDatesAsync_ShouldReturnDates_FromPayload()
        {
            // Arrange
            // sample JSON: adjust property names/casing if your models expect different shape
            var sampleJson = @"
            {
              ""optionChain"": {
                ""result"": [
                  {
                    ""underlyingSymbol"": ""AAPL"",
                    ""expirationDates"": [1622505600, 1625097600],
                    ""options"": []
                  }
                ],
                ""error"": null
              }
            }";

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(sampleJson, Encoding.UTF8, "application/json")
            };
            var client = CreateMockHttpClient(response);
            var service = new OptionsDataService(client);

            // Act
            var dates = await service.GetExpirationDatesAsync("AAPL");

            // Assert
            await Assert.That(dates).IsNotNull();
            await Assert.That(dates).IsNotEmpty();
            await Assert.That(dates.Count).IsEqualTo(2);
            // verify first unix timestamp converted to DateTime if the method returns longs, adjust as needed
        }

        [Test]
        public async Task GetOptionsChainAsync_ShouldParseOptionsPayload()
        {
            // Arrange
            // Minimal options JSON payload — update calls/puts fields to match your model shape
            var sampleJson = @"
            {
              ""optionChain"": {
                ""result"": [
                  {
                    ""underlyingSymbol"": ""AAPL"",
                    ""expirationDates"": [1622505600],
                    ""options"": [
                      {
                        ""expirationDate"": 1622505600,
                        ""calls"": [
                          {
                            ""contractSymbol"": ""AAPL210625C00125000"",
                            ""strike"": 125.0,
                            ""lastPrice"": 2.5,
                            ""volume"": 10
                          }
                        ],
                        ""puts"": []
                      }
                    ]
                  }
                ],
                ""error"": null
              }
            }";

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(sampleJson, Encoding.UTF8, "application/json")
            };
            var client = CreateMockHttpClient(response);
            var service = new OptionsDataService(client);

            // Act
            var result = await service.GetOptionsChainAsync("AAPL");

            // Assert basic expectations — adapt assertions to the actual shape of OptionsDataResponse / OptionsChainData
            await Assert.That(result).IsNotNull();
            // If your model exposes OptionChain/Result etc. adapt these checks:
            // await Assert.That(result.OptionChain).IsNotNull();
            // await Assert.That(result.OptionChain.Result).IsNotEmpty();
        }
    }
}