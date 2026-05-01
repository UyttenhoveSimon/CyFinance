# Yahoo Finance API Client Library

This is a **C# client library** designed to interact with the **Yahoo Finance API**.

The project was originally a fork of [dougdellolio/YahooFinanceAPI](https://github.com/dougdellolio/YahooFinanceAPI). However, due to significant changes in Yahoo’s API, the library has been **rebuilt from scratch** with the goal of providing a **C# implementation** that closely mirrors the popular Python library [`yfinance`](https://github.com/ranaroussi/yfinance).
## Native AOT Support

CyFinance is configured for **Native AOT compilation** via `PublishAot=true` in the project file. This enables the library to be compiled into native code ahead-of-time for improved performance and reduced memory footprint. Currently, the library compiles successfully with warnings about JSON serialization that can be addressed by implementing `JsonSerializerContext` for full Native AOT compatibility.
The table below outlines the current feature parity between `yfinance` and this C# library (**CyFinance**):

| Feature | yfinance (Python) | CyFinance (C#) |
| :--- | :---: | :---: |
| Current Quote Data | ✅ | ✅ |
| Historical Price Data | ✅ | ❌ |
| Company Information (Profile, Summary) | ✅ | ⚠️ Limited |
| Financial Statements (Income, Balance Sheet, Cash Flow) | ✅ | ✅ |
| Dividends & Splits History | ✅ | ✅ *(Current dividend data only)* |
| **Analyst Recommendations** | ✅ | ✅ |
| Shareholder Information (Major, Institutional) | ✅ | ❌ |
| Earnings Calendar | ✅ | ❌ |
| Option Chains | ✅ | ❌ |
| Company News | ✅ | ❌ |
| **Quote Search** | ✅ | ✅ |
| **Stock Screening (Screener API)** | ✅ | ✅ |
| Multi-Ticker Data Download | ✅ | ❌ |

##  Which internal APIs are used by yfinance ?

Info from Claude: Based on analysis of the yfinance library source code and documentation.

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

### 2.5 **Financial Statements API** *(Dedicated Service)*

- **Endpoint**: Uses `https://query2.finance.yahoo.com/v10/finance/quoteSummary/{ticker}` under the hood
- **Purpose**: Simplified access to income statements, balance sheets, and cash flow statements
- **Usage**: Used by `FinancialStatementsService` class

#### Financial Statements parity with yfinance (implemented)

- Income statement data via `GetIncomeStatementAsync(ticker)` - Returns annual and quarterly data
- Balance sheet data via `GetBalanceSheetAsync(ticker)` - Returns annual and quarterly data
- Cash flow statement data via `GetCashFlowStatementAsync(ticker)` - Returns annual and quarterly data
- All statements via `GetAllStatementsAsync(ticker)` - Returns all three statement types
- Custom module selection via `GetStatementsAsync(ticker, params string[] modules)`

Example (get income statement):

```csharp
var financialStatementsService = new FinancialStatementsService(quoteSummaryService);
var incomeStatement = await financialStatementsService.GetIncomeStatementAsync("AAPL");

foreach (var statement in incomeStatement?.AnnualStatements ?? new List<FinancialStatement>())
{
    Console.WriteLine($"Total Revenue: {statement.TotalRevenue?.Fmt}");
    Console.WriteLine($"Net Income: {statement.NetIncome?.Fmt}");
}
```

Example (get all financial statements):

```csharp
var allStatements = await financialStatementsService.GetAllStatementsAsync("MSFT");

// Access each statement type
var incomeStatements = allStatements?.IncomeStatementHistory?.IncomeStatementStatements;
var balanceSheets = allStatements?.BalanceSheetHistory?.BalanceSheetStatements;
var cashFlows = allStatements?.CashflowStatementHistory?.CashflowStatements;
```

### 2.6 **Analyst Recommendations Service** *(Dedicated Service)*

- **Endpoint**: Uses `https://query2.finance.yahoo.com/v10/finance/quoteSummary/{ticker}` under the hood
- **Purpose**: Simplified access to analyst recommendations and rating changes
- **Usage**: Used by `AnalystRecommendationsService` class

#### Analyst Recommendations parity with yfinance (implemented)

- Recommendation trend data via `GetRecommendationTrendAsync(ticker)` - Returns analyst sentiment over time
- Rating change history via `GetRatingChangeHistoryAsync(ticker)` - Returns upgrades/downgrades
- Complete recommendations summary via `GetRecommendationsAsync(ticker)` - Returns both trend and history with convenience methods

**Convenience Methods:**
- `GetConsensusRating()` - Calculates consensus rating (Strong Buy, Buy, Hold, Sell, Strong Sell)
- `GetRecommendationPercentages()` - Returns percentage breakdown of recommendations

Example (get consensus rating):

```csharp
var recommendationsService = new AnalystRecommendationsService(quoteSummaryService);
var recommendations = await recommendationsService.GetRecommendationsAsync("AAPL");

Console.WriteLine($"Consensus: {recommendations?.GetConsensusRating()}");

var breakdown = recommendations?.GetRecommendationPercentages();
foreach (var (rating, percentage) in breakdown ?? new Dictionary<string, double>())
{
    Console.WriteLine($"{rating}: {percentage:F1}%");
}
```

Example (get rating change history):

```csharp
var history = await recommendationsService.GetRatingChangeHistoryAsync("MSFT");

foreach (var change in history ?? new List<RatingChange>())
{
    Console.WriteLine($"{change.Firm}: {change.FromGrade} → {change.ToGrade} ({change.Action})");
}
```

### 3. **Options Data API**

- **Endpoint**: `https://query2.finance.yahoo.com/v7/finance/options/{ticker}`
- **Purpose**: Options chain data
- **Parameters**:
  - `date`: Expiration date (Unix timestamp)
- **Usage**: Used by `.option_chain()` method

### 4. **Screener API**

- **Endpoint**: `https://query2.finance.yahoo.com/v1/finance/screener`
- **Purpose**: Stock screening and filtering
- **Usage**: Used by `Screener` and `EquityQuery` classes

#### Screener parity with yfinance (implemented)

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

Example (predefined):

```csharp
var result = await screenerService.ScreenPredefinedAsync(PredefinedScreenersCatalogItem.DayGainers);
```

Example (typed custom query):

```csharp
var query = EquityQuery.And(
    EquityQuery.Eq("region", "us"),
    EquityQuery.Gte("intradaymarketcap", 2_000_000_000),
    EquityQuery.Gt("dayvolume", 15_000));

var result = await screenerService.ScreenAsync(
    query,
    size: 25,
    sortField: "percentchange",
    sortAsc: false);
```

### 5. **Search API**

- **Endpoint**: `https://query2.finance.yahoo.com/v1/finance/search`
- **Purpose**: Search for tickers, news, and quotes
- **Parameters**:
  - `q`: Search query
  - `quotesCount`: Number of quote results
  - `newsCount`: Number of news results
- **Usage**: Used by `SearchService` class

#### Search API parity with yfinance (implemented)

- Search for tickers and quotes via `SearchAsync(query, quotesCount, newsCount)`
- Separate methods for quotes-only search: `SearchQuotesAsync(query, quotesCount)`
- Separate methods for news-only search: `SearchNewsAsync(query, newsCount)`
- Returns structured results with multiple result types: quotes, news, research, nav

Example (comprehensive search):

```csharp
var searchService = new SearchService(httpClient);
var results = await searchService.SearchAsync("Apple", quotesCount: 10, newsCount: 5);

foreach (var quote in results?.Quotes ?? new List<SearchQuote>())
{
    Console.WriteLine($"{quote.Symbol}: {quote.LongName} ({quote.Exchange})");
}
```

Example (quotes only):

```csharp
var quotes = await searchService.SearchQuotesAsync("Tesla", quotesCount: 5);
```

Example (news only):

```csharp
var news = await searchService.SearchNewsAsync("Microsoft", newsCount: 3);
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

### 8. **Market Summary API**

- **Endpoint**: `https://query2.finance.yahoo.com/v6/finance/quote/marketSummary`
- **Purpose**: Market indices and summary information
- **Usage**: Used by `Market` class

### 9. **Trending Tickers API**

- **Endpoint**: `https://query2.finance.yahoo.com/v1/finance/trending/{region}`
- **Purpose**: Get trending stocks by region
- **Regions**: US, AU, CA, FR, DE, HK, IT, ES, GB, IN

### 10. **Sector/Industry APIs**

- **Endpoint**: `https://query2.finance.yahoo.com/v1/finance/sector/{sector_id}`
- **Endpoint**: `https://query2.finance.yahoo.com/v1/finance/industry/{industry_id}`
- **Purpose**: Sector and industry performance data
- **Usage**: Used by `Sector` and `Industry` classes

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

### 16. **Currency/Forex API**

- **Endpoint**: `https://query2.finance.yahoo.com/v8/finance/chart/{currency_pair}`
- **Purpose**: Foreign exchange rates and historical data
- **Usage**: Used for currency pairs (e.g., "EURUSD=X")

### 17. **Cryptocurrency API**

- **Endpoint**: `https://query2.finance.yahoo.com/v8/finance/chart/{crypto_symbol}`
- **Purpose**: Cryptocurrency price data
- **Usage**: Used for crypto symbols (e.g., "BTC-USD")

## Base URLs and Domains

- **Primary Domain**: `query2.finance.yahoo.com` (main API endpoint)
- **Fallback Domain**: `query1.finance.yahoo.com` (backup/alternative)
- **Streaming Domain**: `streamer.finance.yahoo.com` (WebSocket streaming)

## How to Install?


## What is Provided?