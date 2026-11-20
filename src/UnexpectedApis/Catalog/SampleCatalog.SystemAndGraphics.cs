using UnexpectedApis.Apis;
using UnexpectedApis.Core;

namespace UnexpectedApis.Catalog;

public static partial class SampleCatalog
{
    private static List<Sample> SystemAndGraphicsApis() =>
    [
        new ApiSample("jumplist", "Jump list", AppSystem, Symbol.AllApps, "Adds your own shortcuts to the app's icon on the taskbar or home screen.",
                ApiPlatforms.Windows | ApiPlatforms.Android | ApiPlatforms.iOS,
                "https://platform.uno/docs/articles/features/windows-ui-startscreen.html", () => new JumpListDemo())
            .Height(560)
            .Code("""
                if (JumpList.IsSupported())
                {
                    var jumpList = await JumpList.LoadCurrentAsync();

                    var item = JumpListItem.CreateWithArguments("open-inbox", "Inbox");
                    item.Description = "Jump straight to your mail";
                    item.Logo = new Uri("ms-appx:///Assets/inbox.png");

                    jumpList.Items.Add(item);
                    await jumpList.SaveAsync();
                }
                """)
            .Tip("Android and iOS show jump list items as app shortcuts. Desktop heads, tvOS and the browser report IsSupported() as false.")
            .Find("shortcut", "taskbar", "quick actions", "startscreen"),

        new ApiSample("titlebar", "Title bar", AppSystem, Symbol.NewWindow, "Changes the window's title and the colors of its caption buttons.",
                ApiPlatforms.Windows | ApiPlatforms.Desktop,
                "https://platform.uno/docs/articles/features/windows-ui-xaml-window.html#customizing-window-border-and-title-bar", () => new TitleBarDemo())
            .Code("""
                window.Title = "My app";

                if (AppWindowTitleBar.IsCustomizationSupported())
                {
                    var titleBar = window.AppWindow.TitleBar;
                    titleBar.ButtonBackgroundColor = Colors.DarkSlateBlue;
                    titleBar.ButtonForegroundColor = Colors.White;
                }
                """)
            .Tip("This app already extends its content into the title bar, so the demo only touches the title text and the caption button background. It puts both back when you leave.")
            .Find("window", "caption", "chrome", "appwindow", "extendscontentintotitlebar"),

        new ApiSample("acrylicbrush", "AcrylicBrush", Graphics, Symbol.Highlight, "Frosted glass that blurs whatever sits behind it.",
                ApiPlatforms.All & ~ApiPlatforms.Windows,
                "https://platform.uno/docs/articles/features/shapes-and-brushes.html#acrylicbrush", () => new AcrylicBrushDemo())
            .Height(560)
            .Xaml("""
                <Border CornerRadius="160" Width="320" Height="320">
                    <Border.Background>
                        <AcrylicBrush TintColor="Red"
                                      TintOpacity="0.8"
                                      TintLuminosityOpacity="0.3"
                                      FallbackColor="Green" />
                    </Border.Background>
                </Border>
                """)
            .Tip("The brush blurs in-app content only, and FallbackColor is what you get when the effect can't run.")
            .Find("blur", "glass", "backdrop", "tint", "transparency"),

        new ApiSample("datagrid", "DataGrid", Graphics, Symbol.List, "A sortable, editable table from the Windows Community Toolkit.",
                ApiPlatforms.All,
                "https://platform.uno/docs/articles/uno-community-toolkit.html", () => new DataGridDemo())
            .Height(560)
            .Xaml("""
                <!-- xmlns:wct="using:CommunityToolkit.WinUI.UI.Controls" -->

                <wct:DataGrid ItemsSource="{x:Bind Plants}"
                              AutoGenerateColumns="True"
                              HeadersVisibility="Column"
                              SelectionMode="Extended"
                              IsReadOnly="False" />
                """)
            .Tip("Add the Uno.CommunityToolkit.WinUI.UI.Controls.DataGrid package. On Windows the original CommunityToolkit.WinUI.UI.Controls.DataGrid takes its place.")
            .Find("table", "grid", "rows", "columns", "sort", "toolkit"),

        new ApiSample("glcanvaselement", "GLCanvasElement", Graphics, Symbol.Rotate, "Draws 3D graphics with raw OpenGL, here a spinning cube.",
                ApiPlatforms.Windows | ApiPlatforms.Desktop | ApiPlatforms.WebAssembly,
                "https://platform.uno/docs/articles/controls/GLCanvasElement.html", () => new GLCanvasElementDemo())
            .Code("""
                public class MyGlCanvasElement() : GLCanvasElement(() => App.MainWindow)
                {
                    protected override void Init(GL gl)
                    {
                        // Create buffers, shaders and textures
                    }

                    protected override void RenderOverride(GL gl)
                    {
                        gl.ClearColor(0.1f, 0.12f, 0.2f, 1);
                        gl.Clear(ClearBufferMask.ColorBufferBit);
                        Invalidate(); // keep animating
                    }

                    protected override void OnDestroy(GL gl)
                    {
                        // Delete what Init created
                    }
                }
                """)
            .Tip("Add the GLCanvas UnoFeature. It needs OpenGL, so it works on Windows, desktop and WebAssembly (WebGL) but not on mobile heads. The OpenGL calls come from Silk.NET.")
            .Find("opengl", "3d", "cube", "silk", "shader", "webgl"),

        new ApiSample("mapsui", "MapsUI", Graphics, Symbol.Map, "Pans and zooms an OpenStreetMap map, and can jump to where you are.",
                ApiPlatforms.Windows,
                "https://platform.uno/docs/articles/features/windows-devices-geolocation.html", () => new MapsUIDemo())
            .Code("""
                var mapControl = new Mapsui.UI.WinUI.MapControl();
                mapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());

                var center = SphericalMercator.FromLonLat(14.4378, 50.0755);
                mapControl.Map.Navigator.CenterOnAndZoomTo(
                    new MPoint(center),
                    mapControl.Map.Navigator.Resolutions[11]);
                """)
            .Tip("Needs the Mapsui.WinUI package and internet access for the tiles. Mapsui.Uno.WinUI 5.2 is built for Uno 6 and crashes the app at startup on Uno 7.1, so other heads wait for a rebuilt Mapsui.")
            .Find("openstreetmap", "osm", "geolocation", "location", "tiles"),

        new ApiSample("mediaplayerelement", "MediaPlayerElement", Graphics, Symbol.Video, "Plays a video with the standard transport controls.",
                ApiPlatforms.All,
                "https://platform.uno/docs/articles/controls/MediaPlayerElement.html", () => new MediaPlayerElementDemo())
            .Xaml("""
                <MediaPlayerElement
                    Source="ms-appx:///Assets/Media/sample.mp4"
                    AreTransportControlsEnabled="True"
                    AutoPlay="False" />
                """)
            .Tip("Add the MediaElement UnoFeature. Desktop playback goes through LibVLC, so codecs beyond the usual ones may need the system to provide them.")
            .Find("video", "audio", "mp4", "player", "media"),

        new ApiSample("skcanvaselement", "SKCanvasElement", Graphics, Symbol.Edit, "Draws 2D shapes, curves and gradients with SkiaSharp.",
                ApiPlatforms.All & ~ApiPlatforms.Windows,
                "https://platform.uno/docs/articles/controls/SKCanvasElement.html", () => new SKCanvasElementDemo())
            .Height(480)
            .Code("""
                public class MyCanvas : SKCanvasElement
                {
                    protected override void RenderOverride(SKCanvas canvas, Size area)
                    {
                        using SKPaint paint = new() { Color = SKColors.CornflowerBlue, IsAntialias = true };
                        canvas.DrawCircle((float)area.Width / 2, (float)area.Height / 2, 50, paint);
                    }
                }
                """)
            .Tip("It reuses the window's own Skia canvas, so it only works on heads that render with Skia. Check SKCanvasElement.IsSupportedOnCurrentPlatform(); the Windows App SDK head renders with WinUI.")
            .Find("skia", "skiasharp", "2d", "canvas", "drawing", "paint"),

        new ApiSample("webview", "WebView", Graphics, Symbol.World, "Shows a live web page inside the app.",
                ApiPlatforms.All,
                "https://platform.uno/docs/articles/controls/WebView.html", () => new WebViewDemo())
            .Height(520)
            .Xaml("""
                <WebView2 Source="https://platform.uno/" />
                """)
            .Tip("Add the WebView UnoFeature for the desktop head. In the browser it's an iframe, so sites that forbid framing (X-Frame-Options) stay blank. Mobile heads need the internet permission.")
            .Find("web", "browser", "html", "webview2", "iframe", "url"),
    ];
}
