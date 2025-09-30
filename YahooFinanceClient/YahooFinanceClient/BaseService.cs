using System.Text.Json;
using System.Text.Json.Serialization;

namespace YahooFinanceClient
{
    public abstract class BaseService
    {
        protected readonly HttpClient Client;
        protected readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        protected BaseService(HttpClient client)
        {
            Client = client;
        }
    }
}