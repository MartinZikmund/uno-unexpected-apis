using UnexpectedApis.Apis;
using UnexpectedApis.Core;

namespace UnexpectedApis.Catalog;

public static partial class SampleCatalog
{
    private static List<Sample> DeviceAndSystemApis() =>
    [
        new ApiSample("clipboard", "Clipboard", AppSystem, Symbol.Paste, "Copies, pastes and watches text on the system clipboard.",
                ApiPlatforms.All & ~ApiPlatforms.TvOS, "https://platform.uno/docs/articles/features/clipboard.html", () => new ClipboardDemo())
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

        new ApiSample("gamepad", "Gamepad", Device, Symbol.Play, "Shows every connected gamepad live: buttons, sticks and triggers.",
                ApiPlatforms.All & ~ApiPlatforms.Desktop, "https://platform.uno/docs/articles/features/gamepad.html", () => new GamepadDemo())
            .Code("""
                Gamepad.GamepadAdded += (s, gamepad) => { /* a pad showed up */ };

                var reading = Gamepad.Gamepads[0].GetCurrentReading();
                var buttons = reading.Buttons;
                var leftX = reading.LeftThumbstickX;
                var trigger = reading.LeftTrigger;
                """)
            .Tip("There is no input event: poll GetCurrentReading() on a timer, as the demo does every 100 ms.")
            .Tip("Gamepads only show up after a button press on some platforms, especially in the browser.")
            .Find("controller", "joystick", "xbox", "input"),

        new ApiSample("midi", "MIDI", Device, Symbol.Audio, "Plays notes on a connected MIDI device, keyboard keys included.",
                ApiPlatforms.All & ~ApiPlatforms.Desktop, "https://platform.uno/docs/articles/features/windows-devices-midi.html", () => new MidiDemo())
            .Code("""
                var selector = MidiOutPort.GetDeviceSelector();
                var devices = await DeviceInformation.FindAllAsync(selector);

                var port = await MidiOutPort.FromIdAsync(devices[0].Id);
                port.SendMessage(new MidiNoteOnMessage(channel: 0, note: 60, velocity: 127));
                """)
            .Tip("In the browser the page needs Web MIDI permission, and the browser must support it.")
            .Tip("Send a MidiNoteOffMessage afterwards or the note keeps ringing.")
            .Find("music", "note", "piano", "instrument", "sound"),

        new ApiSample("speech", "Speech recognizer", Device, Symbol.Microphone, "Turns what you say into text.",
                ApiPlatforms.All & ~ApiPlatforms.Desktop & ~ApiPlatforms.TvOS, "https://platform.uno/docs/articles/features/SpeechRecognition.html", () => new SpeechRecognizerDemo())
            .Code("""
                using var recognizer = new SpeechRecognizer(new Language("en-US"));
                recognizer.HypothesisGenerated += (s, e) => Debug.WriteLine(e.Hypothesis.Text);

                await recognizer.CompileConstraintsAsync();
                var result = await recognizer.RecognizeAsync();
                var text = result.Text;
                """)
            .Tip("It needs the microphone: RECORD_AUDIO on Android, NSMicrophoneUsageDescription and NSSpeechRecognitionUsageDescription on iOS.")
            .Find("voice", "dictation", "microphone", "listen"),

        new ApiSample("network", "Network information", Device, Symbol.World, "Tells you whether the device is online and when that changes.",
                ApiPlatforms.All, "https://platform.uno/docs/articles/features/windows-networking.html", () => new NetworkInformationDemo())
            .Code("""
                NetworkInformation.NetworkStatusChanged += s =>
                {
                    var profile = NetworkInformation.GetInternetConnectionProfile();
                    var level = profile?.GetNetworkConnectivityLevel();
                    if (level == NetworkConnectivityLevel.InternetAccess)
                    {
                        // Connected to the internet
                    }
                };
                """)
            .Tip("On Android the app needs the ACCESS_NETWORK_STATE permission.")
            .Find("online", "offline", "wifi", "connectivity", "internet"),

        new ApiSample("filesystem", "File system", AppSystem, Symbol.Folder, "Lists, creates and edits text files in a folder you pick.",
                ApiPlatforms.All, "https://platform.uno/docs/articles/features/windows-storage-pickers.html", () => new FileSystemDemo())
            .Code("""
                var picker = new FolderPicker { FileTypeFilter = { "*" } };
                InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(window)); // Windows only
                var folder = await picker.PickSingleFolderAsync();

                var files = await folder.GetFilesAsync();
                var file = await folder.CreateFileAsync("hello.txt", CreationCollisionOption.OpenIfExists);
                await FileIO.WriteTextAsync(file, "Hello world!");
                """)
            .Tip("On Windows, initialize the picker with the window handle first, or it throws.")
            .Tip("The folder picker is not supported in every browser; ApplicationData.Current.LocalFolder works everywhere.")
            .Find("folder", "file", "picker", "storage", "read", "write"),

        new ApiSample("launcher", "Launcher", AppSystem, Symbol.Go, "Opens a web page, a mail draft or any other URI in the app that handles it.",
                ApiPlatforms.All, "https://platform.uno/docs/articles/features/windows-system.html", () => new LauncherDemo())
            .Code("""
                var launched = await Launcher.LaunchUriAsync(new Uri("https://platform.uno/"));
                """)
            .Tip("On iOS, custom schemes you query or open must be declared in LSApplicationQueriesSchemes; on Android 11+ add a queries entry.")
            .Find("open", "browser", "url", "uri", "deep link", "mailto"),

        new ApiSample("sharing", "Sharing", AppSystem, Symbol.Share, "Opens the system share sheet with a link to the app.",
                ApiPlatforms.All & ~ApiPlatforms.Desktop & ~ApiPlatforms.TvOS, "https://platform.uno/docs/articles/features/windows-applicationmodel-datatransfer.html", () => new SharingDemo())
            .Code("""
                DataTransferManager.GetForCurrentView().DataRequested += (s, args) =>
                {
                    args.Request.Data.Properties.Title = "Unexpected APIs in Uno Platform";
                    args.Request.Data.Properties.Description = "Link to the app";
                    args.Request.Data.SetWebLink(new Uri("https://aka.platform.uno/demo-unexpected-apis"));
                };

                DataTransferManager.ShowShareUI();
                """)
            .Tip("In the browser, sharing relies on the Web Share API, which needs HTTPS and a user gesture.")
            .Find("share sheet", "send", "datatransfer", "link"),

        new ApiSample("badge", "Badge notification", AppSystem, Symbol.Important, "Puts a number on the app icon.",
                ApiPlatforms.Windows | ApiPlatforms.Apple, "https://platform.uno/docs/articles/features/windows-ui-notifications.html", () => new BadgeNotificationDemo())
            .Code("""
                var badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);
                var badgeElement = (XmlElement)badgeXml.SelectSingleNode("/badge");
                badgeElement.SetAttribute("value", "42");

                BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(new BadgeNotification(badgeXml));
                """)
            .Tip("iOS and tvOS need notification permission (UNAuthorizationOptions.Badge) before the badge shows; the demo asks on the first click.")
            .Find("icon", "counter", "unread", "notification"),
    ];
}



