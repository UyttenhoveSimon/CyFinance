using System.Text.Json.Serialization;

namespace CyFinance.Models.StockScreening;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(ScreenerResponse))]
[JsonSerializable(typeof(ScreenerRequest))]
internal partial class StockScreeningJsonSerializerContext : JsonSerializerContext
{
}
