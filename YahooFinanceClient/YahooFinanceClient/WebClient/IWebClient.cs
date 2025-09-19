using System.Threading.Tasks;

namespace YahooFinanceClient.WebClient
{
    public interface IWebClient
    {
        Task<string> DownloadFileAsync(string stock, string variable);
    }
}
