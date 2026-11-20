using UnexpectedApis.Apis;
using UnexpectedApis.Core;

namespace UnexpectedApis.Catalog;

public static partial class SampleCatalog
{
    private static List<Sample> DeviceAndSystemApis() =>
    [
        new ApiSample("clipboard", "Clipboard", AppSystem, Symbol.Paste, "Copies, pastes and watches text on the system clipboard.",
                ApiPlatforms.All, "https://platform.uno/docs/articles/features/clipboard.html", () => new ClipboardDemo())
            .Code("""
                DataPackage package = new();
                package.SetText("Hello world");
                Clipboard.SetContent(package);

                Clipboard.ContentChanged += async (s, e) =>
                {
                    var content = Clipboard.GetContent();
                    var text = await content.GetTextAsync();
                };
                """)
            .Tip("In the browser, reading the clipboard needs a user gesture and the page's permission.")
            .Find("copy", "paste", "datapackage"),
    ];
}
