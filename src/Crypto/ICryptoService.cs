using CyFinance.Models.Crypto;
using CyFinance.Models.HistoricalData;

namespace CyFinance.Services.Crypto;

/// <summary>
/// Provides crypto market data.
/// </summary>
public interface ICryptoService
{
    /// <summary>
    /// Gets the latest quote for a crypto pair.
    /// </summary>
    /// <param name="cryptoSymbol">The base symbol, for example BTC.</param>
    /// <param name="quoteCurrency">The quote currency, for example USD.</param>
    Task<CryptoQuote?> GetCryptoQuoteAsync(string cryptoSymbol, string quoteCurrency = "USD");

    /// <summary>
    /// Gets historical OHLCV data for a crypto pair.
    /// </summary>
    /// <param name="cryptoSymbol">The base symbol, for example BTC.</param>
    /// <param name="quoteCurrency">The quote currency, for example USD.</param>
    /// <param name="startDate">Inclusive start date. Defaults to one year ago.</param>
    /// <param name="endDate">Inclusive end date. Defaults to now.</param>
    Task<List<CryptoHistoricalPoint>> GetHistoricalPricesAsync(
        string cryptoSymbol,
        string quoteCurrency = "USD",
        DateTime? startDate = null,
        DateTime? endDate = null,
        ChartInterval interval = ChartInterval.OneDay);
}
