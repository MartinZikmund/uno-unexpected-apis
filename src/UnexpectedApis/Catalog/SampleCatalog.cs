using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;

namespace UnexpectedApis.Catalog;

public static partial class SampleCatalog
{
    private const string AccentStyle = "AccentButtonStyle";

    public static readonly Category Actions = new("actions", "Actions", Symbol.Play, "Buttons and the things they open");
    public static readonly Category Input = new("input", "Input", Symbol.Edit, "Toggles, sliders and pickers");
    public static readonly Category Text = new("text", "Text", Symbol.Font, "Showing and typing words");
    public static readonly Category DateTime = new("datetime", "Date and time", Symbol.Calendar, "Calendars and pickers");
    public static readonly Category Status = new("status", "Status", Symbol.Important, "Progress, messages, tips");
    public static readonly Category Dialogs = new("dialogs", "Dialogs and flyouts", Symbol.Message, "Things that float above");
    public static readonly Category Collections = new("collections", "Collections", Symbol.List, "Lists, grids and trees");
    public static readonly Category Navigation = new("navigation", "Navigation", Symbol.Map, "Getting around an app");
    public static readonly Category Layout = new("layout", "Layout", Symbol.ViewAll, "Panels and containers");

    // Declared here, not in the partial files, so they exist before All is built.
    public static readonly Category Sensors = new("sensors", "Sensors", Symbol.Target, "Motion, light, heading and location");
    public static readonly Category Device = new("device", "Device", Symbol.CellPhone, "Hardware you can drive");
    public static readonly Category AppSystem = new("system", "App and system", Symbol.Setting, "Clipboard, files, sharing and the shell");
    public static readonly Category Graphics = new("graphics", "Graphics and media", Symbol.Pictures, "Canvases, maps, video and the web");

    public static IReadOnlyList<Category> ApiCategories { get; } = (Category[])[Sensors, Device, AppSystem, Graphics];

    public static IReadOnlyList<Category> ControlCategories { get; } =
        (Category[])[Actions, Input, Text, DateTime, Status, Dialogs, Collections, Navigation, Layout];

    public static IReadOnlyList<Category> Categories { get; } = (Category[])[.. ControlCategories, .. ApiCategories];

    public static IReadOnlyList<Sample> All { get; } = (Sample[])[.. Build(), .. BuildApis()];

    public static Sample? Find(string id) => All.FirstOrDefault(sample => sample.Id == id);

    private static List<Sample> Build() =>
    [
        // Actions
        new Sample<Button>("button", "Button", Actions, Symbol.TouchPointer,"Starts an immediate action.", () => new Button())
            .Text("Content", "Save changes", (c, v) => c.Content = v)
            .Choice("Style", "Default", ["Default", "Accent"],
                (c, v) => c.Style = (Style)Application.Current.Resources[v == "Accent" ? AccentStyle : "DefaultButtonStyle"],
                _ => "{StaticResource " + AccentStyle + "}")
            .Flag("IsEnabled", true, (c, v) => c.IsEnabled = v)
            .Tip("Use one accent button per view: it marks the primary action, and two of them mark nothing.")
            .Find("click", "command", "accent"),

        new Sample<ToggleButton>("togglebutton", "ToggleButton", Actions, Symbol.Pin, "A button that stays pressed.", () => new ToggleButton())
            .Text("Content", "Pin to top", (c, v) => c.Content = v)
            .Flag("IsChecked", false, (c, v) => c.IsChecked = v)
            .Flag("IsThreeState", false, (c, v) => c.IsThreeState = v)
            .Flag("IsEnabled", true, (c, v) => c.IsEnabled = v)
            .Find("pressed", "latched"),

        new Sample<HyperlinkButton>("hyperlinkbutton", "HyperlinkButton", Actions, Symbol.Link, "A button that reads as a link.", () => new HyperlinkButton())
            .Text("Content", "Read the docs", (c, v) => c.Content = v)
            .Flag("IsEnabled", true, (c, v) => c.IsEnabled = v)
            .Attr("NavigateUri", "https://platform.uno/docs")
            .Find("link", "url", "navigate"),

        new Sample<RepeatButton>("repeatbutton", "RepeatButton", Actions, Symbol.Refresh, "Fires over and over while held.", () => new RepeatButton())
            .Text("Content", "Hold me", (c, v) => c.Content = v)
            .Number("Delay", 250, 0, 1000, 50, (c, v) => c.Delay = (int)v)
            .Number("Interval", 100, 10, 500, 10, (c, v) => c.Interval = (int)v)
            .Find("hold", "press and hold"),

        new Sample<DropDownButton>("dropdownbutton", "DropDownButton", Actions, Symbol.More, "A button that opens a menu.", () => new DropDownButton { Flyout = SortMenu() })
            .Text("Content", "Sort by", (c, v) => c.Content = v)
            .Flag("IsEnabled", true, (c, v) => c.IsEnabled = v)
            .Body("<DropDownButton.Flyout>", "    <MenuFlyout>", "        <MenuFlyoutItem Text=\"Name\" />", "        <MenuFlyoutItem Text=\"Date\" />", "    </MenuFlyout>", "</DropDownButton.Flyout>")
            .Find("menu", "chevron"),

        new Sample<SplitButton>("splitbutton", "SplitButton", Actions, Symbol.Share, "A default action, plus a menu of the others.", () => new SplitButton { Flyout = SortMenu() })
            .Text("Content", "Export", (c, v) => c.Content = v)
            .Flag("IsEnabled", true, (c, v) => c.IsEnabled = v)
            .Body("<SplitButton.Flyout>", "    <MenuFlyout>", "        <MenuFlyoutItem Text=\"Name\" />", "        <MenuFlyoutItem Text=\"Date\" />", "    </MenuFlyout>", "</SplitButton.Flyout>")
            .Find("menu", "default action"),

        new Sample<CommandBar>("commandbar", "CommandBar", Actions, Symbol.Setting, "A toolbar that overflows gracefully.", CreateCommandBar)
            .Flag("IsOpen", false, (c, v) => c.IsOpen = v)
            .Enum("DefaultLabelPosition", CommandBarDefaultLabelPosition.Bottom, (c, v) => c.DefaultLabelPosition = v)
            .Enum("OverflowButtonVisibility", CommandBarOverflowButtonVisibility.Auto, (c, v) => c.OverflowButtonVisibility = v)
            .Body("<AppBarButton Icon=\"Add\" Label=\"New\" />", "<AppBarButton Icon=\"Edit\" Label=\"Edit\" />", "<AppBarButton Icon=\"Delete\" Label=\"Delete\" />", "<CommandBar.SecondaryCommands>", "    <AppBarButton Label=\"Archive\" />", "</CommandBar.SecondaryCommands>")
            .Tip("AppBarButton only belongs inside an AppBar or CommandBar. Anywhere else, use a Button with an icon.")
            .Find("toolbar", "appbar", "ribbon"),

        // Input
        new Sample<CheckBox>("checkbox", "CheckBox", Input, Symbol.Accept, "Picks any number of independent options.", () => new CheckBox())
            .Text("Content", "Email me about updates", (c, v) => c.Content = v)
            .Flag("IsChecked", false, (c, v) => c.IsChecked = v)
            .Flag("IsThreeState", false, (c, v) => c.IsThreeState = v)
            .Flag("IsEnabled", true, (c, v) => c.IsEnabled = v)
            .Tip("If the change applies immediately, a ToggleSwitch says so better. A CheckBox implies a later Save.")
            .Find("tick", "boolean", "tristate"),

        new Sample<RadioButtons>("radiobuttons", "RadioButtons", Input, Symbol.List, "Picks exactly one of a few visible options.", CreateRadioButtons)
            .Text("Header", "Delivery", (c, v) => c.Header = v)
            .Number("MaxColumns", 1, 1, 3, 1, (c, v) => c.MaxColumns = (int)v)
            .Flag("IsEnabled", true, (c, v) => c.IsEnabled = v)
            .Attr("SelectedIndex", "0")
            .Body("<x:String>Standard</x:String>", "<x:String>Express</x:String>", "<x:String>Pick up</x:String>")
            .Tip("Five or more options? A ComboBox will cost you less screen.")
            .Find("option", "single choice", "group"),

        new Sample<ToggleSwitch>("toggleswitch", "ToggleSwitch", Input, Symbol.Switch, "Turns something on or off, right now.", () => new ToggleSwitch())
            .Text("Header", "Wi-Fi", (c, v) => c.Header = v)
            .Text("OnContent", "Connected", (c, v) => c.OnContent = v)
            .Text("OffContent", "Disconnected", (c, v) => c.OffContent = v)
            .Flag("IsOn", true, (c, v) => c.IsOn = v)
            .Flag("IsEnabled", true, (c, v) => c.IsEnabled = v)
            .Find("boolean", "on off", "setting"),

        new Sample<Slider>("slider", "Slider", Input, Symbol.Volume, "Picks a value from a range by feel.", () => new Slider { MinWidth = 280 })
            .Text("Header", "Volume", (c, v) => c.Header = v)
            .Number("Value", 40, 0, 100, 1, (c, v) => c.Value = v)
            .Number("StepFrequency", 1, 1, 25, 1, (c, v) => c.StepFrequency = v)
            .Number("TickFrequency", 0, 0, 25, 5, (c, v) => c.TickFrequency = v)
            .Enum("TickPlacement", TickPlacement.None, (c, v) => c.TickPlacement = v)
            .Enum("Orientation", Orientation.Horizontal, (c, v) =>
            {
                c.Orientation = v;
                c.MinHeight = v == Orientation.Vertical ? 200 : 0;
            })
            .Flag("IsEnabled", true, (c, v) => c.IsEnabled = v)
            .Tip("When the exact number matters, pair it with a NumberBox. Nobody lands on 37 by dragging.")
            .Find("range", "volume", "track"),

        new Sample<RatingControl>("rating", "RatingControl", Input, Symbol.Favorite, "Shows or collects a star rating.", () => new RatingControl())
            .Number("Value", 3, 0, 5, 0.5, (c, v) => c.Value = v)
            .Number("MaxRating", 5, 1, 10, 1, (c, v) => c.MaxRating = (int)v)
            .Flag("IsClearEnabled", true, (c, v) => c.IsClearEnabled = v)
            .Flag("IsReadOnly", false, (c, v) => c.IsReadOnly = v)
            .Find("stars", "review", "score"),

        new Sample<ColorPicker>("colorpicker", "ColorPicker", Input, Symbol.FontColor, "Picks any color, with a spectrum and sliders.", () => new ColorPicker())
            .Flag("IsAlphaEnabled", false, (c, v) => c.IsAlphaEnabled = v)
            .Flag("IsMoreButtonVisible", false, (c, v) => c.IsMoreButtonVisible = v)
            .Flag("IsColorSliderVisible", true, (c, v) => c.IsColorSliderVisible = v)
            .Flag("IsColorChannelTextInputVisible", true, (c, v) => c.IsColorChannelTextInputVisible = v)
            .Enum("ColorSpectrumShape", ColorSpectrumShape.Box, (c, v) => c.ColorSpectrumShape = v)
            .Fill(560)
            .Find("palette", "hex", "rgb", "spectrum"),

        new Sample<ComboBox>("combobox", "ComboBox", Input, Symbol.List, "Picks one of many from a compact list.", () => new ComboBox { ItemsSource = new[] { "Small", "Medium", "Large" }, SelectedIndex = 1, MinWidth = 200 })
            .Text("Header", "Size", (c, v) => c.Header = v)
            .Text("PlaceholderText", "Pick a size", (c, v) => c.PlaceholderText = v, alwaysEmit: false)
            .Flag("IsEditable", false, (c, v) => c.IsEditable = v)
            .Flag("IsEnabled", true, (c, v) => c.IsEnabled = v)
            .Attr("SelectedIndex", "1")
            .Body("<x:String>Small</x:String>", "<x:String>Medium</x:String>", "<x:String>Large</x:String>")
            .Find("dropdown", "select", "picker"),

        // Text
        new Sample<TextBlock>("textblock", "TextBlock", Text, Symbol.Font, "Displays read-only text.", () => new TextBlock { MaxWidth = 420 })
            .Text("Text", "Good typography is invisible. Bad typography is everywhere.", (c, v) => c.Text = v)
            .Choice("Style", "BodyTextBlockStyle", ["CaptionTextBlockStyle", "BodyTextBlockStyle", "BodyStrongTextBlockStyle", "SubtitleTextBlockStyle", "TitleTextBlockStyle"],
                (c, v) => c.Style = (Style)Application.Current.Resources[v],
                v => "{StaticResource " + v + "}")
            .Enum("TextWrapping", TextWrapping.Wrap, (c, v) => c.TextWrapping = v)
            .Enum("TextTrimming", TextTrimming.None, (c, v) => c.TextTrimming = v)
            .Flag("IsTextSelectionEnabled", false, (c, v) => c.IsTextSelectionEnabled = v)
            .Tip("Reach for the type ramp styles over a raw FontSize. Open Typography from the sidebar for the whole set.")
            .Find("label", "typography", "style"),

        new Sample<TextBox>("textbox", "TextBox", Text, Symbol.Edit, "Takes a line, or several, of plain text.", () => new TextBox { MinWidth = 320 })
            .Text("Header", "Display name", (c, v) => c.Header = v)
            .Text("PlaceholderText", "Ada Lovelace", (c, v) => c.PlaceholderText = v)
            .Flag("AcceptsReturn", false, (c, v) => c.AcceptsReturn = v)
            .Flag("IsReadOnly", false, (c, v) => c.IsReadOnly = v)
            .Flag("IsSpellCheckEnabled", false, (c, v) => c.IsSpellCheckEnabled = v)
            .Enum("TextWrapping", TextWrapping.NoWrap, (c, v) => c.TextWrapping = v)
            .Tip("With x:Bind TwoWay, add UpdateSourceTrigger=PropertyChanged, or the view model only hears about it on focus loss.")
            .Find("input", "form", "field", "multiline"),

        new Sample<PasswordBox>("passwordbox", "PasswordBox", Text, Symbol.Permissions, "Takes a secret, with an optional reveal.", () => new PasswordBox { MinWidth = 320 })
            .Text("Header", "Password", (c, v) => c.Header = v)
            .Text("PlaceholderText", "At least 12 characters", (c, v) => c.PlaceholderText = v)
            .Enum("PasswordRevealMode", PasswordRevealMode.Peek, (c, v) => c.PasswordRevealMode = v)
            .Find("secret", "login", "reveal"),

        new Sample<NumberBox>("numberbox", "NumberBox", Text, Symbol.Calculator, "Takes a number, and can do the math typed into it.", () => new NumberBox { MinWidth = 200 })
            .Text("Header", "Quantity", (c, v) => c.Header = v)
            .Number("Value", 3, 0, 100, 1, (c, v) => c.Value = v)
            .Number("Minimum", 0, 0, 10, 1, (c, v) => c.Minimum = v)
            .Number("Maximum", 100, 10, 100, 10, (c, v) => c.Maximum = v)
            .Enum("SpinButtonPlacementMode", NumberBoxSpinButtonPlacementMode.Inline, (c, v) => c.SpinButtonPlacementMode = v)
            .Flag("AcceptsExpression", true, (c, v) => c.AcceptsExpression = v)
            .Tip("With AcceptsExpression on, typing 4*12 commits 48. It looks like magic and costs one attribute.")
            .Find("numeric", "spinner", "integer", "calculator"),

        new Sample<AutoSuggestBox>("autosuggestbox", "AutoSuggestBox", Text, Symbol.Find, "A search box that suggests as you type.", CreateAutoSuggest)
            .Text("Header", "Find a city", (c, v) => c.Header = v)
            .Text("PlaceholderText", "Try Prague", (c, v) => c.PlaceholderText = v)
            .Flag("IsEnabled", true, (c, v) => c.IsEnabled = v)
            .Attr("QueryIcon", "Find")
            .Tip("Only rebuild suggestions when args.Reason is UserInput. Otherwise picking one kicks off another search.")
            .Find("search", "typeahead", "completion"),

        new Sample<RichEditBox>("richeditbox", "RichEditBox", Text, Symbol.Document, "Edits formatted text.", () => new RichEditBox { MinWidth = 360, MinHeight = 120 })
            .Text("Header", "Notes", (c, v) => c.Header = v)
            .Text("PlaceholderText", "Write something worth formatting", (c, v) => c.PlaceholderText = v)
            .Flag("IsReadOnly", false, (c, v) => c.IsReadOnly = v)
            .Find("formatted", "rtf", "editor"),

        // Date and time
        new Sample<CalendarDatePicker>("calendardatepicker", "CalendarDatePicker", DateTime, Symbol.Calendar, "Picks a date from a dropdown calendar.", () => new CalendarDatePicker())
            .Text("Header", "Start date", (c, v) => c.Header = v)
            .Text("PlaceholderText", "Pick a date", (c, v) => c.PlaceholderText = v)
            .Flag("IsTodayHighlighted", true, (c, v) => c.IsTodayHighlighted = v)
            .Flag("IsEnabled", true, (c, v) => c.IsEnabled = v)
            .Tip("The default for a date near today. Reach for DatePicker when the date is far away, like a birthday.")
            .Find("date", "calendar", "day"),

        new Sample<DatePicker>("datepicker", "DatePicker", DateTime, Symbol.CalendarDay, "Picks a date you already know.", () => new DatePicker())
            .Text("Header", "Date of birth", (c, v) => c.Header = v)
            .Flag("DayVisible", true, (c, v) => c.DayVisible = v)
            .Flag("MonthVisible", true, (c, v) => c.MonthVisible = v)
            .Flag("YearVisible", true, (c, v) => c.YearVisible = v)
            .Find("date", "birthday", "spinner"),

        new Sample<TimePicker>("timepicker", "TimePicker", DateTime, Symbol.Clock, "Picks a time of day.", () => new TimePicker())
            .Text("Header", "Reminder", (c, v) => c.Header = v)
            .Number("MinuteIncrement", 5, 1, 30, 1, (c, v) => c.MinuteIncrement = (int)v)
            .Choice("ClockIdentifier", "24HourClock", ["12HourClock", "24HourClock"], (c, v) => c.ClockIdentifier = v)
            .Find("time", "clock", "hour"),

        new Sample<CalendarView>("calendarview", "CalendarView", DateTime, Symbol.CalendarWeek, "An always-visible month to pick from.", () => new CalendarView())
            .Enum("SelectionMode", CalendarViewSelectionMode.Single, (c, v) => c.SelectionMode = v)
            .Enum("DisplayMode", CalendarViewDisplayMode.Month, (c, v) => c.DisplayMode = v)
            .Flag("IsTodayHighlighted", true, (c, v) => c.IsTodayHighlighted = v)
            .Flag("IsOutOfScopeEnabled", true, (c, v) => c.IsOutOfScopeEnabled = v)
            .Fill(420)
            .Find("month", "date range", "agenda"),

        // Status
        new Sample<ProgressBar>("progressbar", "ProgressBar", Status, Symbol.Sync, "Shows how far along something is.", () => new ProgressBar { MinWidth = 320 })
            .Number("Value", 60, 0, 100, 1, (c, v) => c.Value = v)
            .Flag("IsIndeterminate", false, (c, v) => c.IsIndeterminate = v)
            .Flag("ShowPaused", false, (c, v) => c.ShowPaused = v)
            .Flag("ShowError", false, (c, v) => c.ShowError = v)
            .Find("loading", "percent", "busy"),

        new Sample<ProgressRing>("progressring", "ProgressRing", Status, Symbol.Refresh, "Says something is happening, compactly.", () => new ProgressRing { Width = 64, Height = 64 })
            .Flag("IsActive", true, (c, v) => c.IsActive = v)
            .Flag("IsIndeterminate", true, (c, v) => c.IsIndeterminate = v)
            .Number("Value", 40, 0, 100, 1, (c, v) => c.Value = v)
            .Attr("Width", "64").Attr("Height", "64")
            .Find("spinner", "loading", "busy"),

        new Sample<InfoBar>("infobar", "InfoBar", Status, Symbol.Important, "An inline, app-wide message that stays until dealt with.", () => new InfoBar { IsOpen = true })
            .Text("Title", "Update available", (c, v) => c.Title = v)
            .Text("Message", "Restart to finish installing version 7.0.", (c, v) => c.Message = v)
            .Enum("Severity", InfoBarSeverity.Informational, (c, v) => c.Severity = v)
            .Flag("IsOpen", true, (c, v) => c.IsOpen = v)
            .Flag("IsClosable", true, (c, v) => c.IsClosable = v)
            .Flag("IsIconVisible", true, (c, v) => c.IsIconVisible = v)
            .Find("message", "banner", "alert", "warning", "error"),

        new Sample<InfoBadge>("infobadge", "InfoBadge", Status, Symbol.Flag, "A tiny count or dot that asks for attention.", () => new InfoBadge())
            .Number("Value", 7, -1, 99, 1, (c, v) => c.Value = (int)v)
            .Tip("A Value of -1 draws the bare dot, which suits 'something is new' without a number to keep accurate.")
            .Find("notification", "count", "dot"),

        new Sample<Button>("tooltip", "ToolTip", Status, Symbol.Help, "Explains something on hover or focus.", CreateTooltipHost, tag: "Button")
            .Snippet("<Button Content=\"Hover me\"\n        ToolTipService.ToolTip=\"Saves without asking\" />")
            .Tip("Icon-only buttons need a tooltip and an AutomationProperties.Name. One helps eyes, the other screen readers.")
            .Find("hover", "hint", "tooltipservice"),

        new Sample<PersonPicture>("personpicture", "PersonPicture", Status, Symbol.Contact, "An avatar, with initials when there's no photo.", () => new PersonPicture())
            .Text("DisplayName", "Ada Lovelace", (c, v) => c.DisplayName = v)
            .Number("Width", 96, 32, 160, 8, (c, v) => c.Width = v)
            .Number("Height", 96, 32, 160, 8, (c, v) => c.Height = v)
            .Flag("IsGroup", false, (c, v) => c.IsGroup = v)
            .Number("BadgeNumber", 0, 0, 99, 1, (c, v) => c.BadgeNumber = (int)v)
            .Find("avatar", "profile", "initials", "contact"),

        // Dialogs and flyouts
        new Sample<Button>("contentdialog", "ContentDialog", Dialogs, Symbol.Message, "Stops everything for a decision.", CreateDialogHost, tag: "Button")
            .Snippet("<ContentDialog x:Name=\"ConfirmDialog\"\n               Title=\"Discard changes?\"\n               Content=\"You'll lose what you typed.\"\n               PrimaryButtonText=\"Discard\"\n               CloseButtonText=\"Keep editing\"\n               DefaultButton=\"Close\" />")
            .Tip("Set XamlRoot before ShowAsync when you create the dialog in code, or it throws.")
            .Tip("Label buttons with what they do. 'Discard' and 'Keep editing' beat 'Yes' and 'No' every time.")
            .Find("modal", "confirm", "popup", "alert"),

        new Sample<Button>("flyout", "Flyout", Dialogs, Symbol.Comment, "Light-dismiss content anchored to a control.", CreateFlyoutHost, tag: "Button")
            .Snippet("<Button Content=\"Empty cart\">\n    <Button.Flyout>\n        <Flyout>\n            <StackPanel Spacing=\"12\">\n                <TextBlock Text=\"Remove all items?\" />\n                <Button Content=\"Yes, empty it\" />\n            </StackPanel>\n        </Flyout>\n    </Button.Flyout>\n</Button>")
            .Find("popover", "popup", "light dismiss"),

        new Sample<Button>("menuflyout", "MenuFlyout", Dialogs, Symbol.Bullets, "Commands, sub-menus, separators and checks.", CreateMenuHost, tag: "Button")
            .Snippet("<Button Content=\"Options\">\n    <Button.Flyout>\n        <MenuFlyout>\n            <MenuFlyoutItem Icon=\"Copy\" Text=\"Copy\" />\n            <ToggleMenuFlyoutItem Text=\"Word wrap\" IsChecked=\"True\" />\n            <MenuFlyoutSeparator />\n            <MenuFlyoutSubItem Text=\"Share\">\n                <MenuFlyoutItem Text=\"Email\" />\n            </MenuFlyoutSubItem>\n        </MenuFlyout>\n    </Button.Flyout>\n</Button>")
            .Find("context menu", "right click", "commands"),

        new Sample<Button>("teachingtip", "TeachingTip", Dialogs, Symbol.Help, "Points at a feature to explain it once.", CreateTeachingTipHost, tag: "Button")
            .Snippet("<Button x:Name=\"TipButton\" Content=\"Show the tip\" />\n<TeachingTip Target=\"{x:Bind TipButton}\"\n             Title=\"Autosave is on\"\n             Subtitle=\"Your work saves every 30 seconds.\"\n             IsOpen=\"True\" />")
            .Find("coachmark", "onboarding", "hint"),

        // Collections
        new Sample<ListView>("listview", "ListView", Collections, Symbol.List, "A virtualized vertical list.", () => new ListView { ItemsSource = Enumerable.Range(1, 40).Select(i => $"Invoice #{1000 + i}").ToList() })
            .Enum("SelectionMode", ListViewSelectionMode.Single, (c, v) => c.SelectionMode = v)
            .Flag("IsItemClickEnabled", false, (c, v) => c.IsItemClickEnabled = v)
            .Flag("CanReorderItems", false, (c, v) => c.CanReorderItems = v)
            .Attr("ItemsSource", "{x:Bind Invoices}")
            .Fill()
            .Tip("Never wrap a ListView in a ScrollViewer. It gives the list infinite height and virtualization quietly dies.")
            .Find("items", "rows", "master detail", "virtualized"),

        new Sample<GridView>("gridview", "GridView", Collections, Symbol.ViewAll, "A virtualized wrapping grid of tiles.", CreateGridView)
            .Enum("SelectionMode", ListViewSelectionMode.Single, (c, v) => c.SelectionMode = v)
            .Flag("IsItemClickEnabled", false, (c, v) => c.IsItemClickEnabled = v)
            .Attr("ItemsSource", "{x:Bind Tiles}")
            .Fill()
            .Find("tiles", "thumbnails", "wrap"),

        new Sample<TreeView>("treeview", "TreeView", Collections, Symbol.ShowBcc, "A hierarchy you can fold.", CreateTreeView)
            .Enum("SelectionMode", TreeViewSelectionMode.Single, (c, v) => c.SelectionMode = v)
            .Flag("CanDragItems", true, (c, v) => c.CanDragItems = v)
            .Body("<TreeView.RootNodes>", "    <TreeViewNode Content=\"Documents\" IsExpanded=\"True\">", "        <TreeViewNode.Children>", "            <TreeViewNode Content=\"Invoices\" />", "        </TreeViewNode.Children>", "    </TreeViewNode>", "</TreeView.RootNodes>")
            .Fill()
            .Find("hierarchy", "folders", "explorer", "nested"),

        new Sample<FlipView>("flipview", "FlipView", Collections, Symbol.Pictures, "Pages through items one at a time.", CreateFlipView)
            .Flag("UseTouchAnimationsForAllNavigation", true, (c, v) => c.UseTouchAnimationsForAllNavigation = v)
            .Attr("ItemsSource", "{x:Bind Slides}")
            .Fill(280)
            .Find("carousel", "gallery", "slides"),

        // Navigation
        new Sample<NavigationView>("navigationview", "NavigationView", Navigation, Symbol.GlobalNavigationButton, "The top-level structure of an app.", CreateNavigationView)
            .Enum("PaneDisplayMode", NavigationViewPaneDisplayMode.Left, (c, v) => c.PaneDisplayMode = v)
            .Flag("IsPaneOpen", true, (c, v) => c.IsPaneOpen = v)
            .Flag("IsSettingsVisible", true, (c, v) => c.IsSettingsVisible = v)
            .Flag("IsPaneToggleButtonVisible", true, (c, v) => c.IsPaneToggleButtonVisible = v)
            .Number("OpenPaneLength", 240, 160, 360, 20, (c, v) => c.OpenPaneLength = v)
            .Body("<NavigationView.MenuItems>", "    <NavigationViewItem Icon=\"Home\" Content=\"Home\" />", "    <NavigationViewItem Icon=\"Mail\" Content=\"Inbox\" />", "    <NavigationViewItem Icon=\"Calendar\" Content=\"Calendar\" />", "</NavigationView.MenuItems>")
            .Fill(420)
            .Tip("Have 2 to 7 top sections. Past that, group them, or nobody finds the eighth.")
            .Find("sidebar", "hamburger", "menu", "pane"),

        new Sample<TabView>("tabview", "TabView", Navigation, Symbol.BrowsePhotos, "Document tabs the user can add and close.", CreateTabView)
            .Flag("IsAddTabButtonVisible", true, (c, v) => c.IsAddTabButtonVisible = v)
            .Flag("CanReorderTabs", true, (c, v) => c.CanReorderTabs = v)
            .Enum("TabWidthMode", TabViewWidthMode.Equal, (c, v) => c.TabWidthMode = v)
            .Enum("CloseButtonOverlayMode", TabViewCloseButtonOverlayMode.Auto, (c, v) => c.CloseButtonOverlayMode = v)
            .Body("<TabViewItem Header=\"Report.xaml\" />", "<TabViewItem Header=\"Styles.xaml\" />")
            .Fill(280)
            .Find("documents", "browser", "tabs"),

        new Sample<BreadcrumbBar>("breadcrumbbar", "BreadcrumbBar", Navigation, Symbol.Forward, "Shows the path, and lets you climb it.", () => new BreadcrumbBar { ItemsSource = new[] { "Home", "Documents", "Invoices", "2026" } })
            .Attr("ItemsSource", "{x:Bind Path}")
            .Find("path", "trail", "hierarchy"),

        new Sample<SelectorBar>("selectorbar", "SelectorBar", Navigation, Symbol.Sort, "Switches between a few views of the same data.", CreateSelectorBar)
            .Body("<SelectorBarItem Text=\"Recent\" IsSelected=\"True\" />", "<SelectorBarItem Text=\"Shared\" />", "<SelectorBarItem Text=\"Favorites\" />")
            .Find("segmented", "tabs", "switcher", "modes"),

        new Sample<Pivot>("pivot", "Pivot", Navigation, Symbol.ViewAll, "Swipeable sections with text headers.", CreatePivot)
            .Text("Title", "Mail", (c, v) => c.Title = v)
            .Body("<PivotItem Header=\"All\">", "    <TextBlock Text=\"Everything\" />", "</PivotItem>", "<PivotItem Header=\"Unread\">", "    <TextBlock Text=\"Only the new ones\" />", "</PivotItem>")
            .Fill(280)
            .Find("swipe", "sections", "tabs"),

        // Layout
        new Sample<Expander>("expander", "Expander", Layout, Symbol.Up, "Hides detail until someone asks.", () => new Expander { Content = new TextBlock { Text = "Everything you were not ready for." }, MinWidth = 360 })
            .Text("Header", "Advanced options", (c, v) => c.Header = v)
            .Flag("IsExpanded", true, (c, v) => c.IsExpanded = v)
            .Enum("ExpandDirection", ExpandDirection.Down, (c, v) => c.ExpandDirection = v)
            .Body("<TextBlock Text=\"Everything you were not ready for.\" />")
            .Find("collapse", "accordion", "disclosure"),

        new Sample<StackPanel>("stackpanel", "StackPanel", Layout, Symbol.AlignLeft, "Stacks children in a line.", () => CreateStack())
            .Enum("Orientation", Orientation.Vertical, (c, v) => c.Orientation = v)
            .Number("Spacing", 8, 0, 48, 4, (c, v) => c.Spacing = v)
            .Enum("HorizontalAlignment", HorizontalAlignment.Center, (c, v) => c.HorizontalAlignment = v)
            .Body("<Button Content=\"One\" />", "<Button Content=\"Two\" />", "<Button Content=\"Three\" />")
            .Tip("Spacing beats a Margin on every child. Keep it on the 4px grid: 4, 8, 12, 16, 24.")
            .Find("panel", "column", "row", "spacing"),

        new Sample<Grid>("grid", "Grid", Layout, Symbol.ViewAll, "Rows and columns: the layout that does the heavy lifting.", CreateGrid)
            .Number("RowSpacing", 8, 0, 48, 4, (c, v) => c.RowSpacing = v)
            .Number("ColumnSpacing", 8, 0, 48, 4, (c, v) => c.ColumnSpacing = v)
            .Body("<Grid.RowDefinitions>", "    <RowDefinition Height=\"Auto\" />", "    <RowDefinition Height=\"*\" />", "</Grid.RowDefinitions>", "<Grid.ColumnDefinitions>", "    <ColumnDefinition Width=\"240\" />", "    <ColumnDefinition Width=\"*\" />", "</Grid.ColumnDefinitions>", "<Border Grid.ColumnSpan=\"2\" />", "<Border Grid.Row=\"1\" />", "<Border Grid.Row=\"1\" Grid.Column=\"1\" />")
            .Fill()
            .Tip("Use RowSpacing and ColumnSpacing rather than empty spacer rows.")
            .Find("table", "rows", "columns", "panel"),

        new Sample<SplitView>("splitview", "SplitView", Layout, Symbol.OpenPane, "A pane that can overlay or push the content.", CreateSplitView)
            .Flag("IsPaneOpen", true, (c, v) => c.IsPaneOpen = v)
            .Enum("DisplayMode", SplitViewDisplayMode.Inline, (c, v) => c.DisplayMode = v)
            .Enum("PanePlacement", SplitViewPanePlacement.Left, (c, v) => c.PanePlacement = v)
            .Number("OpenPaneLength", 200, 120, 320, 20, (c, v) => c.OpenPaneLength = v)
            .Body("<SplitView.Pane>", "    <TextBlock Text=\"Pane\" />", "</SplitView.Pane>", "<TextBlock Text=\"Content\" />")
            .Fill(320)
            .Find("drawer", "sidebar", "overlay"),

        new Sample<ScrollViewer>("scrollviewer", "ScrollViewer", Layout, Symbol.Scan, "Scrolls and zooms oversized content.", CreateScrollViewer)
            .Enum("HorizontalScrollBarVisibility", ScrollBarVisibility.Auto, (c, v) => c.HorizontalScrollBarVisibility = v)
            .Enum("VerticalScrollBarVisibility", ScrollBarVisibility.Auto, (c, v) => c.VerticalScrollBarVisibility = v)
            .Enum("ZoomMode", ZoomMode.Disabled, (c, v) => c.ZoomMode = v)
            .Fill()
            .Find("scroll", "zoom", "pan", "overflow"),
    ];

    private static MenuFlyout SortMenu()
    {
        MenuFlyout menu = new();
        menu.Items.Add(new MenuFlyoutItem { Text = "Name" });
        menu.Items.Add(new MenuFlyoutItem { Text = "Date" });
        return menu;
    }

    private static CommandBar CreateCommandBar()
    {
        CommandBar bar = new() { Background = null };
        bar.PrimaryCommands.Add(new AppBarButton { Icon = new SymbolIcon(Symbol.Add), Label = "New" });
        bar.PrimaryCommands.Add(new AppBarButton { Icon = new SymbolIcon(Symbol.Edit), Label = "Edit" });
        bar.PrimaryCommands.Add(new AppBarButton { Icon = new SymbolIcon(Symbol.Delete), Label = "Delete" });
        bar.SecondaryCommands.Add(new AppBarButton { Label = "Archive" });
        return bar;
    }

    private static RadioButtons CreateRadioButtons()
    {
        RadioButtons group = new() { SelectedIndex = 0 };
        group.Items.Add("Standard");
        group.Items.Add("Express");
        group.Items.Add("Pick up");
        return group;
    }

    private static AutoSuggestBox CreateAutoSuggest()
    {
        string[] cities = ["Prague", "Brno", "Ostrava", "Oslo", "Lisbon", "Linz", "Paris", "Porto"];
        AutoSuggestBox box = new() { MinWidth = 320, QueryIcon = new SymbolIcon(Symbol.Find) };
        box.TextChanged += (sender, args) =>
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = cities.Where(city => city.Contains(sender.Text, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        };
        return box;
    }

    private static Button CreateTooltipHost()
    {
        Button button = new() { Content = "Hover me" };
        ToolTipService.SetToolTip(button, "Saves without asking");
        return button;
    }

    private static Button CreateDialogHost()
    {
        Button button = new() { Content = "Discard changes" };
        button.Click += async (_, _) =>
        {
            ContentDialog dialog = new()
            {
                XamlRoot = button.XamlRoot,
                RequestedTheme = button.ActualTheme,
                Title = "Discard changes?",
                Content = "You'll lose what you typed.",
                PrimaryButtonText = "Discard",
                CloseButtonText = "Keep editing",
                DefaultButton = ContentDialogButton.Close,
            };
            await dialog.ShowAsync();
        };
        return button;
    }

    private static Button CreateFlyoutHost()
    {
        StackPanel content = new() { Spacing = 12 };
        content.Children.Add(new TextBlock { Text = "Remove all items?" });
        Flyout flyout = new() { Content = content };
        Button confirm = new() { Content = "Yes, empty it" };
        confirm.Click += (_, _) => flyout.Hide();
        content.Children.Add(confirm);
        return new Button { Content = "Empty cart", Flyout = flyout };
    }

    private static Button CreateMenuHost()
    {
        MenuFlyout menu = new();
        menu.Items.Add(new MenuFlyoutItem { Text = "Copy", Icon = new SymbolIcon(Symbol.Copy) });
        menu.Items.Add(new ToggleMenuFlyoutItem { Text = "Word wrap", IsChecked = true });
        menu.Items.Add(new MenuFlyoutSeparator());
        MenuFlyoutSubItem share = new() { Text = "Share" };
        share.Items.Add(new MenuFlyoutItem { Text = "Email" });
        menu.Items.Add(share);
        return new Button { Content = "Options", Flyout = menu };
    }

    private static Button CreateTeachingTipHost()
    {
        Button button = new() { Content = "Show the tip" };
        TeachingTip tip = new()
        {
            Target = button,
            Title = "Autosave is on",
            Subtitle = "Your work saves every 30 seconds.",
        };
        button.Loaded += (_, _) =>
        {
            if (button.Parent is Panel panel && !panel.Children.Contains(tip))
            {
                panel.Children.Add(tip);
            }
        };
        button.Click += (_, _) => tip.IsOpen = true;
        return button;
    }

    private static GridView CreateGridView()
    {
        GridView grid = new();
        foreach (var name in new[] { "Accent", "Layer", "Card", "Stroke", "Solid", "Subtle", "Text", "Smoke" })
        {
            Border tile = new()
            {
                Width = 120,
                Height = 88,
                CornerRadius = new(8),
                Background = (Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"],
                Child = new TextBlock { Text = name, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center },
            };
            grid.Items.Add(tile);
        }

        return grid;
    }

    private static TreeView CreateTreeView()
    {
        TreeViewNode documents = new() { Content = "Documents", IsExpanded = true };
        documents.Children.Add(new TreeViewNode { Content = "Invoices" });
        documents.Children.Add(new TreeViewNode { Content = "Receipts" });
        TreeViewNode pictures = new() { Content = "Pictures" };
        pictures.Children.Add(new TreeViewNode { Content = "Prague" });
        TreeView tree = new();
        tree.RootNodes.Add(documents);
        tree.RootNodes.Add(pictures);
        return tree;
    }

    private static FlipView CreateFlipView()
    {
        FlipView flip = new();
        foreach (var slide in new[] { "First slide", "Second slide", "Third slide" })
        {
            flip.Items.Add(new TextBlock
            {
                Text = slide,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Style = (Style)Application.Current.Resources["TitleTextBlockStyle"],
            });
        }

        return flip;
    }

    private static NavigationView CreateNavigationView()
    {
        NavigationView nav = new() { IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed };
        nav.MenuItems.Add(new NavigationViewItem { Content = "Home", Icon = new SymbolIcon(Symbol.Home), IsSelected = true });
        nav.MenuItems.Add(new NavigationViewItem { Content = "Inbox", Icon = new SymbolIcon(Symbol.Mail) });
        nav.MenuItems.Add(new NavigationViewItem { Content = "Calendar", Icon = new SymbolIcon(Symbol.Calendar) });
        nav.Content = new TextBlock { Text = "Your page goes here", Margin = new(24) };
        return nav;
    }

    private static TabView CreateTabView()
    {
        TabView tabs = new();
        foreach (var header in new[] { "Report.xaml", "Styles.xaml" })
        {
            tabs.TabItems.Add(new TabViewItem { Header = header, Content = new TextBlock { Text = header, Margin = new(16) } });
        }

        tabs.AddTabButtonClick += (sender, _) =>
            sender.TabItems.Add(new TabViewItem { Header = $"Untitled {sender.TabItems.Count + 1}" });
        tabs.TabCloseRequested += (sender, args) => sender.TabItems.Remove(args.Tab);
        return tabs;
    }

    private static SelectorBar CreateSelectorBar()
    {
        SelectorBar bar = new();
        bar.Items.Add(new SelectorBarItem { Text = "Recent", IsSelected = true });
        bar.Items.Add(new SelectorBarItem { Text = "Shared" });
        bar.Items.Add(new SelectorBarItem { Text = "Favorites" });
        return bar;
    }

    private static Pivot CreatePivot()
    {
        Pivot pivot = new();
        pivot.Items.Add(new PivotItem { Header = "All", Content = new TextBlock { Text = "Everything" } });
        pivot.Items.Add(new PivotItem { Header = "Unread", Content = new TextBlock { Text = "Only the new ones" } });
        return pivot;
    }

    private static StackPanel CreateStack()
    {
        StackPanel stack = new() { VerticalAlignment = VerticalAlignment.Center };
        foreach (var name in new[] { "One", "Two", "Three" })
        {
            stack.Children.Add(new Button { Content = name });
        }

        return stack;
    }

    private static Grid CreateGrid()
    {
        Grid grid = new();
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(240) });
        grid.ColumnDefinitions.Add(new ColumnDefinition());

        var header = Cell("Header");
        Grid.SetColumnSpan(header, 2);
        grid.Children.Add(header);

        var pane = Cell("240");
        Grid.SetRow(pane, 1);
        grid.Children.Add(pane);

        var main = Cell("*");
        Grid.SetRow(main, 1);
        Grid.SetColumn(main, 1);
        grid.Children.Add(main);
        return grid;

        static Border Cell(string text) => new()
        {
            Padding = new(16),
            CornerRadius = new(4),
            Background = (Brush)Application.Current.Resources["AccentFillColorSecondaryBrush"],
            Child = new TextBlock { Text = text, Foreground = (Brush)Application.Current.Resources["TextOnAccentFillColorPrimaryBrush"] },
        };
    }

    private static SplitView CreateSplitView() => new()
    {
        Pane = new TextBlock { Text = "Pane", Margin = new(16) },
        Content = new TextBlock { Text = "Content", Margin = new(16) },
    };

    private static ScrollViewer CreateScrollViewer()
    {
        Rectangle wide = new()
        {
            Width = 1600,
            Height = 900,
            Fill = new LinearGradientBrush
            {
                StartPoint = new(0, 0),
                EndPoint = new(1, 1),
                GradientStops =
                {
                    new GradientStop { Color = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorLight2"], Offset = 0 },
                    new GradientStop { Color = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorDark2"], Offset = 1 },
                },
            },
        };
        return new ScrollViewer { Content = wide };
    }
}
