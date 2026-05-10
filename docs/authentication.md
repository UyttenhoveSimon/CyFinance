# Authentication

CyFinance handles the Yahoo Finance crumb flow inside the services, so normal callers only need to resolve the client and invoke the API.

For tests and mocked HTTP clients, the repository uses the `X-CyFinance-SkipAuth` header to bypass the live Yahoo crumb fetch.

That keeps unit tests deterministic while preserving the real authentication flow for production callers.