using System.Text.Json;
using System.Text.Json.Serialization;

namespace CyFinance.Models.GlobalCalendars;

public class EarningsCalendarEvent
{
    public string? Symbol { get; set; }
    public string? Company { get; set; }
    public double? MarketCap { get; set; }
    public string? EventName { get; set; }
    public DateTime? EventStartDate { get; set; }
    public string? Timing { get; set; }
    public double? EpsEstimate { get; set; }
    public double? ReportedEps { get; set; }
    public double? SurprisePercent { get; set; }
}

public class IpoCalendarEvent
{
    public string? Symbol { get; set; }
    public string? Company { get; set; }
    public string? Exchange { get; set; }
    public DateTime? FilingDate { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? AmendedDate { get; set; }
    public double? PriceFrom { get; set; }
    public double? PriceTo { get; set; }
    public double? Price { get; set; }
    public string? Currency { get; set; }
    public double? Shares { get; set; }
    public string? DealType { get; set; }
}

public class EconomicEvent
{
    public string? Event { get; set; }
    public string? Region { get; set; }
    public DateTime? EventTime { get; set; }
    public string? Period { get; set; }
    public double? Actual { get; set; }
    public double? Expected { get; set; }
    public double? Last { get; set; }
    public double? Revised { get; set; }
}

public class SplitCalendarEvent
{
    public string? Symbol { get; set; }
    public string? Company { get; set; }
    public DateTime? PayableOn { get; set; }
    public bool? Optionable { get; set; }
    public double? OldShareWorth { get; set; }
    public double? ShareWorth { get; set; }
}

internal sealed class GlobalCalendarResponse
{
    [JsonPropertyName("finance")]
    public GlobalCalendarFinance? Finance { get; set; }
}

internal sealed class GlobalCalendarFinance
{
    [JsonPropertyName("result")]
    public List<GlobalCalendarResult>? Result { get; set; }

    [JsonPropertyName("error")]
    public object? Error { get; set; }
}

internal sealed class GlobalCalendarResult
{
    [JsonPropertyName("documents")]
    public List<GlobalCalendarDocument>? Documents { get; set; }
}

internal sealed class GlobalCalendarDocument
{
    [JsonPropertyName("rows")]
    public List<List<JsonElement>>? Rows { get; set; }
}

[JsonConverter(typeof(GlobalCalendarQueryNodeConverter))]
internal sealed class GlobalCalendarQueryNode
{
    public GlobalCalendarQueryNode(string @operator, IEnumerable<object> operands)
    {
        Operator = @operator.ToUpperInvariant();
        Operands = operands.ToList();
    }

    [JsonPropertyName("operator")]
    public string Operator { get; }

    [JsonPropertyName("operands")]
    public List<object> Operands { get; }

    public void Append(GlobalCalendarQueryNode operand)
    {
        Operands.Add(operand);
    }
}

internal sealed class GlobalCalendarRequestPayload
{
    [JsonPropertyName("sortType")]
    public string SortType { get; set; } = "DESC";

    [JsonPropertyName("entityIdType")]
    public required string EntityIdType { get; set; }

    [JsonPropertyName("sortField")]
    public required string SortField { get; set; }

    [JsonPropertyName("includeFields")]
    public required List<string> IncludeFields { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("query")]
    public required GlobalCalendarQueryNode Query { get; set; }
}

internal sealed class GlobalCalendarQueryNodeConverter : JsonConverter<GlobalCalendarQueryNode>
{
    public override GlobalCalendarQueryNode Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException("Reading GlobalCalendarQueryNode is not supported");
    }

    public override void Write(
        Utf8JsonWriter writer,
        GlobalCalendarQueryNode value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("operator");
        writer.WriteStringValue(value.Operator);

        writer.WritePropertyName("operands");
        writer.WriteStartArray();

        foreach (var operand in value.Operands)
        {
            if (operand is GlobalCalendarQueryNode node)
            {
                Write(writer, node, options);
            }
            else if (operand is string str)
            {
                writer.WriteStringValue(str);
            }
            else if (operand is long longValue)
            {
                writer.WriteNumberValue(longValue);
            }
            else if (operand is int intValue)
            {
                writer.WriteNumberValue(intValue);
            }
            else if (operand is double doubleValue)
            {
                writer.WriteNumberValue(doubleValue);
            }
            else if (operand is decimal decimalValue)
            {
                writer.WriteNumberValue(decimalValue);
            }
            else if (operand is bool boolValue)
            {
                writer.WriteBooleanValue(boolValue);
            }
            else if (operand == null)
            {
                writer.WriteNullValue();
            }
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}