using System.Text;
using System.Text.RegularExpressions;

namespace UnexpectedApis.Core;

public readonly record struct XamlAttribute(string Name, string Value);

public static partial class XamlSnippet
{
    private const int MaxInlineAttributes = 2;
    private const string Indent = "    ";

    public static string Build(string tag, IReadOnlyList<XamlAttribute> attributes, IReadOnlyList<string>? body = null)
    {
        StringBuilder xaml = new();
        xaml.Append('<').Append(tag);

        var separator = attributes.Count > MaxInlineAttributes
            ? "\n" + new string(' ', tag.Length + 2)
            : " ";

        for (var i = 0; i < attributes.Count; i++)
        {
            xaml.Append(i == 0 ? " " : separator)
                .Append(attributes[i].Name).Append("=\"").Append(Escape(attributes[i].Value)).Append('"');
        }

        if (body is not { Count: > 0 })
        {
            return xaml.Append(" />").ToString();
        }

        xaml.Append(">\n");
        foreach (var line in body)
        {
            xaml.Append(Indent).Append(line).Append('\n');
        }

        return xaml.Append("</").Append(tag).Append('>').ToString();
    }

    private static string Escape(string value) =>
        LooseAmpersand().Replace(value, "&amp;").Replace("\"", "&quot;").Replace("<", "&lt;");

    [GeneratedRegex(@"&(?!#x?[0-9a-fA-F]+;|[a-z]+;)")]
    private static partial Regex LooseAmpersand();
}
