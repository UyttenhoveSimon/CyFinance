using System.Text.Json.Serialization;

namespace CyFinance.Models.MarketSummary;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(MarketSummaryRoot))]
[JsonSerializable(typeof(MarketTimeRoot))]
internal partial class MarketSummaryJsonSerializerContext : JsonSerializerContext
{
}
