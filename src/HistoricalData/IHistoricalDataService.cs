using CyFinance.Models.HistoricalData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CyFinance.Services.HistoricalData
{
    public interface IHistoricalDataService
    {
        Task<ChartResponse> GetHistoricalDataAsync(
            string ticker,
            DateTime? startDate = null,
            DateTime? endDate = null,
            ChartInterval interval = ChartInterval.OneDay,
            bool includeDividends = true,
            bool includeSplits = true);

        Task<List<HistoricalPrice>> GetHistoricalPricesAsync(
            string ticker,
            DateTime? startDate = null,
            DateTime? endDate = null,
            ChartInterval interval = ChartInterval.OneDay,
            bool includeDividends = true,
            bool includeSplits = true);

        List<DividendInfo> GetDividends(ChartResponse chartResponse);
        List<SplitInfo> GetSplits(ChartResponse chartResponse);
    }
}