using CyFinance.Models.FinancialStatements;

namespace CyFinance.Services.FinancialStatements
{
    /// <summary>
    /// Interface for Yahoo Finance Financial Statements API
    /// Provides access to income statements, balance sheets, and cash flow statements
    /// </summary>
    public interface IFinancialStatementsService
    {
        /// <summary>
        /// Get income statement (annual and quarterly) for a ticker
        /// </summary>
        /// <param name="ticker">The stock ticker symbol</param>
        /// <returns>IncomeStatement with annual and quarterly data</returns>
        Task<IncomeStatement?> GetIncomeStatementAsync(string ticker);

        /// <summary>
        /// Get balance sheet (annual and quarterly) for a ticker
        /// </summary>
        /// <param name="ticker">The stock ticker symbol</param>
        /// <returns>BalanceSheet with annual and quarterly data</returns>
        Task<BalanceSheet?> GetBalanceSheetAsync(string ticker);

        /// <summary>
        /// Get cash flow statement (annual and quarterly) for a ticker
        /// </summary>
        /// <param name="ticker">The stock ticker symbol</param>
        /// <returns>CashFlowStatement with annual and quarterly data</returns>
        Task<CashFlowStatement?> GetCashFlowStatementAsync(string ticker);

        /// <summary>
        /// Get all financial statements (income, balance sheet, cash flow) for a ticker
        /// </summary>
        /// <param name="ticker">The stock ticker symbol</param>
        /// <returns>FinancialStatementsResponse containing all statement types</returns>
        Task<FinancialStatementsResponse?> GetAllStatementsAsync(string ticker);

        /// <summary>
        /// Get specific financial statements by module names
        /// </summary>
        /// <param name="ticker">The stock ticker symbol</param>
        /// <param name="modules">Module names like 'incomeStatementHistory', 'balanceSheetHistoryQuarterly', etc.</param>
        /// <returns>FinancialStatementsResponse with requested modules</returns>
        Task<FinancialStatementsResponse?> GetStatementsAsync(string ticker, params string[] modules);
    }
}
