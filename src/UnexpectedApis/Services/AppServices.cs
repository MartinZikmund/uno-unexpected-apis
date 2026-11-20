using UnexpectedApis.Core;
using Windows.ApplicationModel.DataTransfer;

namespace UnexpectedApis.Services;

public static class AppServices
{
    private static readonly string LibraryPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "UnexpectedApis",
        "library.json");

    public static UserLibrary Library { get; } = LoadLibrary();

    public static Action<string>? Navigate { get; set; }

    public static Action<string>? ShowToast { get; set; }

    public static void Copy(string text, string message)
    {
        try
        {
#pragma warning disable Uno0001 // tvOS has no clipboard; the catch reports it.
            DataPackage package = new();
            package.SetText(text);
            Clipboard.SetContent(package);
#pragma warning restore Uno0001
            ShowToast?.Invoke(message);
        }
        catch (Exception ex)
        {
            ShowToast?.Invoke($"Couldn't copy: {ex.Message}");
        }
    }

    public static void SaveLibrary()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LibraryPath)!);
            File.WriteAllText(LibraryPath, Library.ToJson());
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine($"Could not save the library: {ex.Message}");
        }
    }

    private static UserLibrary LoadLibrary()
    {
        try
        {
            return File.Exists(LibraryPath) ? UserLibrary.FromJson(File.ReadAllText(LibraryPath)) : new();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine($"Could not read the library: {ex.Message}");
            return new();
        }
    }
}

public static class Routes
{
    public const string Home = "home";
    public const string Brushes = "token:brushes";
    public const string Typography = "token:type";
    public const string Icons = "token:icons";
    public const string SamplePrefix = "sample:";

    public static string ForSample(string id) => SamplePrefix + id;
}
