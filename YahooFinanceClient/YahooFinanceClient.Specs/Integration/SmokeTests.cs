using TUnit.Core;
using TUnit.Assertions;
using System.Threading.Tasks;
using YahooFinance;
using YahooFinanceClient.Models;

namespace YahooFinanceClient.Specs.Integration;

public class SmokeTests
{
    [Test]
    public async Task RetrieveStock_SmokeTest_ConstructsStockAndSubmodels()
    {
        // Arrange
        var client = new YahooFinance.YahooFinance();

        // Act
        var stock = await client.RetrieveStockAsync("AAPL");

        // Assert: ensure the main object and sub-models are instantiated
        await Assert.That(stock).IsNotNull();
        await Assert.That(stock.PricingData).IsNotNull();
        await Assert.That(stock.VolumeData).IsNotNull();
        await Assert.That(stock.AverageData).IsNotNull();
        await Assert.That(stock.DividendData).IsNotNull();
        await Assert.That(stock.RatioData).IsNotNull();
    }
}