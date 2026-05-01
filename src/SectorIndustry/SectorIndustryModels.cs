using CyFinance.Models.StockScreening;

namespace CyFinance.Models.SectorIndustry;

/// <summary>
/// Sector and industry classification for a given ticker.
/// </summary>
public class SectorIndustryInfo
{
    public string? Ticker { get; set; }
    public string? Sector { get; set; }
    public string? SectorKey { get; set; }
    public string? Industry { get; set; }
    public string? IndustryKey { get; set; }
    public string? Website { get; set; }
    public string? Country { get; set; }
    public int? FullTimeEmployees { get; set; }
}

/// <summary>
/// Summary row for a stock returned when screening by sector or industry.
/// </summary>
public class SectorScreenerEntry
{
    public string? Symbol { get; set; }
    public string? Name { get; set; }
    public string? Exchange { get; set; }
    public decimal? Price { get; set; }
    public decimal? Change { get; set; }
    public decimal? ChangePercent { get; set; }
    public long? MarketCap { get; set; }
    public long? Volume { get; set; }
}
