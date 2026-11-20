namespace UnexpectedApis.Core;

public enum XamlTokenKind
{
    Text,
    Punctuation,
    Tag,
    AttributeName,
    AttributeValue,
    Comment,
}

public readonly record struct XamlToken(XamlTokenKind Kind, string Text);

public static class XamlTokenizer
{
    private const string CommentStart = "<!--";
    private const string CommentEnd = "-->";

    public static List<XamlToken> Tokenize(string source)
    {
        List<XamlToken> tokens = new();
        var i = 0;

        while (i < source.Length)
        {
            if (string.CompareOrdinal(source, i, CommentStart, 0, CommentStart.Length) == 0)
            {
                var end = source.IndexOf(CommentEnd, i, StringComparison.Ordinal);
                var stop = end < 0 ? source.Length : end + CommentEnd.Length;
                tokens.Add(new(XamlTokenKind.Comment, source[i..stop]));
                i = stop;
            }
            else if (source[i] == '<')
            {
                i = ReadTag(source, i, tokens);
            }
            else
            {
                var next = source.IndexOf('<', i);
                var stop = next < 0 ? source.Length : next;
                tokens.Add(new(XamlTokenKind.Text, source[i..stop]));
                i = stop;
            }
        }

        return tokens;
    }

    private static int ReadTag(string source, int start, List<XamlToken> tokens)
    {
        var i = start + 1;
        if (i < source.Length && source[i] is '/' or '?')
        {
            i++;
        }

        tokens.Add(new(XamlTokenKind.Punctuation, source[start..i]));
        i = Read(source, i, XamlTokenKind.Tag, tokens, c => !char.IsWhiteSpace(c) && c is not ('>' or '/'));

        while (i < source.Length)
        {
            var c = source[i];
            if (c == '>')
            {
                tokens.Add(new(XamlTokenKind.Punctuation, ">"));
                return i + 1;
            }

            if (char.IsWhiteSpace(c))
            {
                i = Read(source, i, XamlTokenKind.Punctuation, tokens, char.IsWhiteSpace);
            }
            else if (c is '=' or '/' or '?')
            {
                tokens.Add(new(XamlTokenKind.Punctuation, c.ToString()));
                i++;
            }
            else if (c is '"' or '\'')
            {
                var close = source.IndexOf(c, i + 1);
                var stop = close < 0 ? source.Length : close + 1;
                tokens.Add(new(XamlTokenKind.AttributeValue, source[i..stop]));
                i = stop;
            }
            else
            {
                i = Read(source, i, XamlTokenKind.AttributeName, tokens, ch => !char.IsWhiteSpace(ch) && ch is not ('=' or '>' or '/'));
            }
        }

        return i;
    }

    private static int Read(string source, int start, XamlTokenKind kind, List<XamlToken> tokens, Func<char, bool> keepGoing)
    {
        var i = start;
        while (i < source.Length && keepGoing(source[i]))
        {
            i++;
        }

        if (i > start)
        {
            tokens.Add(new(kind, source[start..i]));
        }

        return i;
    }
}
