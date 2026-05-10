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

namespace CyFinance;

/// <summary>
/// Facade that exposes CyFinance services from a single entry point.
/// </summary>
public sealed class CyFinanceClient
{
    public CyFinanceClient(
        IQuoteSummaryService quoteSummary,
        IHistoricalDataService historicalData,
        IOptionsDataService optionsData,
        ISearchService search,
        IStockScreeningService stockScreening,
        IMarketSummaryService market,
        IGlobalCalendarsService calendars,
        IEarningsCalendarService earningsCalendar,
        IFinancialStatementsService financialStatements,
        IAnalystRecommendationsService analystRecommendations,
        IShareholderInformationService shareholderInformation,
        ICompanyNewsService companyNews,
        IFundDataService fundData,
        ISectorIndustryService sectorIndustry,
        ICurrencyService currency,
        ICryptoService crypto)
    {
        QuoteSummary = quoteSummary;
        HistoricalData = historicalData;
        OptionsData = optionsData;
        Search = search;
        StockScreening = stockScreening;
        Market = market;
        Calendars = calendars;
        EarningsCalendar = earningsCalendar;
        FinancialStatements = financialStatements;
        AnalystRecommendations = analystRecommendations;
        ShareholderInformation = shareholderInformation;
        CompanyNews = companyNews;
        FundData = fundData;
        SectorIndustry = sectorIndustry;
        Currency = currency;
        Crypto = crypto;
    }

    public IQuoteSummaryService QuoteSummary { get; }

    public IHistoricalDataService HistoricalData { get; }

    public IOptionsDataService OptionsData { get; }

    public ISearchService Search { get; }

    public IStockScreeningService StockScreening { get; }

    public IMarketSummaryService Market { get; }

    public IGlobalCalendarsService Calendars { get; }

    public IEarningsCalendarService EarningsCalendar { get; }

    public IFinancialStatementsService FinancialStatements { get; }

    public IAnalystRecommendationsService AnalystRecommendations { get; }

    public IShareholderInformationService ShareholderInformation { get; }

    public ICompanyNewsService CompanyNews { get; }

    public IFundDataService FundData { get; }

    public ISectorIndustryService SectorIndustry { get; }

    public ICurrencyService Currency { get; }

    public ICryptoService Crypto { get; }
}
