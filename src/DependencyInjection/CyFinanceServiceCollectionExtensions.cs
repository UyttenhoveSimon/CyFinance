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
using Microsoft.Extensions.DependencyInjection;

namespace CyFinance.DependencyInjection;

public static class CyFinanceServiceCollectionExtensions
{
    public static IServiceCollection AddCyFinance(this IServiceCollection services)
    {
        services.AddHttpClient<IQuoteSummaryService, QuoteSummaryService>();
        services.AddHttpClient<IHistoricalDataService, HistoricalDataService>();
        services.AddHttpClient<IOptionsDataService, OptionsDataService>();
        services.AddHttpClient<ISearchService, SearchService>();
        services.AddHttpClient<IStockScreeningService, StockScreeningService>();
        services.AddHttpClient<IMarketSummaryService, MarketSummaryService>();
        services.AddHttpClient<IGlobalCalendarsService, GlobalCalendarsService>();
        services.AddHttpClient<ICompanyNewsService, CompanyNewsService>();
        services.AddHttpClient<ICurrencyService, CurrencyService>();
        services.AddHttpClient<ICryptoService, CryptoService>();

        services.AddTransient<IEarningsCalendarService, EarningsCalendarService>();
        services.AddTransient<IFinancialStatementsService, FinancialStatementsService>();
        services.AddTransient<IAnalystRecommendationsService, AnalystRecommendationsService>();
        services.AddTransient<IShareholderInformationService, ShareholderInformationService>();
        services.AddTransient<IFundDataService, FundDataService>();
        services.AddTransient<ISectorIndustryService, SectorIndustryService>();

        services.AddTransient<CyFinanceClient>();

        return services;
    }
}
