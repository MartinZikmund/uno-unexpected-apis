#if DEBUG
using UnexpectedApis.Catalog;
using UnexpectedApis.Pages;

namespace UnexpectedApis;

/// <summary>Opens every page once and logs what failed. Set UNEXPECTEDAPIS_SMOKE to the report path to run it.</summary>
internal static class SmokeRunner
{
    private static readonly TimeSpan SettleTime = TimeSpan.FromMilliseconds(400);

    public static void StartIfRequested(Frame frame)
    {
        if (Environment.GetEnvironmentVariable("UNEXPECTEDAPIS_SMOKE") is { Length: > 0 } reportPath)
        {
            _ = RunAsync(frame, reportPath);
        }
    }

    private static async Task RunAsync(Frame frame, string reportPath)
    {
        List<string> report = new();

        foreach (var sample in SampleCatalog.All)
        {
            var navigated = frame.Navigate(typeof(SamplePage), sample.Id);
            await Task.Delay(SettleTime);
            var error = (frame.Content as SamplePage)?.LastError;
            report.Add($"{(navigated && error is null ? "ok  " : "FAIL")} {sample.Id} {error}");
        }

        foreach (var page in new[] { typeof(BrushesPage), typeof(TypographyPage), typeof(IconsPage), typeof(HomePage) })
        {
            var navigated = frame.Navigate(page);
            await Task.Delay(SettleTime);
            report.Add($"{(navigated ? "ok  " : "FAIL")} {page.Name}");
        }

        await File.WriteAllLinesAsync(reportPath, report);
    }
}
#endif
