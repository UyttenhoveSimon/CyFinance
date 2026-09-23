using CyFinance.Models.Currency;
using CyFinance.Models.HistoricalData;

namespace CyFinance.Services.Currency;

/// <summary>
/// Provides foreign exchange rates.
/// </summary>
public interface ICurrencyService
{
    /// <summary>
    /// Gets the latest rate for a currency pair.
    /// </summary>
    /// <param name="baseCurrency">A 3-letter ISO code, for example EUR.</param>
    /// <param name="quoteCurrency">A 3-letter ISO code, for example USD.</param>
    Task<CurrencyQuote?> GetExchangeRateAsync(string baseCurrency, string quoteCurrency);

    /// <summary>
    /// Gets historical OHLCV rates for a currency pair.
    /// </summary>
    /// <param name="baseCurrency">A 3-letter ISO code, for example EUR.</param>
    /// <param name="quoteCurrency">A 3-letter ISO code, for example USD.</param>
    /// <param name="startDate">Inclusive start date. Defaults to one year ago.</param>
    /// <param name="endDate">Inclusive end date. Defaults to now.</param>
    Task<List<CurrencyHistoricalPoint>> GetHistoricalRatesAsync(
        string baseCurrency,
        string quoteCurrency,
        DateTime? startDate = null,
        DateTime? endDate = null,
        ChartInterval interval = ChartInterval.OneDay);

    /// <summary>
    /// Converts an amount at the latest available rate.
    /// </summary>
    /// <param name="baseCurrency">A 3-letter ISO code, for example EUR.</param>
    /// <param name="quoteCurrency">A 3-letter ISO code, for example USD.</param>
    Task<double?> ConvertAsync(double amount, string baseCurrency, string quoteCurrency);
}
