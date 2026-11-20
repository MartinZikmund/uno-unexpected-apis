namespace UnexpectedApis.Core;

public sealed record SearchEntry<T>(T Value, string Title, IReadOnlyList<string> Keywords);

public static class CatalogSearch
{
    private const int MinScatteredLength = 3;

    public static IEnumerable<T> Rank<T>(string query, IEnumerable<SearchEntry<T>> entries)
    {
        var terms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (terms.Length == 0)
        {
            return [];
        }

        return entries
            .Select(entry => (entry, scores: terms.Select(term => Score(term, entry)).ToArray()))
            .Where(x => x.scores.All(score => score > 0))
            .OrderByDescending(x => x.scores.Sum())
            .ThenBy(x => x.entry.Title, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.entry.Value);
    }

    private static int Score<T>(string term, SearchEntry<T> entry)
    {
        var title = entry.Title;
        if (title.Equals(term, StringComparison.OrdinalIgnoreCase))
        {
            return 100;
        }

        if (title.StartsWith(term, StringComparison.OrdinalIgnoreCase))
        {
            return 80;
        }

        if (Initials(title).StartsWith(term, StringComparison.OrdinalIgnoreCase))
        {
            return 70;
        }

        var index = title.IndexOf(term, StringComparison.OrdinalIgnoreCase);
        if (index > 0)
        {
            return char.IsUpper(title[index]) ? 60 : 40;
        }

        foreach (var keyword in entry.Keywords)
        {
            if (keyword.StartsWith(term, StringComparison.OrdinalIgnoreCase))
            {
                return 30;
            }

            if (keyword.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                return 20;
            }
        }

        return IsScatteredMatch(term, title) ? 10 : 0;
    }

    private static string Initials(string title) => string.Concat(title.Where(char.IsUpper));

    private static bool IsScatteredMatch(string term, string title)
    {
        if (term.Length < MinScatteredLength)
        {
            return false;
        }

        var position = 0;
        foreach (var c in title)
        {
            if (char.ToUpperInvariant(c) == char.ToUpperInvariant(term[position]) && ++position == term.Length)
            {
                return true;
            }
        }

        return false;
    }
}
