
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CyFinance.Services.OptionsData;

/// <summary>
/// Provides option chains. Expiration dates are Unix timestamps in seconds throughout,
/// because that is what Yahoo expects back when requesting a specific expiry.
/// </summary>
public interface IOptionsDataService
{
    /// <summary>
    /// Gets the chain payload as Yahoo returns it.
    /// </summary>
    /// <param name="date">An expiration to fetch. Defaults to the nearest one.</param>
    Task<OptionsDataResponse> GetOptionsChainAsync(string ticker, long? date = null);

    /// <summary>
    /// Gets every expiration Yahoo lists for the ticker.
    /// </summary>
    Task<List<long>> GetExpirationDatesAsync(string ticker);

    /// <summary>
    /// Gets the calls and puts for one expiration.
    /// </summary>
    /// <param name="expirationDate">
    /// An expiration taken from <see cref="GetExpirationDatesAsync" />. Yahoo matches it
    /// exactly, so a timestamp that is merely close to one returns nothing.
    /// </param>
    Task<OptionsChainData?> GetOptionsForExpirationAsync(string ticker, long expirationDate);

    /// <summary>
    /// Gets the call contracts.
    /// </summary>
    /// <param name="date">An expiration to fetch. Defaults to the nearest one.</param>
    Task<List<OptionContract>> GetCallsAsync(string ticker, long? date = null);

    /// <summary>
    /// Gets the put contracts.
    /// </summary>
    /// <param name="date">An expiration to fetch. Defaults to the nearest one.</param>
    Task<List<OptionContract>> GetPutsAsync(string ticker, long? date = null);

    long DateTimeToUnixTimeStamp(DateTime dateTime);

    DateTime UnixTimeStampToDateTime(long unixTimeStamp);
}
