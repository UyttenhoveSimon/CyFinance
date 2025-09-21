namespace YahooFinanceClient.Models.QuoteSummary;

public record QuoteResponse(QuoteSummary QuoteSummary);

public record QuoteSummary(
    List<QuoteResult> Result,
    object? Error);

public record QuoteResult
{
    public PriceData? Price { get; init; }
    public SummaryDetail? SummaryDetail { get; init; }
    public AssetProfile? AssetProfile { get; init; }
    public FinancialData? FinancialData { get; init; }
    public KeyStatistics? DefaultKeyStatistics { get; init; }
    public IncomeStatementHistory? IncomeStatementHistory { get; init; }
    public BalanceSheetHistory? BalanceSheetHistory { get; init; }
    public CashflowStatementHistory? CashflowStatementHistory { get; init; }
    public Earnings? Earnings { get; init; }
    public CalendarEvents? CalendarEvents { get; init; }
}

public record YahooValue(double? Raw, string? Fmt);
public record YahooLongValue(long? Raw, string? Fmt);

public record PriceData
{
    public YahooValue? RegularMarketPrice { get; init; }
    public YahooValue? RegularMarketChange { get; init; }
    public YahooValue? RegularMarketChangePercent { get; init; }
    public YahooLongValue? RegularMarketTime { get; init; }
    public string? Currency { get; init; }
    public string? Symbol { get; init; }
    public string? ShortName { get; init; }
    public string? LongName { get; init; }
    public string? MarketState { get; init; }
}

public record SummaryDetail
{
    public YahooLongValue? MarketCap { get; init; }
    public YahooLongValue? EnterpriseValue { get; init; }
    public YahooValue? TrailingPE { get; init; }
    public YahooValue? ForwardPE { get; init; }
    public YahooValue? Beta { get; init; }
    public YahooValue? DividendYield { get; init; }
    public YahooValue? FiftyTwoWeekLow { get; init; }
    public YahooValue? FiftyTwoWeekHigh { get; init; }
    public YahooLongValue? Volume { get; init; }
    public YahooLongValue? AverageVolume { get; init; }
}

public record AssetProfile
{
    public string? LongBusinessSummary { get; init; }
    public string? Industry { get; init; }
    public string? Sector { get; init; }
    public string? Website { get; init; }
    public int? FullTimeEmployees { get; init; }
    public string? Country { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
}

public record FinancialData
{
    public YahooLongValue? TotalRevenue { get; init; }
    public YahooLongValue? TotalCash { get; init; }
    public YahooLongValue? TotalDebt { get; init; }
    public YahooLongValue? FreeCashflow { get; init; }
    public YahooLongValue? OperatingCashflow { get; init; }
    public YahooValue? RevenueGrowth { get; init; }
    public YahooValue? GrossMargins { get; init; }
    public YahooValue? OperatingMargins { get; init; }
    public YahooValue? ProfitMargins { get; init; }
}

public record KeyStatistics
{
    public YahooValue? EnterpriseToRevenue { get; init; }
    public YahooValue? EnterpriseToEbitda { get; init; }
    public YahooValue? _52WeekChange { get; init; }   // JSON is "52WeekChange"
    public YahooLongValue? SharesOutstanding { get; init; }
    public YahooValue? BookValue { get; init; }
    public YahooValue? PriceToBook { get; init; }
    public YahooLongValue? LastFiscalYearEnd { get; init; }
}

public record IncomeStatementHistory(List<FinancialStatement> IncomeStatements);
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
