using System.Text.Json.Serialization;
using CyFinance.Models.QuoteSummary;

namespace CyFinance.Models.FinancialStatements
{
    /// <summary>
    /// Financial statements response containing income, balance sheet, and cash flow data
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

    /// <summary>
    /// Income Statement container
    /// </summary>
    public class IncomeStatement
    {
        public string? Ticker { get; set; }
        public List<FinancialStatement>? AnnualStatements { get; set; }
        public List<FinancialStatement>? QuarterlyStatements { get; set; }
    }

    /// <summary>
    /// Balance Sheet container
    /// </summary>
    public class BalanceSheet
    {
        public string? Ticker { get; set; }
        public List<FinancialStatement>? AnnualStatements { get; set; }
        public List<FinancialStatement>? QuarterlyStatements { get; set; }
    }

    /// <summary>
    /// Cash Flow Statement container
    /// </summary>
    public class CashFlowStatement
    {
        public string? Ticker { get; set; }
        public List<FinancialStatement>? AnnualStatements { get; set; }
        public List<FinancialStatement>? QuarterlyStatements { get; set; }
    }
}
