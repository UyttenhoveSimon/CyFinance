using CyFinance.Models.FundData;
using CyFinance.Models.QuoteSummary;
using CyFinance.Services.FundData;
using CyFinance.Services.QuoteSummary;
using NSubstitute;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace CyFinance.Tests.FundData;

public class FundDataServiceTests
{
    private readonly IQuoteSummaryService _mockQuoteSummaryService;
    private readonly IFundDataService _service;

    public FundDataServiceTests()
    {
        _mockQuoteSummaryService = Substitute.For<IQuoteSummaryService>();
        _service = new FundDataService(_mockQuoteSummaryService);
    }

    private static QuoteResponse BuildFundResponse(
        string ticker = "VOO",
        string family = "Vanguard",
        string category = "Large Blend",
        string legalType = "Exchange Traded Fund",
        double expenseRatio = 0.0003,
        double grossExpRatio = 0.0003,
        List<FundHolding>? holdings = null,
        FundTrailingReturns? trailingReturns = null,
        List<FundAnnualReturn>? annualReturns = null)
    {
        return new QuoteResponse(
            new CyFinance.Models.QuoteSummary.QuoteSummary(
                new List<QuoteResult>
                {
                    new QuoteResult
                    {
                        Price = new PriceData
                        {
                            Symbol = ticker,
                            LongName = "Vanguard S&P 500 ETF",
                            RegularMarketPrice = new YahooValue(450.0, "450.00")
                        },
                        FundProfile = new FundProfileData
                        {
                            Family = family,
                            CategoryName = category,
                            LegalType = legalType,
                            ManagementInfo = new FundManagementInfo { ManagerName = "Michael Perre" },
                            FeesExpensesInvestment = new FundFeesExpenses
                            {
                                AnnualReportExpenseRatio = new YahooValue(expenseRatio, "0.03%"),
                                GrossExpRatio = new YahooValue(grossExpRatio, "0.03%"),
                                FrontEndSalesLoad = new YahooValue(0, "0.00%")
                            }
                        },
                        TopHoldings = new TopHoldingsData
                        {
                            StockPosition = new YahooValue(0.9989, "99.89%"),
                            BondPosition = new YahooValue(0.0, "0.00%"),
                            Holdings = holdings ?? new List<FundHolding>
                            {
                                new FundHolding { Symbol = "AAPL", HoldingName = "Apple Inc", HoldingPercent = new YahooValue(0.07, "7.00%") },
                                new FundHolding { Symbol = "MSFT", HoldingName = "Microsoft Corp", HoldingPercent = new YahooValue(0.065, "6.50%") }
                            },
                            SectorWeightings = new List<Dictionary<string, YahooValue>>
                            {
                                new() { ["technology"] = new YahooValue(0.31, "31.00%") },
                                new() { ["healthcare"] = new YahooValue(0.13, "13.00%") }
                            }
                        },
                        FundPerformance = new FundPerformanceData
                        {
                            TrailingReturns = trailingReturns ?? new FundTrailingReturns
                            {
                                OneMonth = new YahooValue(0.02, "2.00%"),
                                ThreeMonth = new YahooValue(0.05, "5.00%"),
                                Ytd = new YahooValue(0.12, "12.00%"),
                                OneYear = new YahooValue(0.22, "22.00%"),
                                ThreeYear = new YahooValue(0.10, "10.00%"),
                                FiveYear = new YahooValue(0.14, "14.00%"),
                                TenYear = new YahooValue(0.13, "13.00%")
                            },
                            AnnualTotalReturns = new FundAnnualReturns(
                                annualReturns ?? new List<FundAnnualReturn>
                                {
                                    new FundAnnualReturn { Year = "2023", AnnualValue = new YahooValue(0.2659, "26.59%") },
                                    new FundAnnualReturn { Year = "2022", AnnualValue = new YahooValue(-0.1873, "-18.73%") }
                                })
                        }
                    }
                },
                null));
    }

    [Test]
    public async Task GetFundSummaryAsync_ShouldReturnMappedSummary()
    {
        const string ticker = "VOO";
        _mockQuoteSummaryService.GetQuoteSummaryAsync(ticker, Arg.Any<string[]>())
            .Returns(BuildFundResponse(ticker));

        var result = await _service.GetFundSummaryAsync(ticker);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Ticker).IsEqualTo(ticker);
        await Assert.That(result.FundFamily).IsEqualTo("Vanguard");
        await Assert.That(result.Category).IsEqualTo("Large Blend");
        await Assert.That(result.LegalType).IsEqualTo("Exchange Traded Fund");
        await Assert.That(result.Price).IsEqualTo(450.0);
        await Assert.That(result.ExpenseRatio).IsEqualTo(0.0003);
        await Assert.That(result.TopHoldings).IsNotNull();
        await Assert.That(result.TopHoldings!.Count).IsEqualTo(2);
        await Assert.That(result.SectorWeightings).IsNotNull();
        await Assert.That(result.SectorWeightings!.Count).IsEqualTo(2);
    }

    [Test]
    public async Task GetFundProfileAsync_ShouldReturnMappedProfile()
    {
        const string ticker = "VOO";
        _mockQuoteSummaryService.GetQuoteSummaryAsync(ticker, Arg.Any<string[]>())
            .Returns(BuildFundResponse(ticker));

        var result = await _service.GetFundProfileAsync(ticker);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.FundFamily).IsEqualTo("Vanguard");
        await Assert.That(result.ManagerName).IsEqualTo("Michael Perre");
        await Assert.That(result.ExpenseRatio).IsEqualTo(0.0003);
        await Assert.That(result.FrontEndSalesLoad).IsEqualTo(0.0);
    }

    [Test]
    public async Task GetTopHoldingsAsync_ShouldReturnHoldingsList()
    {
        const string ticker = "SPY";
        _mockQuoteSummaryService.GetQuoteSummaryAsync(ticker, Arg.Any<string[]>())
            .Returns(BuildFundResponse(ticker));

        var result = await _service.GetTopHoldingsAsync(ticker);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Count).IsEqualTo(2);
        await Assert.That(result[0].Symbol).IsEqualTo("AAPL");
        await Assert.That(result[0].Percent).IsEqualTo(0.07);
    }

    [Test]
    public async Task GetSectorWeightingsAsync_ShouldReturnWeightings()
    {
        const string ticker = "VOO";
        _mockQuoteSummaryService.GetQuoteSummaryAsync(ticker, Arg.Any<string[]>())
            .Returns(BuildFundResponse(ticker));

        var result = await _service.GetSectorWeightingsAsync(ticker);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Count).IsEqualTo(2);
        await Assert.That(result[0].Sector).IsEqualTo("technology");
        await Assert.That(result[0].Weight).IsEqualTo(0.31);
    }

    [Test]
    public async Task GetTrailingReturnsAsync_ShouldReturnTrailingReturns()
    {
        const string ticker = "VOO";
        _mockQuoteSummaryService.GetQuoteSummaryAsync(ticker, Arg.Any<string[]>())
            .Returns(BuildFundResponse(ticker));

        var result = await _service.GetTrailingReturnsAsync(ticker);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.OneYear).IsEqualTo(0.22);
        await Assert.That(result.FiveYear).IsEqualTo(0.14);
        await Assert.That(result.TenYear).IsEqualTo(0.13);
        await Assert.That(result.Ytd).IsEqualTo(0.12);
    }

    [Test]
    public async Task GetAnnualReturnsAsync_ShouldReturnAnnualEntries()
    {
        const string ticker = "VOO";
        _mockQuoteSummaryService.GetQuoteSummaryAsync(ticker, Arg.Any<string[]>())
            .Returns(BuildFundResponse(ticker));

        var result = await _service.GetAnnualReturnsAsync(ticker);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Count).IsEqualTo(2);
        await Assert.That(result[0].Year).IsEqualTo("2023");
        await Assert.That(result[0].ReturnPercent).IsEqualTo(0.2659);
        await Assert.That(result[1].Year).IsEqualTo("2022");
        await Assert.That(result[1].ReturnPercent).IsEqualTo(-0.1873);
    }

    [Test]
    public async Task GetFundSummaryAsync_EmptyTicker_ShouldThrow()
    {
        await Assert.That(async () => await _service.GetFundSummaryAsync("  "))
            .ThrowsException()
            .WithMessageContaining("Ticker cannot be empty");
    }
}
