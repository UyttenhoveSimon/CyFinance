using System.Threading.Tasks;

namespace YahooFinanceClient.WebClient
{
    public class WebClient : IWebClient
    {
        private const string url = "http://finance.yahoo.com/d/quotes.csv?s=";

        public async Task<string> DownloadFileAsync(string stock, string variable)
        {
            string csvData;
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                csvData = await httpClient.GetStringAsync(url + stock + "&f=" + variable);
            }
            return csvData;
        }
    }
}
