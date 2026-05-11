using CyFinance.Models.Crypto;
using CyFinance.Models.HistoricalData;

namespace CyFinance.Services.Crypto;

/// <summary>
/// Service contract for crypto market data.
/// </summary>
public interface ICryptoService
{
    /// <summary>
    /// Gets the latest quote snapshot for a crypto pair.
    /// </summary>
    /// <param name="cryptoSymbol">The base crypto symbol (for example, BTC).</param>
    /// <param name="quoteCurrency">The quote currency (for example, USD).</param>
    /// <returns>The current crypto quote, or null when unavailable.</returns>
    Task<CryptoQuote?> GetCryptoQuoteAsync(string cryptoSymbol, string quoteCurrency = "USD");

    /// <summary>
    /// Gets historical OHLCV data for a crypto pair.
    /// </summary>
    /// <param name="cryptoSymbol">The base crypto symbol (for example, BTC).</param>
    /// <param name="quoteCurrency">The quote currency (for example, USD).</param>
    /// <param name="startDate">Optional inclusive start date.</param>
    /// <param name="endDate">Optional inclusive end date.</param>
    /// <param name="interval">The candle interval.</param>
    /// <returns>A list of historical price points.</returns>
    Task<List<CryptoHistoricalPoint>> GetHistoricalPricesAsync(
        string cryptoSymbol,
        string quoteCurrency = "USD",
        DateTime? startDate = null,
        DateTime? endDate = null,
        ChartInterval interval = ChartInterval.OneDay);
}
