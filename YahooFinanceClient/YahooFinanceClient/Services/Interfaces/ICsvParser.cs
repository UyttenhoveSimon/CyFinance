using System.Threading.Tasks;
using YahooFinanceClient.Models;

namespace YahooFinanceClient.CsvParser
{
    public interface ICsvParser
    {
        Task<Stock> RetrieveStockAsync(string ticker);
    }
}
