using CyFinance.Models.FinancialStatements;

namespace CyFinance.Services.FinancialStatements;

/// <summary>
/// Provides income statements, balance sheets and cash flow statements.
/// </summary>
public interface IFinancialStatementsService
{
    /// <summary>
    /// Gets the income statement, annual and quarterly.
    /// </summary>
    Task<IncomeStatement?> GetIncomeStatementAsync(string ticker);

    /// <summary>
    /// Gets the balance sheet, annual and quarterly.
    /// </summary>
    Task<BalanceSheet?> GetBalanceSheetAsync(string ticker);

    /// <summary>
    /// Gets the cash flow statement, annual and quarterly.
    /// </summary>
    Task<CashFlowStatement?> GetCashFlowStatementAsync(string ticker);

    /// <summary>
    /// Gets all three statements in a single call.
    /// </summary>
    Task<FinancialStatementsResponse?> GetAllStatementsAsync(string ticker);

    /// <summary>
    /// Gets the named quote summary modules, for callers who want a narrower request than
    /// <see cref="GetAllStatementsAsync" />.
    /// </summary>
    /// <param name="modules">
    /// Yahoo module names, such as <c>incomeStatementHistory</c> or
    /// <c>balanceSheetHistoryQuarterly</c>.
    /// </param>
    Task<FinancialStatementsResponse?> GetStatementsAsync(string ticker, params string[] modules);
}
