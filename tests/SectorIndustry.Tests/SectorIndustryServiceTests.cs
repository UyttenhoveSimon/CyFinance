using CyFinance.Models.QuoteSummary;
using CyFinance.Models.SectorIndustry;
using CyFinance.Models.StockScreening;
using CyFinance.Services.QuoteSummary;
using CyFinance.Services.SectorIndustry;
using CyFinance.Services.StockScreening;
using NSubstitute;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace CyFinance.Tests.SectorIndustry;

public class SectorIndustryServiceTests
{
    private readonly IQuoteSummaryService _mockQuoteSummaryService;
    private readonly IStockScreeningService _mockScreeningService;
    private readonly ISectorIndustryService _service;

    public SectorIndustryServiceTests()
    {
        _mockQuoteSummaryService = Substitute.For<IQuoteSummaryService>();
        _mockScreeningService = Substitute.For<IStockScreeningService>();
        _service = new SectorIndustryService(_mockQuoteSummaryService, _mockScreeningService);
    }

    private static QuoteResponse BuildAssetProfileResponse(
        string ticker,
        string sector = "Technology",
        string sectorKey = "technology",
        string industry = "Software—Application",
        string industryKey = "software-application",
        string website = "https://www.apple.com",
        string country = "United States",
        int fullTimeEmployees = 164000)
    {
        return new QuoteResponse(
            new CyFinance.Models.QuoteSummary.QuoteSummary(
                new List<QuoteResult>
                {
                    new QuoteResult
                    {
                        AssetProfile = new AssetProfile
                        {
                            Sector = sector,
                            SectorKey = sectorKey,
                            Industry = industry,
                            IndustryKey = industryKey,
                            Website = website,
                            Country = country,
                            FullTimeEmployees = fullTimeEmployees
                        }
                    }
                },
                null));
    }

    private static ScreenerResult BuildScreenerResult(params string[] symbols)
    {
        return new ScreenerResult
        {
            Id = "test",
            Title = "Test Screener",
            Total = symbols.Length,
            Quotes = symbols.Select(s => new CyFinance.Models.StockScreening.Quote
            {
                Symbol = s,
                LongName = $"{s} Inc",
                Exchange = "NASDAQ",
                RegularMarketPrice = 100m,
                RegularMarketChange = 1.5m,
                RegularMarketChangePercent = 0.015m,
                MarketCap = 1_000_000_000L,
                RegularMarketVolume = 5_000_000L
            }).ToList()
        };
    }

    [Test]
    public async Task GetSectorInfoAsync_ShouldReturnMappedInfo()
    {
        const string ticker = "AAPL";
        _mockQuoteSummaryService.GetQuoteSummaryAsync(ticker, Arg.Any<string[]>())
            .Returns(BuildAssetProfileResponse(ticker));

        var result = await _service.GetSectorInfoAsync(ticker);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Ticker).IsEqualTo(ticker);
        await Assert.That(result.Sector).IsEqualTo("Technology");
        await Assert.That(result.SectorKey).IsEqualTo("technology");
        await Assert.That(result.Industry).IsEqualTo("Software—Application");
        await Assert.That(result.IndustryKey).IsEqualTo("software-application");
        await Assert.That(result.Country).IsEqualTo("United States");
        await Assert.That(result.FullTimeEmployees).IsEqualTo(164000);
    }

    [Test]
    public async Task GetStocksInSectorAsync_ShouldReturnMappedEntries()
    {
        _mockScreeningService.ScreenAsync(Arg.Any<ScreenerRequest>())
            .Returns(BuildScreenerResult("AAPL", "MSFT", "GOOGL"));

        var result = await _service.GetStocksInSectorAsync("Technology", 3);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Count).IsEqualTo(3);
        await Assert.That(result[0].Symbol).IsEqualTo("AAPL");
        await Assert.That(result[1].Symbol).IsEqualTo("MSFT");
        await Assert.That(result[2].Symbol).IsEqualTo("GOOGL");
    }

    [Test]
    public async Task GetStocksInIndustryAsync_ShouldReturnMappedEntries()
    {
        _mockScreeningService.ScreenAsync(Arg.Any<ScreenerRequest>())
            .Returns(BuildScreenerResult("CRM", "NOW"));

        var result = await _service.GetStocksInIndustryAsync("Software—Application", 2);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Count).IsEqualTo(2);
        await Assert.That(result[0].Symbol).IsEqualTo("CRM");
    }

    [Test]
    public async Task GetStocksInSectorAsync_NullScreenerResult_ShouldReturnEmptyList()
    {
        _mockScreeningService.ScreenAsync(Arg.Any<ScreenerRequest>())
            .Returns((ScreenerResult?)null);

        var result = await _service.GetStocksInSectorAsync("Financials");

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Count).IsEqualTo(0);
    }

    [Test]
    public async Task GetSectorInfoAsync_EmptyTicker_ShouldThrow()
    {
        await Assert.That(async () => await _service.GetSectorInfoAsync(""))
            .ThrowsException()
            .WithMessageContaining("Ticker cannot be empty");
    }

    [Test]
    public async Task GetStocksInSectorAsync_EmptySector_ShouldThrow()
    {
        await Assert.That(async () => await _service.GetStocksInSectorAsync(" "))
            .ThrowsException()
            .WithMessageContaining("Sector cannot be empty");
    }
}
