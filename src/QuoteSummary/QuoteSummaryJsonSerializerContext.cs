using System.Text.Json.Serialization;

namespace CyFinance.Models.QuoteSummary;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(QuoteResponse))]
internal partial class QuoteSummaryJsonSerializerContext : JsonSerializerContext
{
}
