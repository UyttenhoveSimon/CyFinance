# Yahoo Finance API Client Library

[![NuGet version](https://img.shields.io/nuget/v/CyFinance.svg)](https://www.nuget.org/packages/CyFinance)
[![License: MPL 2.0](https://img.shields.io/badge/License-MPL%202.0-brightgreen.svg)](https://www.mozilla.org/MPL/2.0/)

This is a **C# client library** designed to interact with the **Yahoo Finance API**.

The project was originally a fork of [dougdellolio/YahooFinanceAPI](https://github.com/dougdellolio/YahooFinanceAPI). However, due to significant changes in Yahoo’s API, the library has been **rebuilt from scratch** with the goal of providing a **C# implementation** that closely mirrors the popular Python library [`yfinance`](https://github.com/ranaroussi/yfinance).

## Features Parity

The table below outlines the current feature parity between `yfinance` and this C# library (**CyFinance**):

| Feature | yfinance (Python) | CyFinance (C#) | API Used |
| :--- | :---: | :---: | :--- |
| Current Quote Data | ✅ | ✅ | [Quote Summary API](#2-quote-summary-api) |
| Historical Price Data | ✅ | ✅ | [Chart/Historical Data API](#1-charthistorical-data-api) |
| Company Information (Profile, Summary) | ✅ | ⚠️ Limited | [Quote Summary API](#2-quote-summary-api) |
| Financial Statements (Income, Balance Sheet, Cash Flow) | ✅ | ✅ | [Financial Statements API](#25-financial-statements-api) |
| Dividends & Splits History | ✅ | ✅ *(Current dividend data only)* | [Chart/Historical Data API](#1-charthistorical-data-api) |
| **Analyst Recommendations** | ✅ | ✅ | [Analyst Recommendations](#26-analyst-recommendations) |
| Shareholder Information (Major, Institutional) | ✅ | ✅ | [Shareholder Information](#27-shareholder-information) |
| Earnings Calendar | ✅ | ✅ | [Earnings Calendar](#28-earnings-calendar) |
| Company News | ✅ | ✅ | [Company News](#29-company-news) |
| Option Chains | ✅ | ✅ | [Options Data API](#3-options-data-api) |
| **Quote Search** | ✅ | ✅ | [Search API](#4-search-api) |
| **Stock Screening (Screener API)** | ✅ | ✅ | [Screener API](#5-screener-api) |
| Multi-Ticker Data Download | ✅ | ❌ | [Quote API (Real-time)](#12-quote-api-real-time) |
| **Market Status & Summary** | ✅ | ✅ | [Market Status and Summary API](#8-market-status-and-summary-api) |
| **Sector Domain Data** | ✅ | ✅ | [Sector/Industry APIs](#10-sectorindustry-apis) |
| **Industry Domain Data** | ✅ | ✅ | [Sector/Industry APIs](#10-sectorindustry-apis) |
| **Global Calendars (Earnings, IPO, Splits, Economic Events)** | ✅ | ✅ | [Global Calendars API](#18-global-calendars-api) |
| **Live Streaming (WebSocket)** | ✅ | ❌ | [WebSocket Streaming API](#11-websocket-streaming-api) |
| **Mutual Fund / ETF Data** | ✅ | ✅ | [Mutual Fund/ETF Data API](#15-mutual-fundetf-data-api) |
| **Currency / Forex Data** | ✅ | ✅ | [Currency/Forex API](#16-currencyforex-api) |
| **Cryptocurrency Data** | ✅ | ✅ | [Cryptocurrency API](#17-cryptocurrency-api) |

## Facade API (CyFinanceClient)

CyFinance exposes a facade client (`CyFinanceClient`) so you can access all service APIs from a single DI-resolved entry point.

### Setup

```csharp
using CyFinance;
using CyFinance.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddCyFinance();

await using var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<CyFinanceClient>();
```

### Facade API map

| Facade Property | Service Interface |
| :--- | :--- |
| `client.QuoteSummary` | `IQuoteSummaryService` |
| `client.HistoricalData` | `IHistoricalDataService` |
| `client.OptionsData` | `IOptionsDataService` |
| `client.Search` | `ISearchService` |
| `client.StockScreening` | `IStockScreeningService` |
| `client.Market` | `IMarketSummaryService` |
| `client.Calendars` | `IGlobalCalendarsService` |
| `client.EarningsCalendar` | `IEarningsCalendarService` |
| `client.FinancialStatements` | `IFinancialStatementsService` |
| `client.AnalystRecommendations` | `IAnalystRecommendationsService` |
| `client.ShareholderInformation` | `IShareholderInformationService` |
| `client.CompanyNews` | `ICompanyNewsService` |
| `client.FundData` | `IFundDataService` |
| `client.SectorIndustry` | `ISectorIndustryService` |
| `client.Currency` | `ICurrencyService` |
| `client.Crypto` | `ICryptoService` |

### Facade examples (each API)

1. Quote Summary API

```csharp
var quote = await client.QuoteSummary.GetQuoteSummaryAsync("AAPL", "price", "summaryDetail");
Console.WriteLine(quote?.QuoteSummary?.Result?[0]?.Price?.LongName);
```

2. Historical Data API

```csharp
var prices = await client.HistoricalData.GetHistoricalPricesAsync("MSFT");
Console.WriteLine($"Historical points: {prices.Count}");
```

3. Options Data API

```csharp
var expirations = await client.OptionsData.GetExpirationDatesAsync("AAPL");
Console.WriteLine($"Option expirations: {expirations.Count}");
```

4. Search API

```csharp
var search = await client.Search.SearchAsync("NVIDIA", quotesCount: 5, newsCount: 2);
Console.WriteLine($"Quotes returned: {search?.Quotes?.Count ?? 0}");
```

5. Stock Screening API

```csharp
var gainers = await client.StockScreening.ScreenPredefinedAsync(PredefinedScreenersCatalogItem.DayGainers);
Console.WriteLine(gainers is not null ? "Screener request complete" : "No screener data returned");
```

6. Market Summary API

```csharp
var snapshot = await client.Market.GetMarketSnapshotAsync("US");
Console.WriteLine($"Market open time: {snapshot?.Status?.Open}");
```

7. Global Calendars API

```csharp
var earnings = await client.Calendars.GetEarningsCalendarAsync(limit: 10);
Console.WriteLine($"Upcoming earnings events: {earnings.Count}");
```

8. Earnings Calendar API

```csharp
var earningsCalendar = await client.EarningsCalendar.GetEarningsCalendarAsync("AAPL");
Console.WriteLine($"Next earnings unix date: {earningsCalendar?.GetNextEarningsDate()}");
```

9. Financial Statements API

```csharp
var statements = await client.FinancialStatements.GetAllStatementsAsync("AAPL");
Console.WriteLine(statements is not null ? "Statements loaded" : "No statements returned");
```

10. Analyst Recommendations API

```csharp
var recommendations = await client.AnalystRecommendations.GetRecommendationsAsync("AAPL");
Console.WriteLine($"Consensus rating: {recommendations?.GetConsensusRating()}");
```

11. Shareholder Information API

```csharp
var holders = await client.ShareholderInformation.GetShareholderInformationAsync("AAPL");
Console.WriteLine($"Institutional ownership: {holders?.GetInstitutionalOwnershipPercent():F2}%");
```

12. Company News API

```csharp
var latestNews = await client.CompanyNews.GetLatestCompanyNewsAsync("AAPL");
Console.WriteLine(latestNews?.Title);
```

13. Fund Data API

```csharp
var fund = await client.FundData.GetFundSummaryAsync("VOO");
Console.WriteLine($"Fund name: {fund?.Name}");
```

14. Sector/Industry API

```csharp
var sectorInfo = await client.SectorIndustry.GetSectorInfoAsync("AAPL");
Console.WriteLine($"{sectorInfo?.Sector} / {sectorInfo?.Industry}");
```

15. Currency API

```csharp
var eurUsd = await client.Currency.GetExchangeRateAsync("EUR", "USD");
Console.WriteLine($"EUR/USD: {eurUsd?.Rate}");
```

16. Crypto API

```csharp
var btc = await client.Crypto.GetCryptoQuoteAsync("BTC", "USD");
Console.WriteLine($"BTC/USD: {btc?.Price}");
```

##  Which internal APIs are used by yfinance ?

Info from Claude, based on analysis of the yfinance library source code and documentation.

## Core Data Endpoints

### 1. **Chart/Historical Data API**

- **Endpoint**: `https://query2.finance.yahoo.com/v8/finance/chart/{ticker}`
- **Purpose**: Historical price data, volume, and chart information
- **Parameters**:
  - `period1` & `period2`: Unix timestamps for date range
  - `interval`: Time intervals (1m, 2m, 5m, 15m, 30m, 1h, 1d, 5d, 1wk, 1mo, 3mo)
  - `events`: dividends, splits
- **Usage**: Primary endpoint for `.history()` method

### 2. **Quote Summary API**

- **Endpoint**: `https://query2.finance.yahoo.com/v10/finance/quoteSummary/{ticker}`
- **Purpose**: Comprehensive ticker information including financials, statistics, and company data
- **Modules**:
  - `assetProfile`: Company profile and business description
  - `summaryProfile`: Basic company information
  - `summaryDetail`: Current price and market data
  - `esgScores`: ESG (Environmental, Social, Governance) ratings
  - `price`: Real-time price information
  - `incomeStatementHistory`: Annual income statements
  - `incomeStatementHistoryQuarterly`: Quarterly income statements
  - `balanceSheetHistory`: Annual balance sheets
  - `balanceSheetHistoryQuarterly`: Quarterly balance sheets
  - `cashflowStatementHistory`: Annual cash flow statements
  - `cashflowStatementHistoryQuarterly`: Quarterly cash flow statements
  - `defaultKeyStatistics`: Key financial metrics
  - `financialData`: Real-time financial data
  - `calendarEvents`: Upcoming earnings and events
  - `secFilings`: SEC filing information
  - `recommendationTrend`: Analyst recommendations
  - `upgradeDowngradeHistory`: Analyst upgrades/downgrades
  - `institutionOwnership`: Institutional ownership data
  - `fundOwnership`: Fund ownership data
  - `majorDirectHolders`: Major direct shareholders
  - `majorHoldersBreakdown`: Ownership breakdown
  - `insiderTransactions`: Insider trading activity
  - `insiderHolders`: Insider holdings
  - `netSharePurchaseActivity`: Share buyback activity
  - `earnings`: Earnings data and estimates
  - `earningsHistory`: Historical earnings
  - `earningsTrend`: Earnings trend and estimates
  - `industryTrend`: Industry-wide trends
  - `indexTrend`: Index trend data
  - `sectorTrend`: Sector trend data

### 2.5 **Financial Statements API**

- **Endpoint**: Uses `https://query2.finance.yahoo.com/v10/finance/quoteSummary/{ticker}` under the hood
- **Purpose**: Simplified access to income statements, balance sheets, and cash flow statements
- **Usage**: Used by `FinancialStatementsService` class

#### Financial Statements

- Income statement data via `GetIncomeStatementAsync(ticker)` - Returns annual and quarterly data
- Balance sheet data via `GetBalanceSheetAsync(ticker)` - Returns annual and quarterly data
- Cash flow statement data via `GetCashFlowStatementAsync(ticker)` - Returns annual and quarterly data
- All statements via `GetAllStatementsAsync(ticker)` - Returns all three statement types
- Custom module selection via `GetStatementsAsync(ticker, params string[] modules)`

Facade example (showcase all methods):

```csharp
var incomeStatement = await client.FinancialStatements.GetIncomeStatementAsync("AAPL");
var balanceSheet = await client.FinancialStatements.GetBalanceSheetAsync("AAPL");
var cashFlow = await client.FinancialStatements.GetCashFlowStatementAsync("AAPL");
var allStatements = await client.FinancialStatements.GetAllStatementsAsync("AAPL");
var customStatements = await client.FinancialStatements.GetStatementsAsync(
  "AAPL",
  "incomeStatementHistory",
  "balanceSheetHistory");

Console.WriteLine(incomeStatement is not null ? "Income statement loaded" : "No income statement");
Console.WriteLine(balanceSheet is not null ? "Balance sheet loaded" : "No balance sheet");
Console.WriteLine(cashFlow is not null ? "Cash flow loaded" : "No cash flow");
Console.WriteLine(allStatements is not null ? "All statements loaded" : "No full statement payload");
Console.WriteLine(customStatements is not null ? "Custom statement modules loaded" : "No custom module payload");
```

### 2.6 **Analyst Recommendations**

- **Endpoint**: Uses `https://query2.finance.yahoo.com/v10/finance/quoteSummary/{ticker}` under the hood
- **Purpose**: Simplified access to analyst recommendations and rating changes
- **Usage**: Used by `AnalystRecommendationsService` class

#### Analyst Recommendations

- Recommendation trend data via `GetRecommendationTrendAsync(ticker)` - Returns analyst sentiment over time
- Rating change history via `GetRatingChangeHistoryAsync(ticker)` - Returns upgrades/downgrades
- Complete recommendations summary via `GetRecommendationsAsync(ticker)` - Returns both trend and history with convenience methods

**Convenience Methods:**

- `GetConsensusRating()` - Calculates consensus rating (Strong Buy, Buy, Hold, Sell, Strong Sell)
- `GetRecommendationPercentages()` - Returns percentage breakdown of recommendations

Facade example (showcase all methods):

```csharp
var recommendations = await client.AnalystRecommendations.GetRecommendationsAsync("AAPL");
var trend = await client.AnalystRecommendations.GetRecommendationTrendAsync("AAPL");
var history = await client.AnalystRecommendations.GetRatingChangeHistoryAsync("AAPL");

Console.WriteLine($"Consensus: {recommendations?.GetConsensusRating()}");
Console.WriteLine($"Trend points: {trend?.Count ?? 0}");
Console.WriteLine($"Rating changes: {history?.Count ?? 0}");

var breakdown = recommendations?.GetRecommendationPercentages();
foreach (var (rating, percentage) in breakdown ?? new Dictionary<string, double>())
{
  Console.WriteLine($"{rating}: {percentage:F1}%");
}
```

### 2.7 **Shareholder Information**

- **Endpoint**: Uses `https://query2.finance.yahoo.com/v10/finance/quoteSummary/{ticker}` under the hood
- **Purpose**: Simplified access to major holders, institutional/fund ownership, and insider ownership data
- **Usage**: Used by `ShareholderInformationService` class

#### Shareholder Information

- Major holders breakdown via `GetMajorHoldersBreakdownAsync(ticker)`
- Institutional ownership list via `GetInstitutionalOwnershipAsync(ticker)`
- Fund ownership list via `GetFundOwnershipAsync(ticker)`
- Insider holders list via `GetInsiderHoldersAsync(ticker)`
- Insider transactions via `GetInsiderTransactionsAsync(ticker)`
- Full aggregate payload via `GetShareholderInformationAsync(ticker)`

**Convenience Methods:**

- `GetInstitutionalOwnershipPercent()`
- `GetInsiderOwnershipPercent()`
- `GetLargestInstitutionalHolder()`

Facade example (showcase all methods):

```csharp
var shareholders = await client.ShareholderInformation.GetShareholderInformationAsync("AAPL");
var major = await client.ShareholderInformation.GetMajorHoldersBreakdownAsync("AAPL");
var institutions = await client.ShareholderInformation.GetInstitutionalOwnershipAsync("AAPL");
var funds = await client.ShareholderInformation.GetFundOwnershipAsync("AAPL");
var insiders = await client.ShareholderInformation.GetInsiderHoldersAsync("AAPL");
var insiderTransactions = await client.ShareholderInformation.GetInsiderTransactionsAsync("AAPL");

Console.WriteLine($"Institutional Ownership: {shareholders?.GetInstitutionalOwnershipPercent():F2}%");
Console.WriteLine($"Insider Ownership: {shareholders?.GetInsiderOwnershipPercent():F2}%");
Console.WriteLine($"Largest Holder: {shareholders?.GetLargestInstitutionalHolder()?.Organization}");
Console.WriteLine($"Major holders payload: {(major is not null ? "yes" : "no")}");
Console.WriteLine($"Institutions: {institutions?.Count ?? 0}, Funds: {funds?.Count ?? 0}");
Console.WriteLine($"Insider holders: {insiders?.Count ?? 0}, Insider transactions: {insiderTransactions?.Count ?? 0}");
```

### 2.8 Earnings Calendar

- **Endpoint**: Uses `https://query2.finance.yahoo.com/v10/finance/quoteSummary/{ticker}` under the hood
- **Purpose**: Simplified access to upcoming earnings dates and historical earnings surprises
- **Usage**: Used by `EarningsCalendarService` class

#### Earnings Calendar

- Upcoming earnings dates via `GetUpcomingEarningsAsync(ticker)`
- Historical quarterly earnings via `GetHistoricalEarningsAsync(ticker)`
- Combined earnings snapshot via `GetEarningsCalendarAsync(ticker)`

Facade example (showcase all methods):

```csharp
var upcoming = await client.EarningsCalendar.GetUpcomingEarningsAsync("AAPL");
var historical = await client.EarningsCalendar.GetHistoricalEarningsAsync("AAPL");
var earnings = await client.EarningsCalendar.GetEarningsCalendarAsync("AAPL");

Console.WriteLine($"Upcoming entries: {upcoming?.EarningsDates?.Count ?? 0}");
Console.WriteLine($"Historical entries: {historical?.Quarterly?.Count ?? 0}");
Console.WriteLine($"Next Earnings Date (unix): {earnings?.GetNextEarningsDate()}");
Console.WriteLine($"Most Recent Earnings Date: {earnings?.GetMostRecentEarningsDate()}");
Console.WriteLine($"Beat Rate: {earnings?.GetEarningsBeatRate():F1}%");
```

### 2.9 **Company News

- **Endpoint**: Uses `https://query2.finance.yahoo.com/v1/finance/search` under the hood
- **Purpose**: Ticker-specific news headlines and article metadata
- **Usage**: Used by `CompanyNewsService` class

#### Company News

- Company news list via `GetCompanyNewsAsync(ticker, newsCount)`
- Latest company headline via `GetLatestCompanyNewsAsync(ticker)`
- Filtered recent company news via `GetCompanyNewsSinceAsync(ticker, sinceUnixTime, newsCount)`

Facade example (showcase all methods):

```csharp
var allNews = await client.CompanyNews.GetCompanyNewsAsync("AAPL", newsCount: 10);
var latest = await client.CompanyNews.GetLatestCompanyNewsAsync("AAPL");
var oneDayAgo = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeSeconds();
var recentNews = await client.CompanyNews.GetCompanyNewsSinceAsync("AAPL", oneDayAgo, newsCount: 20);

Console.WriteLine($"News count: {allNews?.Count ?? 0}");
Console.WriteLine($"Latest: {latest?.Title}");
Console.WriteLine($"Recent since yesterday: {recentNews?.Count ?? 0}");
```

### 3. **Options Data API**

- **Endpoint**: `https://query2.finance.yahoo.com/v7/finance/options/{ticker}`
- **Purpose**: Options chain data
- **Parameters**:
  - `date`: Expiration date (Unix timestamp)
- **Usage**: Used by `.option_chain()` method

#### Options chain

- Full option chain retrieval via `GetOptionsChainAsync(ticker, date)`
- Available expiration dates via `GetExpirationDatesAsync(ticker)`
- Expiration-specific chain via `GetOptionsForExpirationAsync(ticker, expirationDate)`
- Calls-only helper via `GetCallsAsync(ticker, date)`
- Puts-only helper via `GetPutsAsync(ticker, date)`

Facade example (showcase all methods):

```csharp
var chain = await client.OptionsData.GetOptionsChainAsync("AAPL");
var expirations = await client.OptionsData.GetExpirationDatesAsync("AAPL");
var expiration = expirations.FirstOrDefault();

if (expiration != 0)
{
  var byExpiration = await client.OptionsData.GetOptionsForExpirationAsync("AAPL", expiration);
  var calls = await client.OptionsData.GetCallsAsync("AAPL", expiration);
  var puts = await client.OptionsData.GetPutsAsync("AAPL", expiration);

  Console.WriteLine($"Expiration {expiration}: calls={calls.Count}, puts={puts.Count}");
  Console.WriteLine(byExpiration is not null ? "Expiration chain loaded" : "No expiration data");
}

Console.WriteLine(chain is not null ? "Full chain loaded" : "No options chain");
Console.WriteLine($"Available expirations: {expirations.Count}");
```

### 4. **Search API**

- **Endpoint**: `https://query2.finance.yahoo.com/v1/finance/search`
- **Purpose**: Search for tickers, news, and quotes
- **Parameters**:
  - `q`: Search query
  - `quotesCount`: Number of quote results
  - `newsCount`: Number of news results
- **Usage**: Used by `SearchService` class

#### Search API

- Search for tickers and quotes via `SearchAsync(query, quotesCount, newsCount)`
- Separate methods for quotes-only search: `SearchQuotesAsync(query, quotesCount)`
- Separate methods for news-only search: `SearchNewsAsync(query, newsCount)`
- Returns structured results with multiple result types: quotes, news, research, nav

Facade example (showcase all methods):

```csharp
var results = await client.Search.SearchAsync("Apple", quotesCount: 10, newsCount: 5);
var quotes = await client.Search.SearchQuotesAsync("Tesla", quotesCount: 5);
var news = await client.Search.SearchNewsAsync("Microsoft", newsCount: 3);

Console.WriteLine($"All search quotes: {results?.Quotes?.Count ?? 0}");
Console.WriteLine($"Quotes-only count: {quotes?.Count ?? 0}");
Console.WriteLine($"News-only count: {news?.Count ?? 0}");
```

### 5. **Screener API**

- **Endpoint**: `https://query2.finance.yahoo.com/v1/finance/screener`
- **Purpose**: Stock screening and filtering
- **Usage**: Used by `Screener` and `EquityQuery` classes

#### Screener

- Custom screener query via `POST /v1/finance/screener` (equivalent to `yf.screen(query=...)`)
- Predefined screener query via `GET /v1/finance/screener/predefined/saved?scrIds=...`
- Query operators supported in request model:
  - `EQ`, `IS-IN`, `BTWN`, `GT`, `LT`, `GTE`, `LTE`, `AND`, `OR`
- Paging/sorting fields supported in request model:
  - `offset`, `size`, `sortField`, `sortType`, `quoteType`, `userId`, `userIdType`

#### Screener catalog and typed helpers

- Predefined screener catalog available via `PredefinedScreeners` constants and `PredefinedScreenersCatalogItem` enum
- Typed query helpers available:
  - `EquityQuery`
  - `FundQuery`
  - `ETFQuery`

Facade example (showcase all methods):

```csharp
var query = EquityQuery.And(
  EquityQuery.Eq("region", "us"),
  EquityQuery.Gte("intradaymarketcap", 2_000_000_000),
  EquityQuery.Gt("dayvolume", 15_000));

var typedResult = await client.StockScreening.ScreenAsync(
  query,
  size: 25,
  sortField: "percentchange",
  sortAsc: false);

var request = new ScreenerRequest
{
  Size = 25,
  SortField = "percentchange",
  Query = query.ToNode()
};

var requestResult = await client.StockScreening.ScreenAsync(request);

var predefinedByEnum = await client.StockScreening.ScreenPredefinedAsync(PredefinedScreenersCatalogItem.DayGainers);
var predefinedByString = await client.StockScreening.ScreenPredefinedAsync("day_gainers");

Console.WriteLine(typedResult is not null ? "Typed screener loaded" : "No typed screener result");
Console.WriteLine(requestResult is not null ? "Request-based screener loaded" : "No request-based screener result");
Console.WriteLine(predefinedByEnum is not null ? "Predefined enum screener loaded" : "No predefined enum screener result");
Console.WriteLine(predefinedByString is not null ? "Predefined string screener loaded" : "No predefined string screener result");
```

### 6. **News API**

- **Endpoint**: `https://query2.finance.yahoo.com/v2/finance/news`
- **Purpose**: Financial news related to specific tickers
- **Usage**: Used for `.news` property

### 7. **Spark Chart API**

- **Endpoint**: `https://query2.finance.yahoo.com/v8/finance/spark`
- **Purpose**: Lightweight chart data for quick price updates
- **Parameters**:
  - `symbols`: Comma-separated ticker symbols
  - `range`: Time range (1d, 5d, 1mo, etc.)
  - `interval`: Data interval

## Market Data APIs

### 8. **Market Status and Summary API**

- **Summary Endpoint**: `https://query1.finance.yahoo.com/v6/finance/quote/marketSummary`
- **Status Endpoint**: `https://query1.finance.yahoo.com/v6/finance/markettime`
- **Purpose**: Market indices, summary moves, trading session windows, and timezone information
- **Usage**: Used by `MarketSummaryService` class

#### Market Status and Summary API

- Market summary by Yahoo market code via `GetMarketSummaryAsync(market)`
- Market status/time window via `GetMarketStatusAsync(market)`
- Combined snapshot via `GetMarketSnapshotAsync(market)`
- Yahoo market codes mirrored from yfinance usage include: `US`, `GB`, `ASIA`, `EUROPE`, `RATES`, `COMMODITIES`, `CURRENCIES`, `CRYPTOCURRENCIES`

Facade example (showcase all methods):

```csharp
var summary = await client.Market.GetMarketSummaryAsync("US");
var status = await client.Market.GetMarketStatusAsync("US");
var snapshot = await client.Market.GetMarketSnapshotAsync("US");

Console.WriteLine($"Summary exchanges: {summary?.Count ?? 0}");
Console.WriteLine($"Open: {status?.Open}, Close: {status?.Close}");
Console.WriteLine($"Snapshot exchanges: {snapshot?.SummaryByExchange?.Count ?? 0}");
```

### 9. **Trending Tickers API**

- **Endpoint**: `https://query2.finance.yahoo.com/v1/finance/trending/{region}`
- **Purpose**: Get trending stocks by region
- **Regions**: US, AU, CA, FR, DE, HK, IT, ES, GB, IN

### 10. **Sector/Industry APIs**

- **Endpoint**: `https://query2.finance.yahoo.com/v1/finance/sector/{sector_id}`
- **Endpoint**: `https://query2.finance.yahoo.com/v1/finance/industry/{industry_id}`
- **Purpose**: Sector and industry performance data
- **Usage**: Simplified sector and industry lookup/screening via `SectorIndustryService`

#### Sector/Industry API

- Sector and industry classification for a ticker via `GetSectorInfoAsync(ticker)`
- Sector screener via `GetStocksInSectorAsync(sector, size)`
- Industry screener via `GetStocksInIndustryAsync(industry, size)`

Facade example (showcase all methods):

```csharp
var info = await client.SectorIndustry.GetSectorInfoAsync("AAPL");
var sectorStocks = await client.SectorIndustry.GetStocksInSectorAsync("Technology", size: 10);
var industryStocks = await client.SectorIndustry.GetStocksInIndustryAsync("Consumer Electronics", size: 10);

Console.WriteLine($"Sector: {info?.Sector}, Industry: {info?.Industry}");
Console.WriteLine($"Sector stocks: {sectorStocks.Count}, Industry stocks: {industryStocks.Count}");
```

## Real-time Data APIs

### 11. **WebSocket Streaming API**

- **Endpoint**: `wss://streamer.finance.yahoo.com/`
- **Purpose**: Live streaming price data
- **Usage**: Used by `WebSocket` and `AsyncWebSocket` classes
- **Protocol**: Uses Yahoo's proprietary streaming protocol

### 12. **Quote API (Real-time)**

- **Endpoint**: `https://query1.finance.yahoo.com/v7/finance/quote`
- **Purpose**: Real-time quotes for multiple symbols
- **Parameters**:
  - `symbols`: Comma-separated ticker symbols
  - `fields`: Specific fields to return
- **Usage**: Used for real-time price updates

## Additional Endpoints

### 13. **Dividend History API**

- **Endpoint**: `https://query2.finance.yahoo.com/v8/finance/chart/{ticker}?events=div`
- **Purpose**: Historical dividend data
- **Usage**: Part of historical data retrieval

### 14. **Stock Splits API**

- **Endpoint**: `https://query2.finance.yahoo.com/v8/finance/chart/{ticker}?events=split`
- **Purpose**: Historical stock split data
- **Usage**: Part of historical data retrieval

### 15. **Mutual Fund/ETF Data API**

- **Endpoint**: `https://query2.finance.yahoo.com/v10/finance/quoteSummary/{ticker}`
- **Modules**:
  - `fundProfile`: Fund-specific information
  - `topHoldings`: Fund's top holdings
  - `bondHoldings`: Bond fund holdings
  - `equityHoldings`: Equity fund holdings
  - `bondRatings`: Bond ratings distribution
  - `sectorWeightings`: Sector allocation
- **Usage**: Simplified fund/ETF access via `FundDataService`

#### Mutual Fund/ETF API

- Fund summary via `GetFundSummaryAsync(ticker)`
- Fund profile via `GetFundProfileAsync(ticker)`
- Top holdings via `GetTopHoldingsAsync(ticker)`
- Sector weightings via `GetSectorWeightingsAsync(ticker)`
- Trailing returns via `GetTrailingReturnsAsync(ticker)`
- Annual returns via `GetAnnualReturnsAsync(ticker)`

Facade example (showcase all methods):

```csharp
var summary = await client.FundData.GetFundSummaryAsync("VOO");
var profile = await client.FundData.GetFundProfileAsync("VOO");
var holdings = await client.FundData.GetTopHoldingsAsync("VOO");
var sectors = await client.FundData.GetSectorWeightingsAsync("VOO");
var trailing = await client.FundData.GetTrailingReturnsAsync("VOO");
var annual = await client.FundData.GetAnnualReturnsAsync("VOO");

Console.WriteLine($"Fund: {summary?.Name}");
Console.WriteLine(profile is not null ? "Profile loaded" : "No profile data");
Console.WriteLine($"Holdings: {holdings?.Count ?? 0}, Sector weights: {sectors?.Count ?? 0}");
Console.WriteLine(trailing is not null ? "Trailing returns loaded" : "No trailing return data");
Console.WriteLine($"Annual return records: {annual?.Count ?? 0}");
```

### 16. **Currency/Forex API**

- **Endpoint**: `https://query2.finance.yahoo.com/v8/finance/chart/{currency_pair}`
- **Purpose**: Foreign exchange rates and historical data
- **Usage**: Used by `CurrencyService` for currency pairs (e.g., "EURUSD=X")

#### Currency/Forex API

- Current exchange rate via `GetExchangeRateAsync(baseCurrency, quoteCurrency)`
- Historical FX rates via `GetHistoricalRatesAsync(baseCurrency, quoteCurrency, startDate, endDate, interval)`
- Amount conversion helper via `ConvertAsync(amount, baseCurrency, quoteCurrency)`

Facade example (showcase all methods):

```csharp
var quote = await client.Currency.GetExchangeRateAsync("EUR", "USD");
var history = await client.Currency.GetHistoricalRatesAsync(
  "EUR",
  "USD",
  startDate: DateTime.UtcNow.AddDays(-7),
  endDate: DateTime.UtcNow,
  interval: ChartInterval.OneDay);
var converted = await client.Currency.ConvertAsync(100, "EUR", "USD");

Console.WriteLine($"{quote?.BaseCurrency}/{quote?.QuoteCurrency}: {quote?.Rate}");
Console.WriteLine($"Historical points: {history.Count}");
Console.WriteLine($"100 EUR = {converted} USD");
```

### 17. **Cryptocurrency API**

- **Endpoint**: `https://query2.finance.yahoo.com/v8/finance/chart/{crypto_symbol}`
- **Purpose**: Cryptocurrency price data
- **Usage**: Used by `CryptoService` for crypto symbols (e.g., "BTC-USD")

#### Cryptocurrency API

- Current crypto quote via `GetCryptoQuoteAsync(cryptoSymbol, quoteCurrency)`
- Historical crypto candles via `GetHistoricalPricesAsync(cryptoSymbol, quoteCurrency, startDate, endDate, interval)`

Facade example (showcase all methods):

```csharp
var btc = await client.Crypto.GetCryptoQuoteAsync("BTC", "USD");
var history = await client.Crypto.GetHistoricalPricesAsync("ETH", "USD", interval: ChartInterval.OneDay);

Console.WriteLine($"{btc?.Symbol}: {btc?.Price}");
Console.WriteLine($"Candles: {history.Count}");
```

### 18. **Global Calendars API**

- **Endpoint**: `https://query1.finance.yahoo.com/v1/finance/visualization`
- **Purpose**: Global event calendars for earnings, IPOs, stock splits, and economic releases
- **Usage**: Used by `GlobalCalendarsService` class

#### Global Calendars API

- Earnings calendar via `GetEarningsCalendarAsync(start, end, limit, offset, marketCap, filterMostActive)`
- IPO calendar via `GetIpoCalendarAsync(start, end, limit, offset)`
- Splits calendar via `GetSplitsCalendarAsync(start, end, limit, offset)`
- Economic events calendar via `GetEconomicEventsCalendarAsync(start, end, limit, offset)`
- Earnings calendar mirrors yfinance’s `Calendars.get_earnings_calendar()` behavior, including the optional `marketCap` filter and the default most-active ticker filter when `offset == 0`

Facade example (showcase all methods):

```csharp
var earnings = await client.Calendars.GetEarningsCalendarAsync(
    start: DateTime.UtcNow.Date,
    end: DateTime.UtcNow.Date.AddDays(7),
    limit: 10,
    marketCap: 1_000_000_000,
    filterMostActive: true);

var ipos = await client.Calendars.GetIpoCalendarAsync(limit: 10);
var splits = await client.Calendars.GetSplitsCalendarAsync(limit: 10);
var economics = await client.Calendars.GetEconomicEventsCalendarAsync(limit: 10);

Console.WriteLine($"Earnings: {earnings.Count}, IPOs: {ipos.Count}");
Console.WriteLine($"Splits: {splits.Count}, Economic events: {economics.Count}");
```

## Base URLs and Domains

- **Primary Domain**: `query2.finance.yahoo.com` (main API endpoint)
- **Fallback Domain**: `query1.finance.yahoo.com` (backup/alternative)
- **Streaming Domain**: `streamer.finance.yahoo.com` (WebSocket streaming)

## Native AOT Support

CyFinance is configured for **Native AOT compilation** via `PublishAot=true` in the project file. This enables the library to be compiled into native code ahead-of-time for improved performance and reduced memory footprint. Currently, the library compiles successfully with warnings about JSON serialization that can be addressed by implementing `JsonSerializerContext` for full Native AOT compatibility.

## NuGet Publishing (ModularPipelines)

Publishing is implemented with [ModularPipelines](https://github.com/thomhurst/ModularPipelines) in [pipelines/CyFinance.Pipelines/Program.cs](pipelines/CyFinance.Pipelines/Program.cs), and executed by [.github/workflows/publish-nuget.yml](.github/workflows/publish-nuget.yml).