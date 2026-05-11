using System.Text.Json.Serialization;

namespace CyFinance.Models.Search;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(SearchResponse))]
internal partial class SearchJsonSerializerContext : JsonSerializerContext
{
}
