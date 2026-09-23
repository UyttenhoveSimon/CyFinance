using System.Text.Json.Serialization;
using CyFinance.Models.QuoteSummary;

namespace CyFinance.Models.FinancialStatements;

/// <summary>
/// The raw quote summary modules behind the three statements. A module is null when it was
/// not requested, and also when Yahoo has no data for it.
/// </summary>
public class FinancialStatementsResponse
{
    public IncomeStatementHistory? IncomeStatementHistory { get; set; }
    public IncomeStatementHistory? IncomeStatementHistoryQuarterly { get; set; }
    public BalanceSheetHistory? BalanceSheetHistory { get; set; }
    public BalanceSheetHistory? BalanceSheetHistoryQuarterly { get; set; }
    public CashflowStatementHistory? CashflowStatementHistory { get; set; }
    public CashflowStatementHistory? CashflowStatementHistoryQuarterly { get; set; }
}

public class IncomeStatement
{
    public string? Ticker { get; set; }
    public List<FinancialStatement>? AnnualStatements { get; set; }
    public List<FinancialStatement>? QuarterlyStatements { get; set; }
}

public class BalanceSheet
{
    public string? Ticker { get; set; }
    public List<FinancialStatement>? AnnualStatements { get; set; }
    public List<FinancialStatement>? QuarterlyStatements { get; set; }
}

public class CashFlowStatement
{
    public string? Ticker { get; set; }
    public List<FinancialStatement>? AnnualStatements { get; set; }
    public List<FinancialStatement>? QuarterlyStatements { get; set; }
}
