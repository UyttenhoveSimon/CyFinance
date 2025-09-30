using System.Text.Json.Serialization; // Required for JsonPropertyName

namespace YahooFinanceClient.Models.QuoteSummary;

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

// These are your base value types, which are correct
public record YahooValue(double? Raw, string? Fmt);
public record YahooLongValue(long? Raw, string? Fmt, string? LongFmt);

public record PriceData
{
    public YahooValue? RegularMarketPrice { get; init; }
    public YahooValue? RegularMarketChange { get; init; }
    public YahooValue? RegularMarketChangePercent { get; init; }
    public YahooLongValue? RegularMarketTime { get; init; }
    public YahooLongValue? PriceHint { get; init; } // Added
    public YahooValue? RegularMarketPreviousClose { get; init; } // Added
    public YahooValue? RegularMarketOpen { get; init; } // Added
    public YahooValue? RegularMarketDayHigh { get; init; } // Added
    public YahooValue? RegularMarketDayLow { get; init; } // Added
    public YahooLongValue? RegularMarketVolume { get; init; } // Added
    public YahooLongValue? AverageDailyVolume10Day { get; init; } // Added
    public YahooLongValue? AverageDailyVolume3Month { get; init; } // Added
    public YahooLongValue? MarketCap { get; init; } // Added
    public string? Currency { get; init; }
    public string? Symbol { get; init; }
    public string? ShortName { get; init; }
    public string? LongName { get; init; }
    public string? MarketState { get; init; }
}

public record SummaryDetail
{
    public YahooLongValue? PriceHint { get; init; } // Added
    public YahooValue? PreviousClose { get; init; } // Added
    public YahooValue? Open { get; init; } // Added
    public YahooValue? DayLow { get; init; } // Added
    public YahooValue? DayHigh { get; init; } // Added
    public YahooValue? RegularMarketPreviousClose { get; init; } // Added
    public YahooValue? RegularMarketOpen { get; init; } // Added
    public YahooValue? RegularMarketDayLow { get; init; } // Added
    public YahooValue? RegularMarketDayHigh { get; init; } // Added
    public YahooValue? DividendRate { get; init; } // Added
    public YahooValue? DividendYield { get; init; }
    public YahooValue? PayoutRatio { get; init; } // Added
    public YahooValue? Beta { get; init; }
    public YahooValue? TrailingPE { get; init; }
    public YahooValue? ForwardPE { get; init; }
    public YahooLongValue? Volume { get; init; }
    public YahooLongValue? RegularMarketVolume { get; init; } // Added
    public YahooLongValue? AverageVolume { get; init; }
    public YahooLongValue? AverageVolume10days { get; init; } // Added
    public YahooLongValue? AverageDailyVolume10Day { get; init; } // Added
    public YahooLongValue? MarketCap { get; init; }
    public YahooValue? FiftyTwoWeekLow { get; init; }
    public YahooValue? FiftyTwoWeekHigh { get; init; }
    public YahooValue? FiftyDayAverage { get; init; } // Added
    public YahooValue? TwoHundredDayAverage { get; init; } // Added
    public YahooValue? TrailingAnnualDividendYield { get; init; } // Added
    public YahooValue? TrailingAnnualDividendRate { get; init; } // Added
}

public record AssetProfile
{
    public string? Address1 { get; init; } // Added
    public string? City { get; init; }
    public string? State { get; init; }
    public string? Zip { get; init; } // Added
    public string? Country { get; init; }
    public string? Phone { get; init; } // Added
    public string? Website { get; init; }
    public string? Industry { get; init; }
    public string? Sector { get; init; }
    public string? LongBusinessSummary { get; init; }
    public int? FullTimeEmployees { get; init; }
    public List<CompanyOfficer>? CompanyOfficers { get; init; } // Added complex type
}

// Added new records to handle the "companyOfficers" array
public record CompanyOfficer(string? Name, int? Age, string? Title, int? YearBorn, YahooLongValue? TotalPay);

public record FinancialData
{
    public YahooValue? CurrentPrice { get; init; } // Added
    public YahooValue? TargetHighPrice { get; init; } // Added
    public YahooValue? TargetLowPrice { get; init; } // Added
    public YahooValue? TargetMeanPrice { get; init; } // Added
    public YahooValue? TargetMedianPrice { get; init; } // Added
    public YahooValue? RecommendationMean { get; init; } // Added
    public string? RecommendationKey { get; init; } // Added
    public YahooLongValue? NumberOfAnalystOpinions { get; init; } // Added
    public YahooLongValue? TotalCash { get; init; }
    public YahooValue? TotalCashPerShare { get; init; } // Added
    public YahooLongValue? Ebitda { get; init; } // Added
    public YahooLongValue? TotalDebt { get; init; }
    public YahooValue? QuickRatio { get; init; } // Added
    public YahooValue? CurrentRatio { get; init; } // Added
    public YahooLongValue? TotalRevenue { get; init; }
    public YahooValue? DebtToEquity { get; init; } // Added
    public YahooValue? RevenuePerShare { get; init; } // Added
    public YahooValue? ReturnOnAssets { get; init; } // Added
    public YahooValue? ReturnOnEquity { get; init; } // Added
    public YahooLongValue? GrossProfits { get; init; } // Added
    public YahooLongValue? FreeCashflow { get; init; }
    public YahooLongValue? OperatingCashflow { get; init; }
    public YahooValue? EarningsGrowth { get; init; } // Added
    public YahooValue? RevenueGrowth { get; init; }
    public YahooValue? GrossMargins { get; init; }
    public YahooValue? EbitdaMargins { get; init; } // Added
    public YahooValue? OperatingMargins { get; init; }
    public YahooValue? ProfitMargins { get; init; }
    public string? FinancialCurrency { get; init; } // Added
}

public record KeyStatistics
{
    public YahooValue? EnterpriseToRevenue { get; init; }
    public YahooValue? EnterpriseToEbitda { get; init; }

    // This attribute is the critical fix for property names that are invalid in C#
    [JsonPropertyName("52WeekChange")]
    public YahooValue? _52WeekChange { get; init; }

    public YahooLongValue? SharesOutstanding { get; init; }
    public YahooValue? BookValue { get; init; }
    public YahooValue? PriceToBook { get; init; }
    public YahooLongValue? LastFiscalYearEnd { get; init; }
}

public record IncomeStatementHistory(List<FinancialStatement> IncomeStatementStatements); // Corrected property name
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