using System.Text.Json.Serialization; // Required for JsonPropertyName

namespace CyFinance.Models.QuoteSummary;

public record QuoteResponse(QuoteSummary QuoteSummary);

public record QuoteSummary(
    List<QuoteResult> Result,
    object? Error);

public record QuoteResult
{
    public AssetProfile? AssetProfile { get; init; }
    public SummaryDetail? SummaryDetail { get; init; }
    public PriceData? Price { get; init; }
    public FinancialData? FinancialData { get; init; }
    public KeyStatistics? DefaultKeyStatistics { get; init; }
    public IncomeStatementHistory? IncomeStatementHistory { get; init; }
    public BalanceSheetHistory? BalanceSheetHistory { get; init; }
    public CashflowStatementHistory? CashflowStatementHistory { get; init; }
    public Earnings? Earnings { get; init; }
    public CalendarEvents? CalendarEvents { get; init; }
}

// Base value types
public record YahooValue(double? Raw, string? Fmt);
public record YahooLongValue(long? Raw, string? Fmt, string? LongFmt);

public record PriceData
{
    public int? MaxAge { get; init; } // ADDED
    public YahooValue? PreMarketChangePercent { get; init; } // ADDED
    public YahooValue? PreMarketChange { get; init; } // ADDED
    public long? PreMarketTime { get; init; } // ADDED
    public YahooValue? PreMarketPrice { get; init; } // ADDED
    public string? PreMarketSource { get; init; } // ADDED
    public YahooValue? RegularMarketPrice { get; init; }
    public YahooValue? RegularMarketChange { get; init; }
    public YahooValue? RegularMarketChangePercent { get; init; }
    public long? RegularMarketTime { get; init; } // FIXED: Changed type from YahooLongValue to long?
    public YahooLongValue? PriceHint { get; init; }
    public YahooValue? RegularMarketPreviousClose { get; init; }
    public YahooValue? RegularMarketOpen { get; init; }
    public YahooValue? RegularMarketDayHigh { get; init; }
    public YahooValue? RegularMarketDayLow { get; init; }
    public YahooLongValue? RegularMarketVolume { get; init; }
    public YahooLongValue? AverageDailyVolume10Day { get; init; }
    public YahooLongValue? AverageDailyVolume3Month { get; init; }
    public YahooLongValue? MarketCap { get; init; }
    public string? RegularMarketSource { get; init; } // ADDED
    public string? Exchange { get; init; } // ADDED
    public string? ExchangeName { get; init; } // ADDED
    public int? ExchangeDataDelayedBy { get; init; } // ADDED
    public string? MarketState { get; init; }
    public string? QuoteType { get; init; } // ADDED
    public string? Currency { get; init; }
    public string? CurrencySymbol { get; init; } // ADDED
    public string? Symbol { get; init; }
    public object? UnderlyingSymbol { get; init; } // ADDED
    public string? ShortName { get; init; }
    public string? LongName { get; init; }
    public string? QuoteSourceName { get; init; } // ADDED
}

public record SummaryDetail
{
    public int? MaxAge { get; init; } // ADDED
    public YahooLongValue? PriceHint { get; init; }
    public YahooValue? PreviousClose { get; init; }
    public YahooValue? Open { get; init; }
    public YahooValue? DayLow { get; init; }
    public YahooValue? DayHigh { get; init; }
    public YahooValue? RegularMarketPreviousClose { get; init; }
    public YahooValue? RegularMarketOpen { get; init; }
    public YahooValue? RegularMarketDayLow { get; init; }
    public YahooValue? RegularMarketDayHigh { get; init; }
    public YahooValue? DividendRate { get; init; }
    public YahooValue? DividendYield { get; init; }
    public YahooLongValue? ExDividendDate { get; init; } // ADDED
    public YahooValue? PayoutRatio { get; init; }
    public YahooValue? FiveYearAvgDividendYield { get; init; } // ADDED
    public YahooValue? Beta { get; init; }
    public YahooValue? TrailingPE { get; init; }
    public YahooValue? ForwardPE { get; init; }
    public YahooLongValue? Volume { get; init; }
    public YahooLongValue? RegularMarketVolume { get; init; }
    public YahooLongValue? AverageVolume { get; init; }
    public YahooLongValue? AverageVolume10days { get; init; }
    public YahooLongValue? AverageDailyVolume10Day { get; init; }
    public YahooValue? Bid { get; init; } // ADDED
    public YahooValue? Ask { get; init; } // ADDED
    public YahooLongValue? BidSize { get; init; } // ADDED
    public YahooLongValue? AskSize { get; init; } // ADDED
    public YahooLongValue? MarketCap { get; init; }
    public YahooValue? FiftyTwoWeekLow { get; init; }
    public YahooValue? FiftyTwoWeekHigh { get; init; }
    public YahooValue? AllTimeHigh { get; init; } // ADDED
    public YahooValue? AllTimeLow { get; init; } // ADDED
    public YahooValue? PriceToSalesTrailing12Months { get; init; } // ADDED
    public YahooValue? FiftyDayAverage { get; init; }
    public YahooValue? TwoHundredDayAverage { get; init; }
    public YahooValue? TrailingAnnualDividendYield { get; init; }
    public YahooValue? TrailingAnnualDividendRate { get; init; }
    public string? Currency { get; init; } // ADDED
    public bool? Tradeable { get; init; } // ADDED
}

public record AssetProfile
{
    public int? MaxAge { get; init; } // ADDED
    public string? Address1 { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? Zip { get; init; }
    public string? Country { get; init; }
    public string? Phone { get; init; }
    public string? Website { get; init; }
    public string? Industry { get; init; }
    public string? IndustryKey { get; init; } // ADDED
    public string? IndustryDisp { get; init; } // ADDED
    public string? Sector { get; init; }
    public string? SectorKey { get; init; } // ADDED
    public string? SectorDisp { get; init; } // ADDED
    public string? LongBusinessSummary { get; init; }
    public int? FullTimeEmployees { get; init; }
    public List<CompanyOfficer>? CompanyOfficers { get; init; }
    public int? AuditRisk { get; init; } // ADDED
    public int? BoardRisk { get; init; } // ADDED
    public int? CompensationRisk { get; init; } // ADDED
    public int? ShareHolderRightsRisk { get; init; } // ADDED
    public int? OverallRisk { get; init; } // ADDED
    public long? GovernanceEpochDate { get; init; } // ADDED
    public long? CompensationAsOfEpochDate { get; init; } // ADDED
    public string? IrWebsite { get; init; } // ADDED
}

// Added new properties to the CompanyOfficer record
public record CompanyOfficer(
    string? Name,
    int? Age,
    string? Title,
    int? YearBorn,
    int? FiscalYear, // ADDED
    YahooLongValue? TotalPay,
    YahooLongValue? ExercisedValue, // ADDED
    YahooLongValue? UnexercisedValue // ADDED
);

public record FinancialData
{
    public int? MaxAge { get; init; } // ADDED
    public YahooValue? CurrentPrice { get; init; }
    public YahooValue? TargetHighPrice { get; init; }
    public YahooValue? TargetLowPrice { get; init; }
    public YahooValue? TargetMeanPrice { get; init; }
    public YahooValue? TargetMedianPrice { get; init; }
    public YahooValue? RecommendationMean { get; init; }
    public string? RecommendationKey { get; init; }
    public YahooLongValue? NumberOfAnalystOpinions { get; init; }
    public YahooLongValue? TotalCash { get; init; }
    public YahooValue? TotalCashPerShare { get; init; }
    public YahooLongValue? Ebitda { get; init; }
    public YahooLongValue? TotalDebt { get; init; }
    public YahooValue? QuickRatio { get; init; }
    public YahooValue? CurrentRatio { get; init; }
    public YahooLongValue? TotalRevenue { get; init; }
    public YahooValue? DebtToEquity { get; init; }
    public YahooValue? RevenuePerShare { get; init; }
    public YahooValue? ReturnOnAssets { get; init; }
    public YahooValue? ReturnOnEquity { get; init; }
    public YahooLongValue? GrossProfits { get; init; }
    public YahooLongValue? FreeCashflow { get; init; }
    public YahooLongValue? OperatingCashflow { get; init; }
    public YahooValue? EarningsGrowth { get; init; }
    public YahooValue? RevenueGrowth { get; init; }
    public YahooValue? GrossMargins { get; init; }
    public YahooValue? EbitdaMargins { get; init; }
    public YahooValue? OperatingMargins { get; init; }
    public YahooValue? ProfitMargins { get; init; }
    public string? FinancialCurrency { get; init; }
}

// Unchanged records below
public record KeyStatistics
{
    public YahooValue? EnterpriseToRevenue { get; init; }
    public YahooValue? EnterpriseToEbitda { get; init; }

    [JsonPropertyName("52WeekChange")]
    public YahooValue? _52WeekChange { get; init; }

    public YahooLongValue? SharesOutstanding { get; init; }
    public YahooValue? BookValue { get; init; }
    public YahooValue? PriceToBook { get; init; }
    public YahooLongValue? LastFiscalYearEnd { get; init; }
}

public record IncomeStatementHistory(List<FinancialStatement> IncomeStatementStatements);
public record BalanceSheetHistory(List<FinancialStatement> BalanceSheetStatements);
public record CashflowStatementHistory(List<FinancialStatement> CashflowStatements);

public record FinancialStatement
{
    public YahooLongValue? EndDate { get; init; }
    public YahooLongValue? TotalRevenue { get; init; }
    public YahooLongValue? NetIncome { get; init; }
    public YahooLongValue? TotalCash { get; init; }
}

public record Earnings(EarningsChart EarningsChart);
public record EarningsChart(List<QuarterlyEarnings> Quarterly);
public record QuarterlyEarnings(string? Date, YahooValue? Actual, YahooValue? Estimate);

public record CalendarEvents
{
    public EarningsCalendar? Earnings { get; init; }
    public YahooLongValue? ExDividendDate { get; init; }
    public YahooLongValue? DividendDate { get; init; }
}

public record EarningsCalendar(List<YahooLongValue> EarningsDate);