
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CyFinance.Services.OptionsData;

public interface IOptionsDataService
{
    /// <summary>
    /// Gets options chain data for a ticker and optional expiration date.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <param name="date">Optional expiration date as Unix seconds.</param>
    /// <returns>The options chain response.</returns>
    Task<OptionsDataResponse> GetOptionsChainAsync(string ticker, long? date = null);

    /// <summary>
    /// Gets available expiration dates for a ticker.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <returns>A list of expiration dates as Unix seconds.</returns>
    Task<List<long>> GetExpirationDatesAsync(string ticker);

    /// <summary>
    /// Gets option contracts for a specific expiration date.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <param name="expirationDate">The expiration date as Unix seconds.</param>
    /// <returns>The options chain data for that expiration, or null if not found.</returns>
    Task<OptionsChainData?> GetOptionsForExpirationAsync(string ticker, long expirationDate);

    /// <summary>
    /// Gets call contracts for a ticker and optional expiration date.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <param name="date">Optional expiration date as Unix seconds.</param>
    /// <returns>A list of call contracts.</returns>
    Task<List<OptionContract>> GetCallsAsync(string ticker, long? date = null);

    /// <summary>
    /// Gets put contracts for a ticker and optional expiration date.
    /// </summary>
    /// <param name="ticker">The ticker symbol.</param>
    /// <param name="date">Optional expiration date as Unix seconds.</param>
    /// <returns>A list of put contracts.</returns>
    Task<List<OptionContract>> GetPutsAsync(string ticker, long? date = null);

    /// <summary>
    /// Converts a DateTime value to Unix seconds.
    /// </summary>
    /// <param name="dateTime">The date and time value.</param>
    /// <returns>The Unix timestamp in seconds.</returns>
    long DateTimeToUnixTimeStamp(DateTime dateTime);

    /// <summary>
    /// Converts Unix seconds to a DateTime value.
    /// </summary>
    /// <param name="unixTimeStamp">The Unix timestamp in seconds.</param>
    /// <returns>The converted DateTime value.</returns>
    DateTime UnixTimeStampToDateTime(long unixTimeStamp);
}
