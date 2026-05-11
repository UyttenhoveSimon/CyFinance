using CyFinance.Models.Currency;
using CyFinance.Models.HistoricalData;

namespace CyFinance.Services.Currency;

/// <summary>
/// Service contract for currency/forex rate retrieval.
/// </summary>
public interface ICurrencyService
{
    /// <summary>
    /// Gets the latest exchange-rate snapshot for a currency pair.
    /// </summary>
    /// <param name="baseCurrency">The base 3-letter ISO currency code.</param>
    /// <param name="quoteCurrency">The quote 3-letter ISO currency code.</param>
    /// <returns>The current exchange-rate quote, or null when unavailable.</returns>
    Task<CurrencyQuote?> GetExchangeRateAsync(string baseCurrency, string quoteCurrency);

    /// <summary>
    /// Gets historical OHLCV rate data for a currency pair.
    /// </summary>
    /// <param name="baseCurrency">The base 3-letter ISO currency code.</param>
    /// <param name="quoteCurrency">The quote 3-letter ISO currency code.</param>
    /// <param name="startDate">Optional inclusive start date.</param>
    /// <param name="endDate">Optional inclusive end date.</param>
    /// <param name="interval">The candle interval.</param>
    /// <returns>A list of historical currency points.</returns>
    Task<List<CurrencyHistoricalPoint>> GetHistoricalRatesAsync(
        string baseCurrency,
        string quoteCurrency,
        DateTime? startDate = null,
        DateTime? endDate = null,
        ChartInterval interval = ChartInterval.OneDay);

    /// <summary>
    /// Converts an amount from one currency to another using the latest available rate.
    /// </summary>
    /// <param name="amount">The amount to convert.</param>
    /// <param name="baseCurrency">The source 3-letter ISO currency code.</param>
    /// <param name="quoteCurrency">The destination 3-letter ISO currency code.</param>
    /// <returns>The converted amount, or null when conversion data is unavailable.</returns>
    Task<double?> ConvertAsync(double amount, string baseCurrency, string quoteCurrency);
}
