# Unexpected APIs in Uno Platform

[![CI](https://github.com/MartinZikmund/uno-unexpected-apis/actions/workflows/ci.yml/badge.svg)](https://github.com/MartinZikmund/uno-unexpected-apis/actions/workflows/ci.yml)
[![Deploy](https://github.com/MartinZikmund/uno-unexpected-apis/actions/workflows/static-web-apps-deploy.yml/badge.svg)](https://github.com/MartinZikmund/uno-unexpected-apis/actions/workflows/static-web-apps-deploy.yml)

[Uno Platform](https://platform.uno) is known for its controls. It also brings a surprising amount of the Windows API surface to every platform: sensors, the clipboard, gamepads, MIDI, speech, files, sharing and more. This app shows both, live, with the code to use them.

- **Controls:** every control is live. Change a property, and the control and its XAML change together. Preview in light, dark or both at once, and at phone widths.
- **Platform APIs:** a working demo for each API, the C# that drives it, and the platforms it runs on. When the current platform lacks an API, the page says so instead of failing.
- **Quick find:** Ctrl+K matches names, capital initials (`ts` finds ToggleSwitch), keywords and scattered letters.
- **Favorites and recents**, plus browsers for theme brushes, the type ramp and every `Symbol`.

It's a community project, not an official Uno Platform app.

## Platforms

One single-project app for every Uno Platform target:

| Head | Target framework |
|---|---|
| Android | `net11.0-android` |
| iOS | `net11.0-ios` |
| tvOS | `net11.0-tvos` |
| Windows (Windows App SDK) | `net11.0-windows10.0.26100` |
| WebAssembly | `net11.0-browserwasm` |
| Desktop (Windows, macOS, Linux) | `net11.0-desktop` |

Sensor samples only come alive on a phone or tablet. The rest work almost everywhere.

## Run it

You need the .NET 11 SDK. [uno-check](https://aka.platform.uno/uno-check) sets up everything else. `src/global.json` pins `Uno.Sdk` to a 7.1 dev build.

```shell
cd src
dotnet run --project UnexpectedApis/UnexpectedApis.csproj -f net11.0-desktop
```

For the browser, use `-f net11.0-browserwasm`. For a device, open `src/UnexpectedApis.sln` in Visual Studio, Rider or VS Code and pick the target.

## Test it

```shell
cd src
dotnet test UnexpectedApis.Tests
```

In Debug builds, set `UNEXPECTEDAPIS_SMOKE` to a file path before launching. The app opens every page once and writes a pass/fail line for each.

## Add a sample

Samples are declared in `src/UnexpectedApis/Catalog`. The stage, navigation, search and home page all come from there.

A control takes one declaration in `SampleCatalog.cs`. Its knob names are the XAML property names, because the snippet is generated from them:

```csharp
new Sample<Slider>("slider", "Slider", Input, Symbol.Volume, "Picks a value from a range by feel.", () => new Slider())
    .Text("Header", "Volume", (c, v) => c.Header = v)
    .Number("Value", 40, 0, 100, 1, (c, v) => c.Value = v)
    .Tip("When the exact number matters, pair it with a NumberBox.")
    .Find("range", "volume"),
```

A platform API takes a demo control in `src/UnexpectedApis/Apis` and one declaration in a `SampleCatalog.*.cs` file:

```csharp
new ApiSample("clipboard", "Clipboard", AppSystem, Symbol.Paste, "Copies, pastes and watches text on the system clipboard.",
        ApiPlatforms.All, "https://platform.uno/docs/articles/features/clipboard.html", () => new ClipboardDemo())
    .Code("""
        DataPackage package = new();
        package.SetText("Hello world");
        Clipboard.SetContent(package);
        """)
    .Find("copy", "paste"),
```

A demo must never throw: where an API is missing, it shows that instead.

## Deployment

Every push to `main` publishes the WebAssembly head to Azure Static Web Apps (`.github/workflows/static-web-apps-deploy.yml`), and each pull request from this repo gets its own preview environment. CI builds all six heads and runs the unit tests on every pull request.

## Credits

Created by [Martin Zikmund](https://github.com/MartinZikmund), with samples by [Dominik Titl](https://github.com/morning4coffe-dev). MIT licensed, see [LICENSE](LICENSE).
