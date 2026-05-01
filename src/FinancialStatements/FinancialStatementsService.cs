using System.Text.Json;
using CyFinance.Models.FinancialStatements;
using CyFinance.Models.QuoteSummary;
using CyFinance.Services.QuoteSummary;

namespace CyFinance.Services.FinancialStatements
{
    /// <summary>
    /// Financial Statements Service for Yahoo Finance
    /// Retrieves income statements, balance sheets, and cash flow statements
    /// </summary>
    public class FinancialStatementsService : IFinancialStatementsService
    {
        private readonly IQuoteSummaryService _quoteSummaryService;

        public FinancialStatementsService(IQuoteSummaryService quoteSummaryService)
        {
            _quoteSummaryService = quoteSummaryService ?? throw new ArgumentNullException(nameof(quoteSummaryService));
        }

        /// <summary>
        /// Get income statement for a ticker
        /// </summary>
        public async Task<IncomeStatement?> GetIncomeStatementAsync(string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker))
                throw new ArgumentException("Ticker cannot be empty", nameof(ticker));

            try
            {
                var response = await _quoteSummaryService.GetQuoteSummaryAsync(ticker,
                    "incomeStatementHistory", "incomeStatementHistoryQuarterly");

                if (response?.QuoteSummary?.Result == null || response.QuoteSummary.Result.Count == 0)
                    return null;

                var result = response.QuoteSummary.Result[0];
                
                return new IncomeStatement
                {
                    Ticker = ticker,
                    AnnualStatements = result.IncomeStatementHistory?.IncomeStatementStatements,
                    QuarterlyStatements = result.IncomeStatementHistory?.IncomeStatementStatements // Note: API doesn't separate annual/quarterly in records
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get income statement for {ticker}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get balance sheet for a ticker
        /// </summary>
        public async Task<BalanceSheet?> GetBalanceSheetAsync(string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker))
                throw new ArgumentException("Ticker cannot be empty", nameof(ticker));

            try
            {
                var response = await _quoteSummaryService.GetQuoteSummaryAsync(ticker,
                    "balanceSheetHistory", "balanceSheetHistoryQuarterly");

                if (response?.QuoteSummary?.Result == null || response.QuoteSummary.Result.Count == 0)
                    return null;

                var result = response.QuoteSummary.Result[0];
                
                return new BalanceSheet
                {
                    Ticker = ticker,
                    AnnualStatements = result.BalanceSheetHistory?.BalanceSheetStatements,
                    QuarterlyStatements = result.BalanceSheetHistory?.BalanceSheetStatements
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get balance sheet for {ticker}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get cash flow statement for a ticker
        /// </summary>
        public async Task<CashFlowStatement?> GetCashFlowStatementAsync(string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker))
                throw new ArgumentException("Ticker cannot be empty", nameof(ticker));

            try
            {
                var response = await _quoteSummaryService.GetQuoteSummaryAsync(ticker,
                    "cashflowStatementHistory", "cashflowStatementHistoryQuarterly");

                if (response?.QuoteSummary?.Result == null || response.QuoteSummary.Result.Count == 0)
                    return null;

                var result = response.QuoteSummary.Result[0];
                
                return new CashFlowStatement
                {
                    Ticker = ticker,
                    AnnualStatements = result.CashflowStatementHistory?.CashflowStatements,
                    QuarterlyStatements = result.CashflowStatementHistory?.CashflowStatements
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get cash flow statement for {ticker}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get all financial statements for a ticker
        /// </summary>
        public async Task<FinancialStatementsResponse?> GetAllStatementsAsync(string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker))
                throw new ArgumentException("Ticker cannot be empty", nameof(ticker));

            return await GetStatementsAsync(ticker,
                "incomeStatementHistory",
                "incomeStatementHistoryQuarterly",
                "balanceSheetHistory",
                "balanceSheetHistoryQuarterly",
                "cashflowStatementHistory",
                "cashflowStatementHistoryQuarterly");
        }

        /// <summary>
        /// Get specific financial statements by module names
        /// </summary>
        public async Task<FinancialStatementsResponse?> GetStatementsAsync(string ticker, params string[] modules)
        {
            if (string.IsNullOrWhiteSpace(ticker))
                throw new ArgumentException("Ticker cannot be empty", nameof(ticker));

            if (modules == null || modules.Length == 0)
                throw new ArgumentException("At least one module must be specified", nameof(modules));

            try
            {
                var response = await _quoteSummaryService.GetQuoteSummaryAsync(ticker, modules);

                if (response?.QuoteSummary?.Result == null || response.QuoteSummary.Result.Count == 0)
                    return null;

                var result = response.QuoteSummary.Result[0];
                
                return new FinancialStatementsResponse
                {
                    IncomeStatementHistory = result.IncomeStatementHistory,
                    BalanceSheetHistory = result.BalanceSheetHistory,
                    CashflowStatementHistory = result.CashflowStatementHistory
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get financial statements for {ticker}: {ex.Message}", ex);
            }
        }
    }
}
