using NSubstitute;
using TUnit.Core;
using TUnit.Assertions;
using YahooFinanceClient.Models;
using YahooFinanceClient.WebClient;
using System;
using System.Threading.Tasks;

// Assuming the class being tested is in this namespace
// using YahooFinanceClient.CsvParser; 

namespace YahooFinanceClient.Specs.CsvParser;

public class CsvParserTests
{
    private IWebClient _mockWebClient;
    private YahooFinanceClient.CsvParser.CsvParser _csvParser;

    // This method runs before each test, setting up a clean environment.
    [Before(Test)]
    public void Setup()
    {
        _mockWebClient = Substitute.For<IWebClient>();
        _csvParser = new YahooFinanceClient.CsvParser.CsvParser(_mockWebClient);
    }

    [Test]
    public async Task RetrieveStock_WithValidCsvData_CorrectlyParsesAllStockProperties()
    {
        // Arrange
        const string ticker = "AAPL";

        // Mock the different web client calls to return specific CSV strings
        _mockWebClient.DownloadFileAsync(ticker, "abb2b3pokjj5k4j6k5w")
            .Returns("136.66,136.61,130.0,132.0,136.53,135.91,145.91,138.91,131.91,115.91,-66.7%,+45.5%,115.91-130.01");

        _mockWebClient.DownloadFileAsync(ticker, "va5b6k3a2")
            .Returns("100000,120,130,130000,145000");

        _mockWebClient.DownloadFileAsync(ticker, "ghl1m3m4t8")
            .Returns("136.8,122.2,141.4,136.6,178.8,133.3");

        _mockWebClient.DownloadFileAsync(ticker, "ydr1q")
            .Returns("12.1,10.4,\"2/7/2017\",\"2/16/2017\"");

        _mockWebClient.DownloadFileAsync(ticker, "ee7e8e9b4j4p5p6rr2r5r6r7s7")
            .Returns("8.33,8.94,10.15,1.62,25.19,69.75B,3.36,5.55,16.68,11.11,1.69,15.54,13.69,1.67");

        // Act
        var result = await _csvParser.RetrieveStockAsync(ticker);

        // Assert
        // Group assertions by the data model for clarity
        // Pricing Data
        await Assert.That(result.PricingData.Ask).IsEqualTo(136.66M);
        await Assert.That(result.PricingData.Bid).IsEqualTo(136.61M);
        await Assert.That(result.PricingData.PreviousClose).IsEqualTo(136.53M);
        await Assert.That(result.PricingData.Open).IsEqualTo(135.91M);
        await Assert.That(result.PricingData.FiftyTwoWeekHigh).IsEqualTo(145.91M);
        await Assert.That(result.PricingData.FiftyTwoWeekLow).IsEqualTo(138.91M);
        await Assert.That(result.PricingData.FiftyTwoWeekLowChangePercent).IsEqualTo(-66.7M);
        await Assert.That(result.PricingData.FiftyTwoWeekHighChangePercent).IsEqualTo(45.5M);
        await Assert.That(result.PricingData.FiftyTwoWeekRange).IsEqualTo("115.91-130.01");

        // Volume Data
        await Assert.That(result.VolumeData.CurrentVolume).IsEqualTo(100000M);
        await Assert.That(result.VolumeData.AverageDailyVolume).IsEqualTo(145000M);

        // Average Data
        await Assert.That(result.AverageData.DayHigh).IsEqualTo(136.8M);
        await Assert.That(result.AverageData.DayLow).IsEqualTo(122.2M);
        await Assert.That(result.AverageData.FiftyDayMovingAverage).IsEqualTo(136.6M);
        await Assert.That(result.AverageData.TwoHundredDayMovingAverage).IsEqualTo(178.8M);

        // Dividend Data
        await Assert.That(result.DividendData.DividendYield).IsEqualTo(12.1M);
        await Assert.That(result.DividendData.DividendPerShare).IsEqualTo(10.4M);
        await Assert.That(result.DividendData.DividendPayDate).IsEqualTo(new DateTime(2017, 2, 7));
        await Assert.That(result.DividendData.ExDividendDate).IsEqualTo(new DateTime(2017, 2, 16));

        // Ratio Data
        await Assert.That(result.RatioData.EarningsPerShare).IsEqualTo(8.33M);
        await Assert.That(result.RatioData.BookValue).IsEqualTo(25.19M);
        await Assert.That(result.RatioData.Ebitda).IsEqualTo("69.75B");
        await Assert.That(result.RatioData.PeRatio).IsEqualTo(16.68M);
        await Assert.That(result.RatioData.PegRatio).IsEqualTo(1.69M);
        await Assert.That(result.RatioData.ShortRatio).IsEqualTo(1.67M);
    }

    [Test]
    public async Task RetrieveStock_WithNotAvailableCsvData_CorrectlyParsesAllPropertiesAsNull()
    {
        // Arrange
        const string ticker = "AAPL";

        _mockWebClient.DownloadFileAsync(ticker, Arg.Any<string>())
            .Returns("N/A,N/A,N/A,N/A,N/A,N/A,N/A,N/A,N/A,N/A,N/A,N/A,N/A,N/A");

        // Act
        var result = await _csvParser.RetrieveStockAsync(ticker);

        // Pricing Data
        await Assert.That(result.PricingData.Ask).IsNull();
        await Assert.That(result.PricingData.Bid).IsNull();
        await Assert.That(result.PricingData.PreviousClose).IsNull();
        await Assert.That(result.PricingData.Open).IsNull();
        await Assert.That(result.PricingData.FiftyTwoWeekHigh).IsNull();
        await Assert.That(result.PricingData.FiftyTwoWeekLow).IsNull();
        await Assert.That(result.PricingData.FiftyTwoWeekRange).IsNull();

        // Volume Data
        await Assert.That(result.VolumeData.CurrentVolume).IsNull();
        await Assert.That(result.VolumeData.AverageDailyVolume).IsNull();

        // Average Data
        await Assert.That(result.AverageData.DayHigh).IsNull();
        await Assert.That(result.AverageData.DayLow).IsNull();
        await Assert.That(result.AverageData.FiftyDayMovingAverage).IsNull();
        await Assert.That(result.AverageData.TwoHundredDayMovingAverage).IsNull();

        // Dividend Data
        await Assert.That(result.DividendData.DividendYield).IsNull();
        await Assert.That(result.DividendData.DividendPerShare).IsNull();
        await Assert.That(result.DividendData.DividendPayDate).IsNull();
        await Assert.That(result.DividendData.ExDividendDate).IsNull();

        // Ratio Data
        await Assert.That(result.RatioData.EarningsPerShare).IsNull();
        await Assert.That(result.RatioData.BookValue).IsNull();
        await Assert.That(result.RatioData.Ebitda).IsNull();
        await Assert.That(result.RatioData.PeRatio).IsNull();
        await Assert.That(result.RatioData.PegRatio).IsNull();
        await Assert.That(result.RatioData.ShortRatio).IsNull();
    }
}