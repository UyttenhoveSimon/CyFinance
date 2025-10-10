
using System.Text.Json;

namespace CyFinance.Services.OptionsData
{
    public class OptionsDataService : BaseService
    {
        public OptionsDataService(HttpClient client) : base(client)
        {
            if (!Client.DefaultRequestHeaders.Contains("User-Agent"))
            {
                Client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            }
        }

        private const string BaseUrl = "https://query1.finance.yahoo.com/v7/finance/options";

        /// <summary>
        /// Get options chain data for a ticker
        /// </summary>
        /// <param name="ticker">Stock ticker symbol</param>
        /// <param name="date">Optional expiration date (Unix timestamp)</param>
        /// <returns>Options data</returns>
        public async Task<OptionsDataResponse> GetOptionsChainAsync(string ticker, long? date = null)
        {
            if (string.IsNullOrWhiteSpace(ticker))
                throw new ArgumentException("Ticker cannot be null or empty", nameof(ticker));

            // This ensures _crumb is populated and valid
            await EnsureAuthenticatedAsync(ticker);

            // --- FIX IS HERE ---
            // Start building the URL with the required crumb parameter
            var url = $"{BaseUrl}/{ticker}?crumb={_crumb}";

            // Append the date if it exists
            if (date.HasValue)
            {
                url += $"&date={date.Value}"; // Use '&' since '?' is already used
            }

            var response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<OptionsDataResponse>(content, options);
        }

        /// <summary>
        /// Get all available expiration dates for a ticker
        /// </summary>
        /// <param name="ticker">Stock ticker symbol</param>
        /// <returns>List of expiration dates as Unix timestamps</returns>
        public async Task<List<long>> GetExpirationDatesAsync(string ticker)
        {
            var data = await GetOptionsChainAsync(ticker);
            return data?.OptionChain?.Result?[0]?.ExpirationDates ?? new List<long>();
        }

        /// <summary>
        /// Get options chain for a specific expiration date
        /// </summary>
        /// <param name="ticker">Stock ticker symbol</param>
        /// <param name="expirationDate">Expiration date (Unix timestamp)</param>
        /// <returns>Options chain data</returns>
        public async Task<OptionsChainData> GetOptionsForExpirationAsync(string ticker, long expirationDate)
        {
            var data = await GetOptionsChainAsync(ticker, expirationDate);
            return data?.OptionChain?.Result?[0]?.Options?[0];
        }

        /// <summary>
        /// Convert Unix timestamp to DateTime
        /// </summary>
        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).DateTime;
        }

        /// <summary>
        /// Convert DateTime to Unix timestamp
        /// </summary>
        public static long DateTimeToUnixTimeStamp(DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        }
    }
}