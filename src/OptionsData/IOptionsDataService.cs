namespace CyFinance.Services.OptionsData
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IOptionsDataService
    {
        Task<OptionsDataResponse> GetOptionsChainAsync(string ticker, long? date = null);
        Task<List<long>> GetExpirationDatesAsync(string ticker);
        Task<OptionsChainData> GetOptionsForExpirationAsync(string ticker, long expirationDate);
        long DateTimeToUnixTimeStamp(DateTime dateTime);
        DateTime UnixTimeStampToDateTime(long unixTimeStamp);
    }
}