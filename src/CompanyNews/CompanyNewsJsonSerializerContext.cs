using System.Text.Json.Serialization;

namespace CyFinance.Models.CompanyNews;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(CompanyNewsResponse))]
internal partial class CompanyNewsJsonSerializerContext : JsonSerializerContext
{
}
