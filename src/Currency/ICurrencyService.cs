using CyFinance.Models.Currency;
using CyFinance.Models.HistoricalData;

namespace CyFinance.Services.Currency;

/// <summary>
/// Service contract for currency/forex rate retrieval.
/// </summary>
public interface ICurrencyService
{
    Task<CurrencyQuote?> GetExchangeRateAsync(string baseCurrency, string quoteCurrency);

    Task<List<CurrencyHistoricalPoint>> GetHistoricalRatesAsync(
        string baseCurrency,
        string quoteCurrency,
        DateTime? startDate = null,
        DateTime? endDate = null,
        ChartInterval interval = ChartInterval.OneDay);

    Task<double?> ConvertAsync(double amount, string baseCurrency, string quoteCurrency);
}