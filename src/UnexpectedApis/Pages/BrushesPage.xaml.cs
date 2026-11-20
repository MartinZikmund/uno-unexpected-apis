using UnexpectedApis.Services;

namespace UnexpectedApis.Pages;

public sealed record BrushToken(string Key, string Group);

public sealed partial class BrushesPage : Page
{
    private static readonly List<BrushToken> Tokens = BuildTokens();

    public BrushesPage()
    {
        InitializeComponent();
        BrushList.ItemsSource = Tokens;
    }

    private static List<BrushToken> BuildTokens()
    {
        (string Group, string[] Names)[] groups =
        [
            ("Text", ["TextFillColorPrimary", "TextFillColorSecondary", "TextFillColorTertiary", "TextFillColorDisabled", "TextFillColorInverse", "AccentTextFillColorPrimary", "AccentTextFillColorSecondary", "AccentTextFillColorTertiary", "AccentTextFillColorDisabled", "TextOnAccentFillColorPrimary", "TextOnAccentFillColorSecondary", "TextOnAccentFillColorDisabled", "TextOnAccentFillColorSelectedText"]),
            ("Accent fill", ["AccentFillColorDefault", "AccentFillColorSecondary", "AccentFillColorTertiary", "AccentFillColorDisabled", "AccentFillColorSelectedTextBackground"]),
            ("Control fill", ["ControlFillColorDefault", "ControlFillColorSecondary", "ControlFillColorTertiary", "ControlFillColorDisabled", "ControlFillColorTransparent", "ControlFillColorInputActive", "ControlStrongFillColorDefault", "ControlStrongFillColorDisabled", "ControlSolidFillColorDefault", "ControlAltFillColorSecondary", "ControlAltFillColorTertiary", "ControlAltFillColorQuarternary", "ControlAltFillColorDisabled", "SubtleFillColorTransparent", "SubtleFillColorSecondary", "SubtleFillColorTertiary", "SubtleFillColorDisabled", "ControlOnImageFillColorDefault", "ControlOnImageFillColorSecondary", "ControlOnImageFillColorTertiary", "ControlOnImageFillColorDisabled"]),
            ("Stroke", ["ControlStrokeColorDefault", "ControlStrokeColorSecondary", "ControlStrokeColorOnAccentDefault", "ControlStrokeColorOnAccentSecondary", "ControlStrokeColorOnAccentTertiary", "ControlStrokeColorOnAccentDisabled", "ControlStrongStrokeColorDefault", "ControlStrongStrokeColorDisabled", "CardStrokeColorDefault", "CardStrokeColorDefaultSolid", "SurfaceStrokeColorDefault", "SurfaceStrokeColorFlyout", "SurfaceStrokeColorInverse", "DividerStrokeColorDefault", "FocusStrokeColorOuter", "FocusStrokeColorInner"]),
            ("Surface", ["CardBackgroundFillColorDefault", "CardBackgroundFillColorSecondary", "SmokeFillColorDefault", "LayerFillColorDefault", "LayerFillColorAlt", "LayerOnAcrylicFillColorDefault", "LayerOnMicaBaseAltFillColorDefault", "LayerOnMicaBaseAltFillColorSecondary", "LayerOnMicaBaseAltFillColorTertiary", "LayerOnMicaBaseAltFillColorTransparent", "SolidBackgroundFillColorBase", "SolidBackgroundFillColorBaseAlt", "SolidBackgroundFillColorSecondary", "SolidBackgroundFillColorTertiary", "SolidBackgroundFillColorQuarternary", "AcrylicBackgroundFillColorDefault", "AcrylicBackgroundFillColorBase"]),
            ("Status", ["SystemFillColorAttention", "SystemFillColorSuccess", "SystemFillColorCaution", "SystemFillColorCritical", "SystemFillColorNeutral", "SystemFillColorSolidNeutral", "SystemFillColorAttentionBackground", "SystemFillColorSuccessBackground", "SystemFillColorCautionBackground", "SystemFillColorCriticalBackground", "SystemFillColorNeutralBackground", "SystemFillColorSolidAttentionBackground", "SystemFillColorSolidNeutralBackground"]),
        ];

        return groups
            .SelectMany(group => group.Names.Select(name => new BrushToken(name + "Brush", group.Group)))
            .ToList();
    }

    private void OnFilterChanged(object sender, TextChangedEventArgs e)
    {
        var filter = FilterBox.Text.Trim();
        BrushList.ItemsSource = filter.Length == 0
            ? Tokens
            : Tokens.Where(t => t.Key.Contains(filter, StringComparison.OrdinalIgnoreCase) || t.Group.Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    private void OnBrushClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is BrushToken token)
        {
            AppServices.Copy($"{{ThemeResource {token.Key}}}", $"{token.Key} copied");
        }
    }
}
