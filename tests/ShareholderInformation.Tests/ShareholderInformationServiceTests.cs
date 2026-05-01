using CyFinance.Models.QuoteSummary;
using CyFinance.Models.ShareholderInformation;
using CyFinance.Services.QuoteSummary;
using CyFinance.Services.ShareholderInformation;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace CyFinance.Tests.ShareholderInformation;

public class ShareholderInformationServiceTests
{
    private readonly IQuoteSummaryService _mockQuoteSummaryService;
    private readonly IShareholderInformationService _service;

    public ShareholderInformationServiceTests()
    {
        _mockQuoteSummaryService = Substitute.For<IQuoteSummaryService>();
        _service = new ShareholderInformationService(_mockQuoteSummaryService);
    }

    private static QuoteResponse CreateMockQuoteResponse()
    {
        return new QuoteResponse(
            new CyFinance.Models.QuoteSummary.QuoteSummary(
                new List<QuoteResult>
                {
                    new QuoteResult
                    {
                        Price = new PriceData { Symbol = "AAPL" },
                        MajorHoldersBreakdown = new MajorHoldersBreakdown
                        {
                            InsidersPercentHeld = new YahooValue(0.005, "0.5%"),
                            InstitutionsPercentHeld = new YahooValue(0.62, "62%"),
                            InstitutionsFloatPercentHeld = new YahooValue(0.63, "63%"),
                            InstitutionsCount = new YahooLongValue(1200, "1.2K", "1,200")
                        },
                        InstitutionOwnership = new OwnershipContainer(
                            new List<OwnershipEntry>
                            {
                                new OwnershipEntry
                                {
                                    Organization = "Vanguard Group, Inc.",
                                    PctHeld = new YahooValue(0.085, "8.5%"),
                                    Position = new YahooLongValue(1350000000, "1.35B", "1,350,000,000"),
                                    Value = new YahooLongValue(250000000000, "250B", "250,000,000,000")
                                }
                            }
                        ),
                        FundOwnership = new OwnershipContainer(
                            new List<OwnershipEntry>
                            {
                                new OwnershipEntry
                                {
                                    Organization = "Vanguard Total Stock Market Index Fund",
                                    PctHeld = new YahooValue(0.032, "3.2%"),
                                    Position = new YahooLongValue(500000000, "500M", "500,000,000"),
                                    Value = new YahooLongValue(92000000000, "92B", "92,000,000,000")
                                }
                            }
                        ),
                        InsiderHolders = new InsiderHolders(
                            new List<InsiderHolderEntry>
                            {
                                new InsiderHolderEntry
                                {
                                    Name = "Tim Cook",
                                    Relation = "Chief Executive Officer",
                                    PositionDirect = new YahooLongValue(3200000, "3.2M", "3,200,000")
                                }
                            }
                        ),
                        InsiderTransactions = new InsiderTransactions(
                            new List<InsiderTransactionEntry>
                            {
                                new InsiderTransactionEntry
                                {
                                    FilerName = "Tim Cook",
                                    FilerRelation = "CEO",
                                    TransactionText = "Sale",
                                    Shares = new YahooLongValue(50000, "50K", "50,000")
                                }
                            }
                        )
                    }
                },
                null));
    }

    [Test]
    public async Task GetShareholderInformationAsync_ValidTicker_ReturnsSummary()
    {
        var ticker = "AAPL";
        var mockResponse = CreateMockQuoteResponse();
        _mockQuoteSummaryService.GetQuoteSummaryAsync(ticker, Arg.Any<string[]>()).Returns(mockResponse);

        var result = await _service.GetShareholderInformationAsync(ticker);

        await Assert.That(result).IsNotNull();
        await Assert.That(result?.Ticker).IsEqualTo(ticker);
        await Assert.That(result?.MajorHoldersBreakdown).IsNotNull();
        await Assert.That(result?.InstitutionalOwnership).IsNotEmpty();
        await Assert.That(result?.FundOwnership).IsNotEmpty();
        await Assert.That(result?.InsiderHolders).IsNotEmpty();
        await Assert.That(result?.InsiderTransactions).IsNotEmpty();
    }

    [Test]
    public async Task GetMajorHoldersBreakdownAsync_ValidTicker_ReturnsBreakdown()
    {
        var ticker = "AAPL";
        _mockQuoteSummaryService.GetQuoteSummaryAsync(ticker, Arg.Any<string[]>()).Returns(CreateMockQuoteResponse());

        var result = await _service.GetMajorHoldersBreakdownAsync(ticker);

        await Assert.That(result).IsNotNull();
        await Assert.That(result?.InstitutionsPercentHeld?.Raw).IsEqualTo(0.62);
    }

    [Test]
    public async Task GetInstitutionalOwnershipAsync_ValidTicker_ReturnsOwnershipList()
    {
        var ticker = "AAPL";
        _mockQuoteSummaryService.GetQuoteSummaryAsync(ticker, Arg.Any<string[]>()).Returns(CreateMockQuoteResponse());

        var result = await _service.GetInstitutionalOwnershipAsync(ticker);

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsNotEmpty();
        await Assert.That(result?[0].Organization).IsEqualTo("Vanguard Group, Inc.");
    }

    [Test]
    public async Task GetInsiderTransactionsAsync_ValidTicker_ReturnsTransactions()
    {
        var ticker = "AAPL";
        _mockQuoteSummaryService.GetQuoteSummaryAsync(ticker, Arg.Any<string[]>()).Returns(CreateMockQuoteResponse());

        var result = await _service.GetInsiderTransactionsAsync(ticker);

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsNotEmpty();
        await Assert.That(result?[0].TransactionText).IsEqualTo("Sale");
    }

    [Test]
    public async Task GetShareholderInformationAsync_EmptyTicker_ThrowsException()
    {
        await Assert.That(async () => await _service.GetShareholderInformationAsync(""))
            .ThrowsException()
            .WithMessageContaining("cannot be empty");
    }

    [Test]
    public async Task GetShareholderInformationAsync_NullTicker_ThrowsException()
    {
        await Assert.That(async () => await _service.GetShareholderInformationAsync(null!))
            .ThrowsException();
    }

    [Test]
    public async Task GetShareholderInformationAsync_NoResult_ReturnsNull()
    {
        var ticker = "UNKNOWN";
        var emptyResponse = new QuoteResponse(new CyFinance.Models.QuoteSummary.QuoteSummary(new List<QuoteResult>(), null));
        _mockQuoteSummaryService.GetQuoteSummaryAsync(ticker, Arg.Any<string[]>()).Returns(emptyResponse);

        var result = await _service.GetShareholderInformationAsync(ticker);

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task ShareholderInformationSummary_ConvenienceMethods_Work()
    {
        var summary = new ShareholderInformationSummary
        {
            MajorHoldersBreakdown = new MajorHoldersBreakdown
            {
                InstitutionsPercentHeld = new YahooValue(0.62, "62%"),
                InsidersPercentHeld = new YahooValue(0.005, "0.5%")
            },
            InstitutionalOwnership = new List<OwnershipEntry>
            {
                new OwnershipEntry { Organization = "A", Position = new YahooLongValue(100, "100", "100") },
                new OwnershipEntry { Organization = "B", Position = new YahooLongValue(200, "200", "200") }
            }
        };

        await Assert.That(summary.GetInstitutionalOwnershipPercent()).IsEqualTo(62.0);
        await Assert.That(summary.GetInsiderOwnershipPercent()).IsEqualTo(0.5);
        await Assert.That(summary.GetLargestInstitutionalHolder()?.Organization).IsEqualTo("B");
    }
}
