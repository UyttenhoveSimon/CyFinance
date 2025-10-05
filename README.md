# Yahoo Finance API Client Library

This is a **C# client library** designed to interact with the **Yahoo Finance API**.

The project was originally a fork of [dougdellolio/YahooFinanceAPI](https://github.com/dougdellolio/YahooFinanceAPI). However, due to significant changes in Yahoo’s API, the library has been **rebuilt from scratch** with the goal of providing a **C# implementation** that closely mirrors the popular Python library [`yfinance`](https://github.com/ranaroussi/yfinance).

The table below outlines the current feature parity between `yfinance` and this C# library (**CyFinance**):

| Feature | yfinance (Python) | CyFinance (C#) |
| :--- | :---: | :---: |
| Current Quote Data | ✅ | ✅ |
| Historical Price Data | ✅ | ❌ |
| Company Information (Profile, Summary) | ✅ | ⚠️ Limited |
| Financial Statements (Income, Balance Sheet, Cash Flow) | ✅ | ❌ |
| Dividends & Splits History | ✅ | ✅ *(Current dividend data only)* |
| Analyst Recommendations | ✅ | ❌ |
| Shareholder Information (Major, Institutional) | ✅ | ❌ |
| Earnings Calendar | ✅ | ❌ |
| Option Chains | ✅ | ❌ |
| Company News | ✅ | ❌ |
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

### 5. **Search API**

- **Endpoint**: `https://query2.finance.yahoo.com/v1/finance/search`
- **Purpose**: Search for tickers, news, and quotes
- **Parameters**:
  - `q`: Search query
  - `quotesCount`: Number of quote results
  - `newsCount`: Number of news results
- **Usage**: Used by `Search` class

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