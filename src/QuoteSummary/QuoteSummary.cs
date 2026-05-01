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
    public MajorHoldersBreakdown? MajorHoldersBreakdown { get; init; }
    public OwnershipContainer? InstitutionOwnership { get; init; }
    public OwnershipContainer? FundOwnership { get; init; }
    public InsiderHolders? InsiderHolders { get; init; }
    public InsiderTransactions? InsiderTransactions { get; init; }
    public TopHoldingsData? TopHoldings { get; init; }
    public FundProfileData? FundProfile { get; init; }
    public FundPerformanceData? FundPerformance { get; init; }
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

public record MajorHoldersBreakdown
{
    public YahooValue? InsidersPercentHeld { get; init; }
    public YahooValue? InstitutionsPercentHeld { get; init; }
    public YahooValue? InstitutionsFloatPercentHeld { get; init; }
    public YahooLongValue? InstitutionsCount { get; init; }
}

public record OwnershipContainer(List<OwnershipEntry> OwnershipList);

public record OwnershipEntry
{
    public YahooLongValue? MaxAge { get; init; }
    public YahooLongValue? ReportDate { get; init; }
    public string? Organization { get; init; }
    public YahooValue? PctHeld { get; init; }
    public YahooLongValue? Position { get; init; }
    public YahooLongValue? Value { get; init; }
}

public record InsiderHolders(List<InsiderHolderEntry> Holders);

public record InsiderHolderEntry
{
    public YahooLongValue? MaxAge { get; init; }
    public string? Name { get; init; }
    public string? Relation { get; init; }
    public string? Url { get; init; }
    public string? TransactionDescription { get; init; }
    public YahooLongValue? LatestTransDate { get; init; }
    public YahooLongValue? PositionDirect { get; init; }
    public YahooLongValue? PositionIndirect { get; init; }
}

public record InsiderTransactions(List<InsiderTransactionEntry> Transactions);

public record InsiderTransactionEntry
{
    public YahooLongValue? MaxAge { get; init; }
    public string? FilerName { get; init; }
    public string? FilerRelation { get; init; }
    public string? FilerUrl { get; init; }
    public string? MoneyText { get; init; }
    public string? TransactionText { get; init; }
    public YahooLongValue? StartDate { get; init; }
    public YahooLongValue? Shares { get; init; }
    public YahooValue? Value { get; init; }
    public string? Ownership { get; init; }
}

// ── Fund / ETF models ────────────────────────────────────────────────────────

public record TopHoldingsData
{
    public int? MaxAge { get; init; }
    public YahooValue? StockPosition { get; init; }
    public YahooValue? BondPosition { get; init; }
    public YahooValue? CashPosition { get; init; }
    public YahooValue? OtherPosition { get; init; }
    public YahooValue? PreferredPosition { get; init; }
    public YahooValue? ConvertiblePosition { get; init; }
    public List<FundHolding>? Holdings { get; init; }
    public EquityHoldingStats? EquityHoldings { get; init; }
    public List<Dictionary<string, YahooValue>>? SectorWeightings { get; init; }
}

public record FundHolding
{
    public string? Symbol { get; init; }
    public string? HoldingName { get; init; }
    public YahooValue? HoldingPercent { get; init; }
}

public record EquityHoldingStats
{
    public YahooValue? PriceToEarnings { get; init; }
    public YahooValue? PriceToBook { get; init; }
    public YahooValue? PriceToSales { get; init; }
    public YahooValue? PriceToCashflow { get; init; }
    public YahooValue? MedianMarketCap { get; init; }
    public YahooValue? ThreeYearEarningsGrowth { get; init; }
}

public record FundProfileData
{
    public int? MaxAge { get; init; }
    public string? StyleBoxUrl { get; init; }
    public string? Family { get; init; }
    public string? CategoryName { get; init; }
    public string? LegalType { get; init; }
    public FundFeesExpenses? FeesExpensesInvestment { get; init; }
    public FundManagementInfo? ManagementInfo { get; init; }
}

public record FundFeesExpenses
{
    public YahooValue? AnnualReportExpenseRatio { get; init; }
    public YahooValue? NetExpRatio { get; init; }
    public YahooValue? GrossExpRatio { get; init; }
    public YahooValue? FrontEndSalesLoad { get; init; }
    public YahooValue? DeferredSalesLoad { get; init; }
    public YahooValue? TwelveBOne { get; init; }
}

public record FundManagementInfo
{
    public string? ManagerName { get; init; }
    public string? ManagerBio { get; init; }
}

public record FundPerformanceData
{
    public int? MaxAge { get; init; }
    public FundPerformanceOverview? PerformanceOverview { get; init; }
    public FundAnnualReturns? AnnualTotalReturns { get; init; }
    public FundTrailingReturns? TrailingReturns { get; init; }
}

public record FundPerformanceOverview
{
    public YahooLongValue? AsOfDate { get; init; }
    public YahooValue? YtdReturnPct { get; init; }
    public YahooValue? OneYearTotalReturn { get; init; }
    public YahooValue? ThreeYearTotalReturn { get; init; }
    public YahooValue? FiveYrAvgReturnPct { get; init; }
}

public record FundAnnualReturns(List<FundAnnualReturn>? Returns);

public record FundAnnualReturn
{
    public string? Year { get; init; }
    public YahooValue? AnnualValue { get; init; }
}

public record FundTrailingReturns
{
    public YahooLongValue? AsOfDate { get; init; }
    public YahooValue? OneMonth { get; init; }
    public YahooValue? ThreeMonth { get; init; }
    public YahooValue? Ytd { get; init; }
    public YahooValue? OneYear { get; init; }
    public YahooValue? ThreeYear { get; init; }
    public YahooValue? FiveYear { get; init; }
    public YahooValue? TenYear { get; init; }
}