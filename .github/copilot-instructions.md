# CyFinance — GitHub Copilot Instructions

## Project Overview

CyFinance is a C# client library for the Yahoo Finance API. It targets .NET 10 with Native AOT enabled. The library provides strongly-typed wrappers around Yahoo Finance endpoints.

## Architecture

- `src/` — Main library (`CyFinance.csproj`). Each feature lives in its own folder with a `Models`, `Service`, and `IService` file.
- `tests/` — Unit and integration tests using TUnit, with NSubstitute for mocking.
- `pipelines/` — ModularPipelines-based C# pipeline (replaces YAML for build/test/publish logic).

## Coding Conventions

- Each new feature must have a corresponding `I{Feature}Service.cs`, `{Feature}Service.cs`, and `{Feature}Models.cs` in a dedicated subfolder under `src/`.
- Services inherit from `BaseService` and use the shared `HttpClient` via DI.
- All public APIs are `async Task<T>` returning strongly-typed model classes.
- Register every new service in `src/DependencyInjection/CyFinanceServiceCollectionExtensions.cs`.
- Use `System.Text.Json` for deserialization. AOT-safe: avoid `dynamic`, `object`, and unbound generics in model classes.
- All public types added to model files must be AOT-compatible (no reflection-based serialization).
- Use Conventional Commits for all commit messages.
	- Format: `type(scope): subject`
	- Common types: `feat`, `fix`, `docs`, `test`, `refactor`, `chore`, `ci`, `build`

## Testing Requirements

- Every new `Service` must have a corresponding test class under `tests/{Feature}.Tests/`.
- Use TUnit (`[Test]`, `[Arguments]`, `Assert.That`). Do not use xUnit or NUnit.
- Mock the `HttpClient` via `NSubstitute` — do not make live HTTP calls in unit tests.
- Set the `X-CyFinance-SkipAuth` header on mocked `HttpRequestMessage` instances to bypass the Yahoo crumb authentication flow.
- Run the full test suite with `dotnet run --project tests/CyFinance.Tests.csproj` before submitting.
- Target: **0 test failures**.

## Pipeline (ModularPipelines)

The pipeline runs via `dotnet run --project pipelines/CyFinance.Pipelines/CyFinance.Pipelines.csproj`. Modules: restore → build → test → pack → push. Do not modify `publish-nuget.yml` without also updating `Program.cs`.

## Security

- Never log or expose API keys or secrets.
- Validate all external input at the HTTP boundary.
- Keep dependencies up to date with dependabot.
