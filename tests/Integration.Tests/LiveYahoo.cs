using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using TUnit.Core.Exceptions;

namespace CyFinance.Tests.Integration;

/// <summary>
/// Helpers for the tests that talk to the real Yahoo endpoints.
/// </summary>
/// <remarks>
/// Yahoo serves these endpoints without authentication and throttles shared CI
/// runners, so a call can come back refused or empty for reasons that have nothing
/// to do with this library. <see cref="RunAsync" /> retries the whole test a few
/// times and, when Yahoo still has nothing to say, skips it with the reason instead
/// of failing the build. Assertions about the shape of data Yahoo did return are
/// left alone, so a genuine regression still fails.
/// </remarks>
internal static class LiveYahoo
{
    private const int Attempts = 3;

    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(5);

    public static async Task RunAsync(string testName, Func<Task> test)
    {
        var failures = new List<string>();

        for (var attempt = 1; attempt <= Attempts; attempt++)
        {
            if (attempt > 1)
            {
                await Task.Delay(RetryDelay);
            }

            try
            {
                await test();

                if (attempt > 1)
                {
                    Console.WriteLine($"[live] {testName}: attempt {attempt}/{Attempts} succeeded.");
                }

                return;
            }
            catch (Exception exception) when (IsYahooUnavailable(exception))
            {
                failures.Add($"attempt {attempt}/{Attempts}: {Describe(exception)}");
                Console.WriteLine($"[live] {testName}: {failures[^1]}");
            }
        }

        throw new SkipTestException(
            $"Yahoo returned no usable data for {testName} ({string.Join("; ", failures)}). " +
            "The library was not exercised, so this run proves nothing either way.");
    }

    /// <summary>
    /// Tells "Yahoo gave us nothing" apart from "Yahoo gave us something and it was wrong".
    /// </summary>
    private static bool IsYahooUnavailable(Exception exception) => exception switch
    {
        HttpRequestException => true,
        // The shared client has a 90 second timeout, which surfaces as a cancellation.
        TaskCanceledException => true,
        System.TimeoutException => true,
        _ => IsMissingDataAssertion(exception)
    };

    /// <summary>
    /// TUnit assertion failures carry no structured reason, so the message is all
    /// there is to separate an absent payload from a wrong one. An assertion that
    /// compares values keeps failing the build; only "nothing came back" is tolerated.
    /// </summary>
    private static bool IsMissingDataAssertion(Exception exception)
    {
        if (exception.GetType().Namespace?.StartsWith("TUnit", StringComparison.Ordinal) != true)
        {
            return false;
        }

        string[] markers = ["to not be null", "received null", "to not be empty"];

        return markers.Any(marker => exception.Message.Contains(marker, StringComparison.OrdinalIgnoreCase));
    }

    private static string Describe(Exception exception) =>
        $"{exception.GetType().Name}: {exception.Message.ReplaceLineEndings(" ").Trim()}";
}
