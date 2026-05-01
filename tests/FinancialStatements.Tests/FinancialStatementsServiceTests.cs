using CyFinance.Models.FinancialStatements;
using CyFinance.Models.QuoteSummary;
using CyFinance.Services.FinancialStatements;
using CyFinance.Services.QuoteSummary;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;

namespace CyFinance.Tests.FinancialStatements
{
    public class FinancialStatementsServiceTests
    {
        private readonly IQuoteSummaryService _mockQuoteSummaryService;
        private readonly IFinancialStatementsService _service;

        public FinancialStatementsServiceTests()
        {
            _mockQuoteSummaryService = Substitute.For<IQuoteSummaryService>();
            _service = new FinancialStatementsService(_mockQuoteSummaryService);
        }

        private QuoteResponse CreateMockQuoteResponse()
        {
            return new QuoteResponse(
                new CyFinance.Models.QuoteSummary.QuoteSummary(
                    new List<QuoteResult>
                    {
                        new QuoteResult
                        {
                            Price = new PriceData { Symbol = "AAPL" },
                            IncomeStatementHistory = new IncomeStatementHistory(
                                new List<FinancialStatement>
                                {
                                    new FinancialStatement { EndDate = new YahooLongValue(1704067200, "2024-12-31", null), TotalRevenue = new YahooLongValue(383285000000, "383.3B", null), NetIncome = new YahooLongValue(93736000000, "93.7B", null) }
                                }
                            ),
                            BalanceSheetHistory = new BalanceSheetHistory(
                                new List<FinancialStatement>
                                {
                                    new FinancialStatement { EndDate = new YahooLongValue(1704067200, "2024-12-31", null), TotalCash = new YahooLongValue(352755000000, "352.8B", null) }
                                }
                            ),
                            CashflowStatementHistory = new CashflowStatementHistory(
                                new List<FinancialStatement>
                                {
                                    new FinancialStatement { EndDate = new YahooLongValue(1704067200, "2024-12-31", null), TotalRevenue = new YahooLongValue(114315000000, "114.3B", null) }
                                }
                            )
                        }
                    },
                    null));
        }

        [Test]
        public async Task GetIncomeStatementAsync_ValidTicker_ReturnsIncomeStatement()
        {
            var ticker = "AAPL";
            var mockResponse = CreateMockQuoteResponse();
            _mockQuoteSummaryService.GetQuoteSummaryAsync(ticker, Arg.Any<string[]>()).Returns(mockResponse);

            var result = await _service.GetIncomeStatementAsync(ticker);

            await Assert.That(result).IsNotNull();
            await Assert.That(result?.Ticker).IsEqualTo(ticker);
            await Assert.That(result?.AnnualStatements).IsNotEmpty();
        }

        [Test]
        public async Task GetBalanceSheetAsync_ValidTicker_ReturnsBalanceSheet()
        {
            var ticker = "AAPL";
            var mockResponse = CreateMockQuoteResponse();
            _mockQuoteSummaryService.GetQuoteSummaryAsync(ticker, Arg.Any<string[]>()).Returns(mockResponse);

            var result = await _service.GetBalanceSheetAsync(ticker);

            await Assert.That(result).IsNotNull();
            await Assert.That(result?.Ticker).IsEqualTo(ticker);
            await Assert.That(result?.AnnualStatements).IsNotEmpty();
        }

        [Test]
        public async Task GetCashFlowStatementAsync_ValidTicker_ReturnsCashFlowStatement()
        {
            var ticker = "AAPL";
            var mockResponse = CreateMockQuoteResponse();
            _mockQuoteSummaryService.GetQuoteSummaryAsync(ticker, Arg.Any<string[]>()).Returns(mockResponse);

            var result = await _service.GetCashFlowStatementAsync(ticker);

            await Assert.That(result).IsNotNull();
            await Assert.That(result?.Ticker).IsEqualTo(ticker);
            await Assert.That(result?.AnnualStatements).IsNotEmpty();
        }

        [Test]
        public async Task GetAllStatementsAsync_ValidTicker_ReturnsAllStatements()
        {
            var ticker = "AAPL";
            var mockResponse = CreateMockQuoteResponse();
            _mockQuoteSummaryService.GetQuoteSummaryAsync(ticker, Arg.Any<string[]>()).Returns(mockResponse);

            var result = await _service.GetAllStatementsAsync(ticker);

            await Assert.That(result).IsNotNull();
            await Assert.That(result?.IncomeStatementHistory).IsNotNull();
            await Assert.That(result?.BalanceSheetHistory).IsNotNull();
            await Assert.That(result?.CashflowStatementHistory).IsNotNull();
        }

        [Test]
        public async Task GetIncomeStatementAsync_EmptyTicker_ThrowsException()
        {
            await Assert.That(async () => await _service.GetIncomeStatementAsync(""))
                .ThrowsException()
                .WithMessageContaining("cannot be empty");
        }

        [Test]
        public async Task GetBalanceSheetAsync_NullTicker_ThrowsException()
        {
            await Assert.That(async () => await _service.GetBalanceSheetAsync(null!))
                .ThrowsException();
        }

        [Test]
        public async Task GetStatementsAsync_NoModules_ThrowsException()
        {
            await Assert.That(async () => await _service.GetStatementsAsync("AAPL"))
                .ThrowsException()
                .WithMessageContaining("At least one module");
        }

        [Test]
        public async Task GetIncomeStatementAsync_WithMultipleRecords_ReturnsStatements()
        {
            var ticker = "AAPL";
            var mockResponse = CreateMockQuoteResponse();
            _mockQuoteSummaryService.GetQuoteSummaryAsync(ticker, Arg.Any<string[]>()).Returns(mockResponse);

            var result = await _service.GetIncomeStatementAsync(ticker);

            await Assert.That(result?.AnnualStatements).IsNotEmpty();
        }
    }
}
