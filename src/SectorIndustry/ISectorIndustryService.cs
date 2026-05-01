using CyFinance.Models.SectorIndustry;

namespace CyFinance.Services.SectorIndustry;

/// <summary>
/// Service contract for sector and industry data.
/// </summary>
public interface ISectorIndustryService
{
    /// <summary>
    /// Returns the sector and industry classification for a given ticker symbol.
    /// </summary>
    Task<SectorIndustryInfo?> GetSectorInfoAsync(string ticker);

    /// <summary>
    /// Returns stocks that belong to the given sector (e.g. "Technology").
    /// Uses the Yahoo Finance screener under the hood.
    /// </summary>
    Task<List<SectorScreenerEntry>> GetStocksInSectorAsync(string sector, int size = 25);

    /// <summary>
    /// Returns stocks that belong to the given industry (e.g. "Software—Application").
    /// Uses the Yahoo Finance screener under the hood.
    /// </summary>
    Task<List<SectorScreenerEntry>> GetStocksInIndustryAsync(string industry, int size = 25);
}
