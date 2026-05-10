using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using CyFinance.Models.AnalystRecommendations;
using CyFinance.Models.CompanyNews;
using CyFinance.Models.Crypto;
using CyFinance.Models.Currency;
using CyFinance.Models.EarningsCalendar;
using CyFinance.Models.FinancialStatements;
using CyFinance.Models.FundData;
using CyFinance.Models.GlobalCalendars;
using CyFinance.Models.HistoricalData;
using CyFinance.Models.MarketSummary;
using CyFinance.Models.QuoteSummary;
using CyFinance.Models.Search;
using CyFinance.Models.SectorIndustry;
using CyFinance.Models.ShareholderInformation;
using CyFinance.Models.StockScreening;
using CyFinance.Services.AnalystRecommendations;
using CyFinance.Services.CompanyNews;
using CyFinance.Services.Crypto;
using CyFinance.Services.Currency;
using CyFinance.Services.EarningsCalendar;
using CyFinance.Services.FinancialStatements;
using CyFinance.Services.FundData;
using CyFinance.Services.GlobalCalendars;
using CyFinance.Services.HistoricalData;
using CyFinance.Services.MarketSummary;
using CyFinance.Services.OptionsData;
using CyFinance.Services.QuoteSummary;
using CyFinance.Services.Search;
using CyFinance.Services.SectorIndustry;
using CyFinance.Services.ShareholderInformation;
using CyFinance.Services.StockScreening;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace CyFinance.Tests.Integration;

public class LiveYahooApiIntegrationTests
{
    private static readonly HttpClient SharedHttpClient = CreateHttpClient();
    private static bool RunLiveYahooTests => string.Equals(
        Environment.GetEnvironmentVariable("RUN_LIVE_YAHOO_TESTS"),
        "1",
        StringComparison.Ordinal);

    private readonly IQuoteSummaryService _quoteSummaryService;
    private readonly IHistoricalDataService _historicalDataService;
    private readonly ICryptoService _cryptoService;
    private readonly ICurrencyService _currencyService;
    private readonly ISearchService _searchService;
    private readonly IEarningsCalendarService _earningsCalendarService;
    private readonly IOptionsDataService _optionsDataService;
    private readonly IAnalystRecommendationsService _analystRecommendationsService;
    private readonly IFinancialStatementsService _financialStatementsService;
    private readonly IFundDataService _fundDataService;
    private readonly ICompanyNewsService _companyNewsService;
    private readonly IGlobalCalendarsService _globalCalendarsService;
    private readonly IMarketSummaryService _marketSummaryService;
    private readonly ISectorIndustryService _sectorIndustryService;
    private readonly IShareholderInformationService _shareholderInformationService;
    private readonly IStockScreeningService _stockScreeningService;

    public LiveYahooApiIntegrationTests()
    {
        _quoteSummaryService = new QuoteSummaryService(SharedHttpClient);
        _historicalDataService = new HistoricalDataService(SharedHttpClient);
        _cryptoService = new CryptoService(SharedHttpClient);
        _currencyService = new CurrencyService(SharedHttpClient);
        _searchService = new SearchService(SharedHttpClient);
        _optionsDataService = new OptionsDataService(SharedHttpClient);
        _companyNewsService = new CompanyNewsService(SharedHttpClient);
        _stockScreeningService = new StockScreeningService(SharedHttpClient);
        _marketSummaryService = new MarketSummaryService(SharedHttpClient);
        _globalCalendarsService = new GlobalCalendarsService(SharedHttpClient, _stockScreeningService);

        _analystRecommendationsService = new AnalystRecommendationsService(_quoteSummaryService);
        _financialStatementsService = new FinancialStatementsService(_quoteSummaryService);
        _fundDataService = new FundDataService(_quoteSummaryService);
        _earningsCalendarService = new EarningsCalendarService(_quoteSummaryService);
        _shareholderInformationService = new ShareholderInformationService(_quoteSummaryService);
        _sectorIndustryService = new SectorIndustryService(_quoteSummaryService, _stockScreeningService);
    }

    [Test]
    public async Task QuoteSummaryService_GetQuoteSummaryAsync_ReturnsLiveSummary()
    {
        if (!RunLiveYahooTests)
        {
            return;
        }

        var result = await _quoteSummaryService.GetQuoteSummaryAsync("AAPL");

        await Assert.That(result).IsNotNull();
        await Assert.That(result?.QuoteSummary?.Result).IsNotNull();
        await Assert.That(result?.QuoteSummary?.Result).IsNotEmpty();
        await Assert.That(result?.QuoteSummary?.Result?.FirstOrDefault()?.Price?.Symbol).IsEqualTo("AAPL");
    }

    [Test]
    public async Task HistoricalDataService_EndToEndMethods_WorkAgainstYahoo()
    {
        if (!RunLiveYahooTests)
        {
            return;
        }

        var startDate = new DateTime(2020, 8, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2020, 9, 30, 0, 0, 0, DateTimeKind.Utc);

        var chart = await _historicalDataService.GetHistoricalDataAsync(
            "AAPL",
            startDate,
            endDate,
            ChartInterval.OneDay,
            includeDividends: true,
            includeSplits: false);

        await Assert.That(chart).IsNotNull();
        await Assert.That(chart.Chart.Result).IsNotEmpty();
        await Assert.That(HistoricalDataService.GetMetadata(chart).Symbol).IsEqualTo("AAPL");

        var historicalPrices = await _historicalDataService.GetHistoricalPricesAsync(
            "AAPL",
            startDate,
            endDate,
            ChartInterval.OneDay,
            includeDividends: true,
            includeSplits: false);

        await Assert.That(historicalPrices).IsNotNull();
        await Assert.That(historicalPrices).IsNotEmpty();

        var dividends = _historicalDataService.GetDividends(chart);
        var splits = _historicalDataService.GetSplits(chart);

        await Assert.That(dividends).IsNotNull();
        await Assert.That(dividends).IsNotEmpty();
        await Assert.That(splits).IsNotNull();
    }

    [Test]
    public async Task CryptoService_EndToEndMethods_WorkAgainstYahoo()
    {
        if (!RunLiveYahooTests)
        {
            return;
        }

        var quote = await _cryptoService.GetCryptoQuoteAsync("BTC");
        var historicalPrices = await _cryptoService.GetHistoricalPricesAsync(
            "BTC",
            interval: ChartInterval.OneDay);

        await Assert.That(quote).IsNotNull();
        await Assert.That(quote?.Symbol).IsEqualTo("BTC-USD");
        await Assert.That(quote?.Price).IsNotNull();
        await Assert.That(historicalPrices).IsNotNull();
        await Assert.That(historicalPrices).IsNotEmpty();
    }

    [Test]
    public async Task CurrencyService_EndToEndMethods_WorkAgainstYahoo()
    {
        if (!RunLiveYahooTests)
        {
            return;
        }

        var exchangeRate = await _currencyService.GetExchangeRateAsync("EUR", "USD");
        var historicalRates = await _currencyService.GetHistoricalRatesAsync(
            "EUR",
            "USD",
            DateTime.UtcNow.AddDays(-10),
            DateTime.UtcNow,
            ChartInterval.OneDay);
        var convertedAmount = await _currencyService.ConvertAsync(10, "EUR", "USD");

        await Assert.That(exchangeRate).IsNotNull();
        await Assert.That(exchangeRate?.Symbol).IsEqualTo("EURUSD=X");
        await Assert.That(historicalRates).IsNotNull();
        await Assert.That(historicalRates).IsNotEmpty();
        await Assert.That(convertedAmount).IsNotNull();
        await Assert.That(convertedAmount!.Value).IsGreaterThan(0d);
    }

    [Test]
    public async Task SearchService_EndToEndMethods_WorkAgainstYahoo()
    {
        if (!RunLiveYahooTests)
        {
            return;
        }

        var searchResponse = await _searchService.SearchAsync("Apple");
        var quotes = await _searchService.SearchQuotesAsync("Apple");
        var news = await _searchService.SearchNewsAsync("Apple");

        await Assert.That(searchResponse).IsNotNull();
        await Assert.That(searchResponse?.Quotes).IsNotNull();
        await Assert.That(searchResponse?.Quotes).IsNotEmpty();
        await Assert.That(quotes).IsNotNull();
        await Assert.That(quotes).IsNotEmpty();
        await Assert.That(news).IsNotNull();
        await Assert.That(news).IsNotEmpty();
    }

    [Test]
    public async Task EarningsCalendarService_EndToEndMethods_WorkAgainstYahoo()
    {
        if (!RunLiveYahooTests)
        {
            return;
        }

        var ticker = "AAPL";

        var upcomingEarnings = await _earningsCalendarService.GetUpcomingEarningsAsync(ticker);
        var historicalEarnings = await _earningsCalendarService.GetHistoricalEarningsAsync(ticker);
        var calendar = await _earningsCalendarService.GetEarningsCalendarAsync(ticker);

        await Assert.That(upcomingEarnings).IsNotNull();
        await Assert.That(upcomingEarnings?.Ticker).IsEqualTo(ticker);
        await Assert.That(upcomingEarnings?.EarningsDates).IsNotNull();
        await Assert.That(upcomingEarnings?.EarningsDates).IsNotEmpty();

        await Assert.That(historicalEarnings).IsNotNull();
        await Assert.That(historicalEarnings?.Quarterly).IsNotNull();
        await Assert.That(historicalEarnings?.Quarterly).IsNotEmpty();

        await Assert.That(calendar).IsNotNull();
        await Assert.That(calendar?.UpcomingEarningsDates).IsNotNull();
        await Assert.That(calendar?.HistoricalEarnings).IsNotNull();
    }

    [Test]
    public async Task OptionsDataService_EndToEndMethods_WorkAgainstYahoo()
    {
        if (!RunLiveYahooTests)
        {
            return;
        }

        var expirations = await _optionsDataService.GetExpirationDatesAsync("AAPL");

        await Assert.That(expirations).IsNotNull();
        await Assert.That(expirations).IsNotEmpty();

        var expiration = expirations[0];
        var optionsChain = await _optionsDataService.GetOptionsChainAsync("AAPL", expiration);
        var optionsForExpiration = await _optionsDataService.GetOptionsForExpirationAsync("AAPL", expiration);
        var calls = await _optionsDataService.GetCallsAsync("AAPL", expiration);
        var puts = await _optionsDataService.GetPutsAsync("AAPL", expiration);

        await Assert.That(optionsChain).IsNotNull();
        await Assert.That(optionsChain.OptionChain.Result).IsNotNull();
        await Assert.That(optionsChain.OptionChain.Result).IsNotEmpty();
        await Assert.That(optionsForExpiration).IsNotNull();
        await Assert.That(calls).IsNotNull();
        await Assert.That(calls).IsNotEmpty();
        await Assert.That(puts).IsNotNull();
        await Assert.That(puts).IsNotEmpty();

        var sampleDate = new DateTime(2024, 1, 15, 12, 30, 0, DateTimeKind.Utc);
        var unixTimestamp = _optionsDataService.DateTimeToUnixTimeStamp(sampleDate);
        var roundTripDate = DateTime.SpecifyKind(_optionsDataService.UnixTimeStampToDateTime(unixTimestamp), DateTimeKind.Utc);

        await Assert.That(unixTimestamp).IsGreaterThan(0);
        await Assert.That(roundTripDate).IsEqualTo(sampleDate);
    }

    [Test]
    public async Task AnalystRecommendationsService_EndToEndMethods_WorkAgainstYahoo()
    {
        if (!RunLiveYahooTests)
        {
            return;
        }

        var ticker = "AAPL";

        var recommendations = await _analystRecommendationsService.GetRecommendationsAsync(ticker);
        var trend = await _analystRecommendationsService.GetRecommendationTrendAsync(ticker);
        var history = await _analystRecommendationsService.GetRatingChangeHistoryAsync(ticker);

        await Assert.That(recommendations).IsNotNull();
        await Assert.That(recommendations?.Ticker).IsEqualTo(ticker);
        await Assert.That(recommendations?.RecommendationTrend).IsNotNull();
        await Assert.That(recommendations?.RecommendationTrend).IsNotEmpty();
        await Assert.That(recommendations?.RatingChangeHistory).IsNotNull();
        await Assert.That(recommendations?.RatingChangeHistory).IsNotEmpty();
        await Assert.That(trend).IsNotNull();
        await Assert.That(trend).IsNotEmpty();
        await Assert.That(history).IsNotNull();
        await Assert.That(history).IsNotEmpty();
    }

    [Test]
    public async Task FinancialStatementsService_EndToEndMethods_WorkAgainstYahoo()
    {
        if (!RunLiveYahooTests)
        {
            return;
        }

        var ticker = "AAPL";

        var incomeStatement = await _financialStatementsService.GetIncomeStatementAsync(ticker);
        var balanceSheet = await _financialStatementsService.GetBalanceSheetAsync(ticker);
        var cashFlowStatement = await _financialStatementsService.GetCashFlowStatementAsync(ticker);
        var allStatements = await _financialStatementsService.GetAllStatementsAsync(ticker);
        var selectedStatements = await _financialStatementsService.GetStatementsAsync(
            ticker,
            "incomeStatementHistory",
            "balanceSheetHistory");

        await Assert.That(incomeStatement).IsNotNull();
        await Assert.That(incomeStatement?.Ticker).IsEqualTo(ticker);
        await Assert.That(balanceSheet).IsNotNull();
        await Assert.That(balanceSheet?.Ticker).IsEqualTo(ticker);
        await Assert.That(cashFlowStatement).IsNotNull();
        await Assert.That(cashFlowStatement?.Ticker).IsEqualTo(ticker);
        await Assert.That(allStatements).IsNotNull();
        await Assert.That(selectedStatements).IsNotNull();
        await Assert.That(
            selectedStatements?.IncomeStatementHistory != null ||
            selectedStatements?.BalanceSheetHistory != null).IsTrue();
    }

    [Test]
    public async Task FundDataService_EndToEndMethods_WorkAgainstYahoo()
    {
        if (!RunLiveYahooTests)
        {
            return;
        }

        var ticker = "VOO";

        var summary = await _fundDataService.GetFundSummaryAsync(ticker);
        var profile = await _fundDataService.GetFundProfileAsync(ticker);
        var topHoldings = await _fundDataService.GetTopHoldingsAsync(ticker);
        var sectorWeightings = await _fundDataService.GetSectorWeightingsAsync(ticker);
        var trailingReturns = await _fundDataService.GetTrailingReturnsAsync(ticker);
        var annualReturns = await _fundDataService.GetAnnualReturnsAsync(ticker);

        await Assert.That(summary).IsNotNull();
        await Assert.That(summary?.Ticker).IsEqualTo(ticker);
        await Assert.That(summary?.TopHoldings).IsNotNull();
        await Assert.That(summary?.TopHoldings).IsNotEmpty();
        await Assert.That(summary?.SectorWeightings).IsNotNull();
        await Assert.That(summary?.SectorWeightings).IsNotEmpty();
        await Assert.That(summary?.TrailingReturns).IsNotNull();

        await Assert.That(profile).IsNotNull();
        await Assert.That(profile?.FundFamily).IsNotNull();
        await Assert.That(profile?.Category).IsNotNull();

        await Assert.That(topHoldings).IsNotNull();
        await Assert.That(topHoldings).IsNotEmpty();
        await Assert.That(sectorWeightings).IsNotNull();
        await Assert.That(sectorWeightings).IsNotEmpty();
        await Assert.That(trailingReturns).IsNotNull();
        await Assert.That(annualReturns).IsNotNull();
        await Assert.That(annualReturns).IsNotEmpty();
    }

    [Test]
    public async Task CompanyNewsService_EndToEndMethods_WorkAgainstYahoo()
    {
        if (!RunLiveYahooTests)
        {
            return;
        }

        var ticker = "AAPL";

        var news = await _companyNewsService.GetCompanyNewsAsync(ticker, 5);
        var latest = await _companyNewsService.GetLatestCompanyNewsAsync(ticker);
        var since = await _companyNewsService.GetCompanyNewsSinceAsync(
            ticker,
            DateTimeOffset.UtcNow.AddYears(-1).ToUnixTimeSeconds(),
            5);

        await Assert.That(news).IsNotNull();
        await Assert.That(news).IsNotEmpty();
        await Assert.That(latest).IsNotNull();
        await Assert.That(latest?.Title).IsNotNull();
        await Assert.That(since).IsNotNull();
        await Assert.That(since).IsNotEmpty();
    }

    [Test]
    public async Task SectorIndustryService_EndToEndMethods_WorkAgainstYahoo()
    {
        if (!RunLiveYahooTests)
        {
            return;
        }

        var sectorInfo = await _sectorIndustryService.GetSectorInfoAsync("AAPL");

        await Assert.That(sectorInfo).IsNotNull();
        await Assert.That(sectorInfo?.Sector).IsNotNull();
        await Assert.That(sectorInfo?.Industry).IsNotNull();

        var sectorStocks = await _sectorIndustryService.GetStocksInSectorAsync(sectorInfo!.Sector!, 10);
        var industryStocks = await _sectorIndustryService.GetStocksInIndustryAsync(sectorInfo.Industry!, 10);

        await Assert.That(sectorStocks).IsNotNull();
        await Assert.That(sectorStocks).IsNotEmpty();
        await Assert.That(industryStocks).IsNotNull();
        await Assert.That(industryStocks).IsNotEmpty();
    }

    [Test]
    public async Task MarketSummaryService_EndToEndMethods_WorkAgainstYahoo()
    {
        if (!RunLiveYahooTests)
        {
            return;
        }

        var summary = await _marketSummaryService.GetMarketSummaryAsync("US");
        var status = await _marketSummaryService.GetMarketStatusAsync("US");
        var snapshot = await _marketSummaryService.GetMarketSnapshotAsync("US");

        await Assert.That(summary).IsNotNull();
        await Assert.That(summary).IsNotEmpty();
        await Assert.That(status).IsNotNull();
        await Assert.That(snapshot).IsNotNull();
        await Assert.That(snapshot?.SummaryByExchange).IsNotNull();
        await Assert.That(snapshot?.Status).IsNotNull();
    }

    [Test]
    public async Task GlobalCalendarsService_EndToEndMethods_WorkAgainstYahoo()
    {
        if (!RunLiveYahooTests)
        {
            return;
        }

        var earnings = await _globalCalendarsService.GetEarningsCalendarAsync(limit: 5, filterMostActive: false);
        var ipos = await _globalCalendarsService.GetIpoCalendarAsync(limit: 5);
        var economics = await _globalCalendarsService.GetEconomicEventsCalendarAsync(limit: 5);
        var splits = await _globalCalendarsService.GetSplitsCalendarAsync(limit: 5);

        await Assert.That(earnings).IsNotNull();
        await Assert.That(ipos).IsNotNull();
        await Assert.That(economics).IsNotNull();
        await Assert.That(splits).IsNotNull();
    }

    [Test]
    public async Task ShareholderInformationService_EndToEndMethods_WorkAgainstYahoo()
    {
        if (!RunLiveYahooTests)
        {
            return;
        }

        var ticker = "AAPL";

        var summary = await _shareholderInformationService.GetShareholderInformationAsync(ticker);
        var majorHolders = await _shareholderInformationService.GetMajorHoldersBreakdownAsync(ticker);
        var institutionalOwnership = await _shareholderInformationService.GetInstitutionalOwnershipAsync(ticker);
        var fundOwnership = await _shareholderInformationService.GetFundOwnershipAsync(ticker);
        var insiderHolders = await _shareholderInformationService.GetInsiderHoldersAsync(ticker);
        var insiderTransactions = await _shareholderInformationService.GetInsiderTransactionsAsync(ticker);

        await Assert.That(summary).IsNotNull();
        await Assert.That(summary?.Ticker).IsEqualTo(ticker);
        await Assert.That(summary?.MajorHoldersBreakdown).IsNotNull();
        await Assert.That(summary?.InstitutionalOwnership).IsNotNull();
        await Assert.That(summary?.FundOwnership).IsNotNull();
        await Assert.That(summary?.InsiderHolders).IsNotNull();
        await Assert.That(summary?.InsiderTransactions).IsNotNull();

        await Assert.That(majorHolders).IsNotNull();
        await Assert.That(institutionalOwnership).IsNotNull();
        await Assert.That(fundOwnership).IsNotNull();
        await Assert.That(insiderHolders).IsNotNull();
        await Assert.That(insiderTransactions).IsNotNull();
    }

    [Test]
    public async Task StockScreeningService_EndToEndMethods_WorkAgainstYahoo()
    {
        if (!RunLiveYahooTests)
        {
            return;
        }

        var request = new ScreenerRequest
        {
            Size = 5,
            SortField = "ticker",
            SortType = "DESC",
            QuoteType = "EQUITY",
            Query = ScreenerQuery.Eq("sector", "Technology")
        };

        var requestResult = await _stockScreeningService.ScreenAsync(request);
        var typedQueryResult = await _stockScreeningService.ScreenAsync(
            EquityQuery.Eq("sector", "Technology"),
            size: 5,
            sortField: "ticker");
        var predefinedStringResult = await _stockScreeningService.ScreenPredefinedAsync(
            PredefinedScreeners.DayGainers,
            count: 5,
            sortAsc: false);
        var predefinedEnumResult = await _stockScreeningService.ScreenPredefinedAsync(
            PredefinedScreenersCatalogItem.DayGainers,
            count: 5,
            sortAsc: false);

        await Assert.That(requestResult).IsNotNull();
        await Assert.That(requestResult?.Quotes).IsNotNull();
        await Assert.That(requestResult?.Quotes).IsNotEmpty();
        await Assert.That(typedQueryResult).IsNotNull();
        await Assert.That(typedQueryResult?.Quotes).IsNotNull();
        await Assert.That(typedQueryResult?.Quotes).IsNotEmpty();
        await Assert.That(predefinedStringResult).IsNotNull();
        await Assert.That(predefinedStringResult?.Quotes).IsNotNull();
        await Assert.That(predefinedStringResult?.Quotes).IsNotEmpty();
        await Assert.That(predefinedEnumResult).IsNotNull();
        await Assert.That(predefinedEnumResult?.Quotes).IsNotNull();
        await Assert.That(predefinedEnumResult?.Quotes).IsNotEmpty();
    }

    private static HttpClient CreateHttpClient()
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
            AllowAutoRedirect = true,
            UseCookies = true,
            CookieContainer = new CookieContainer()
        };

        return new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(90)
        };
    }
}
