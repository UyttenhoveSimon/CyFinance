using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Net.Http.Json;
using YahooFinanceClient.Models.QuoteSummary;

namespace YahooFinanceClient
{
    public class QuoteSummaryService
    {
        private static readonly HttpClient _httpClient = new()
        {
            DefaultRequestHeaders =
        {
            { "User-Agent",
              "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
              "AppleWebKit/537.36 (KHTML, like Gecko) " +
              "Chrome/91.0.4472.124 Safari/537.36" }
        }
        };

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private const string BASE_URL = "https://query2.finance.yahoo.com";

        public async Task<QuoteResponse?> GetQuoteSummaryAsync(
            string ticker, params string[] modules)
        {
            var modulesList = (modules.Length > 0 ? modules :
                new[] { "price", "summaryDetail", "assetProfile", "financialData" });

            var url = $"{BASE_URL}/v10/finance/quoteSummary/{ticker}?modules={string.Join(",", modulesList)}";
            return await _httpClient.GetFromJsonAsync<QuoteResponse>(url, _jsonOptions);
        }
    }
}