using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace YahooFinanceClient.WebClient
{
    public class WebClient : IWebClient
    {
        private static readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOptions;
        private const string BASE_URL = "https://query2.finance.yahoo.com";

        private const string url = "https://finance.yahoo.com/d/quotes.csv?s=";

        static WebClient()
        {
            _httpClient = new HttpClient();

            // Configure JSON serialization options
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            // Set headers to mimic a browser request
            _httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

        }
        public async Task<string> DownloadFileAsync(string stock, string variable)
        {
            return await _httpClient.GetStringAsync(url + stock + "&f=" + variable);
        }
    }
}
