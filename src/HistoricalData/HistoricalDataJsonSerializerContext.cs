using System.Text.Json.Serialization;

namespace CyFinance.Models.HistoricalData;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(ChartResponse))]
internal partial class HistoricalDataJsonSerializerContext : JsonSerializerContext
{
}
