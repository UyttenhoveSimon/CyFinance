using System.Text.Json.Serialization;

namespace CyFinance.Models.GlobalCalendars;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(GlobalCalendarResponse))]
[JsonSerializable(typeof(GlobalCalendarRequestPayload))]
internal partial class GlobalCalendarsJsonSerializerContext : JsonSerializerContext
{
}
