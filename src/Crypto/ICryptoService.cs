using CyFinance.Models.Crypto;
using CyFinance.Models.HistoricalData;

namespace CyFinance.Services.Crypto;

/// <summary>
/// Service contract for crypto market data.
/// </summary>
public interface ICryptoService
{
    Task<CryptoQuote?> GetCryptoQuoteAsync(string cryptoSymbol, string quoteCurrency = "USD");

    Task<List<CryptoHistoricalPoint>> GetHistoricalPricesAsync(
        string cryptoSymbol,
        string quoteCurrency = "USD",
        DateTime? startDate = null,
        DateTime? endDate = null,
        ChartInterval interval = ChartInterval.OneDay);
}