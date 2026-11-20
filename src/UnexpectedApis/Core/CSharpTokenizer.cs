namespace UnexpectedApis.Core;

public enum CSharpTokenKind
{
    Text,
    Keyword,
    Type,
    String,
    Comment,
}

public readonly record struct CSharpToken(CSharpTokenKind Kind, string Text);

/// <summary>Just enough C# lexing to color a snippet: keywords, type-like names, strings and comments.</summary>
public static class CSharpTokenizer
{
    private static readonly HashSet<string> Keywords =
    [
        "abstract", "as", "async", "await", "base", "bool", "break", "byte", "case", "catch", "char", "class", "const",
        "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "false", "finally", "float",
        "for", "foreach", "get", "if", "in", "init", "int", "interface", "internal", "is", "long", "namespace", "new", "null",
        "object", "out", "override", "partial", "private", "protected", "public", "readonly", "record", "ref", "return",
        "sealed", "set", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "using",
        "var", "virtual", "void", "when", "while", "with", "yield",
    ];

    public static List<CSharpToken> Tokenize(string source)
    {
        List<CSharpToken> tokens = new();
        var i = 0;

        while (i < source.Length)
        {
            var start = i;
            var c = source[i];

            if (c == '/' && Peek(source, i + 1) == '/')
            {
                var end = source.IndexOf('\n', i);
                i = end < 0 ? source.Length : end + 1;
                tokens.Add(new(CSharpTokenKind.Comment, source[start..i]));
            }
            else if (c == '/' && Peek(source, i + 1) == '*')
            {
                var end = source.IndexOf("*/", i + 2, StringComparison.Ordinal);
                i = end < 0 ? source.Length : end + 2;
                tokens.Add(new(CSharpTokenKind.Comment, source[start..i]));
            }
            else if (c is '"' or '\'' || (c is '@' or '$' && Peek(source, i + 1) is '"' or '@' or '$'))
            {
                i = ReadString(source, i);
                tokens.Add(new(CSharpTokenKind.String, source[start..i]));
            }
            else if (char.IsLetter(c) || c == '_')
            {
                while (i < source.Length && (char.IsLetterOrDigit(source[i]) || source[i] == '_'))
                {
                    i++;
                }

                var word = source[start..i];
                var kind = Keywords.Contains(word) ? CSharpTokenKind.Keyword
                    : char.IsUpper(word[0]) ? CSharpTokenKind.Type
                    : CSharpTokenKind.Text;
                Add(tokens, kind, word);
            }
            else
            {
                Add(tokens, CSharpTokenKind.Text, c.ToString());
                i++;
            }
        }

        return tokens;
    }

    private static char Peek(string source, int index) => index < source.Length ? source[index] : '\0';

    private static int ReadString(string source, int start)
    {
        var i = start;
        var verbatim = false;
        while (source[i] is '@' or '$')
        {
            verbatim |= source[i] == '@';
            i++;
        }

        var quote = source[i];
        i++;
        while (i < source.Length)
        {
            if (source[i] == quote)
            {
                if (verbatim && Peek(source, i + 1) == quote)
                {
                    i += 2;
                    continue;
                }

                return i + 1;
            }

            i += !verbatim && source[i] == '\\' ? 2 : 1;
        }

        return source.Length;
    }

    /// <summary>Merges runs of plain text so the view creates fewer inlines.</summary>
    private static void Add(List<CSharpToken> tokens, CSharpTokenKind kind, string text)
    {
        if (kind == CSharpTokenKind.Text && tokens.Count > 0 && tokens[^1].Kind == CSharpTokenKind.Text)
        {
            tokens[^1] = new(CSharpTokenKind.Text, tokens[^1].Text + text);
        }
        else
        {
            tokens.Add(new(kind, text));
        }
    }
}
