---
title: CyFinance, a Yahoo Finance API client for C# and .NET
description: CyFinance is an unofficial, strongly typed Yahoo Finance API client for C# and .NET 10. Get stock quotes, historical prices, financial statements, options chains, screeners, ETF, forex and crypto data from .NET, with an API modelled on Python's yfinance.
---

# CyFinance: Yahoo Finance API client for C# and .NET

CyFinance is a strongly typed Yahoo Finance client for .NET 10. It gives C#
applications the market data Python developers get from
[yfinance](https://github.com/ranaroussi/yfinance): real-time and delayed
quotes, historical OHLCV prices, dividends, company profiles, financial
statements, analyst recommendations, options chains, screeners and more.
It is free and open source under the Apache License 2.0, and is not affiliated
with or endorsed by Yahoo.

This site contains the generated API reference together with a few short
guides for the most common tasks. The [library guide](../README.md) lists every
endpoint with examples.

## Install

CyFinance is published on [NuGet](https://www.nuget.org/packages/CyFinance):

```bash
dotnet add package CyFinance
```

## Quick start

Register the services with dependency injection and resolve the facade client.

```csharp
using CyFinance.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddCyFinance();

await using var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<CyFinanceClient>();
```

From there, call the specific service you need:

```csharp
var quote = await client.QuoteSummary.GetQuoteSummaryAsync("AAPL", "price", "summaryDetail");
var prices = await client.HistoricalData.GetHistoricalPricesAsync("MSFT");
```

Then use the API reference in the navigation for the individual services.

## What you can get from Yahoo Finance with CyFinance

- **Quotes and company data**: current price, market cap, P/E, summary
  detail and company profile through the Quote Summary API.
- **Historical prices**: daily, weekly or intraday OHLCV candles, adjusted
  close, dividends and splits through the Chart API.
- **Fundamentals**: income statements, balance sheets and cash-flow
  statements, annual and quarterly.
- **Analysts and owners**: recommendations, price targets, major and
  institutional shareholders.
- **Events**: earnings calendars, global IPO, split and economic-event
  calendars, company news.
- **Options**: option chains by expiry, calls and puts with strikes, volume,
  open interest and implied volatility.
- **Discovery**: ticker search and stock screeners.
- **Markets**: market status and summary, sectors and industries.
- **Other assets**: mutual funds and ETFs, currencies (forex) and
  cryptocurrencies.

## Why CyFinance

- **Typed models** for every response instead of raw JSON.
- **Handles Yahoo's cookie and crumb authentication** for you (see
  [Authentication](authentication.md)).
- **Built for modern .NET**: async APIs, `IHttpClientFactory`, dependency
  injection, trimming and Native AOT compatible.
- **One entry point**: the `CyFinanceClient` facade exposes every service; each
  service can also be injected on its own (see
  [Common patterns](common-patterns.md)).

## FAQ

**Is there an official Yahoo Finance API for .NET?** Yahoo does not publish an
official, documented API. CyFinance calls the same public endpoints the Yahoo
Finance website and yfinance use, so they may change without notice.

**How does CyFinance compare to yfinance?** It mirrors yfinance's features for
C#; the [feature parity table](../README.md#features-parity) shows what is
covered. Multi-ticker downloads and WebSocket streaming are not implemented yet.

**Which .NET versions are supported?** .NET 10 and later.
