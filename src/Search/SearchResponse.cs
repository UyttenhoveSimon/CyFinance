using System.Text.Json.Serialization;

namespace CyFinance.Models.Search
{
    /// <summary>
    /// Response from Yahoo Finance search API
    /// </summary>
    public class SearchResponse
    {
        [JsonPropertyName("quotes")]
        public List<SearchQuote>? Quotes { get; set; }

        [JsonPropertyName("news")]
        public List<SearchNews>? News { get; set; }

        [JsonPropertyName("research")]
        public List<SearchResearch>? Research { get; set; }

        [JsonPropertyName("nav")]
        public List<SearchNav>? Nav { get; set; }

        [JsonPropertyName("totalTime")]
        public long TotalTime { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    /// <summary>
    /// Individual quote result from search
    /// </summary>
    public class SearchQuote
    {
        [JsonPropertyName("exchange")]
        public string? Exchange { get; set; }

        [JsonPropertyName("shortname")]
        public string? ShortName { get; set; }

        [JsonPropertyName("quoteType")]
        public string? QuoteType { get; set; }

        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }

        [JsonPropertyName("index")]
        public string? Index { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("typeDisp")]
        public string? TypeDisp { get; set; }

        [JsonPropertyName("longname")]
        public string? LongName { get; set; }

        [JsonPropertyName("isYahooFinance")]
        public bool IsYahooFinance { get; set; }
    }

    /// <summary>
    /// News result from search
    /// </summary>
    public class SearchNews
    {
        [JsonPropertyName("uuid")]
        public string? Uuid { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("publisher")]
        public string? Publisher { get; set; }

        [JsonPropertyName("link")]
        public string? Link { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("thumbnail")]
        public string? Thumbnail { get; set; }

        [JsonPropertyName("relatedTickers")]
        public List<string>? RelatedTickers { get; set; }
    }

    /// <summary>
    /// Research result from search
    /// </summary>
    public class SearchResearch
    {
        [JsonPropertyName("uuid")]
        public string? Uuid { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("thumbnail")]
        public string? Thumbnail { get; set; }
    }

    /// <summary>
    /// Navigation result from search (fund/ETF)
    /// </summary>
    public class SearchNav
    {
        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }

        [JsonPropertyName("shortname")]
        public string? ShortName { get; set; }

        [JsonPropertyName("longname")]
        public string? LongName { get; set; }

        [JsonPropertyName("quoteType")]
        public string? QuoteType { get; set; }

        [JsonPropertyName("exchange")]
        public string? Exchange { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }
    }
}
