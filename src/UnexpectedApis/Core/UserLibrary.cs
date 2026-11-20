using System.Text.Json;
using System.Text.Json.Serialization;

namespace UnexpectedApis.Core;

public sealed class UserLibrary
{
    public const int MaxRecents = 8;

    private readonly List<string> _favorites;
    private readonly List<string> _recents;

    public UserLibrary() : this(new State(null, null, null))
    {
    }

    private UserLibrary(State state)
    {
        _favorites = state.Favorites ?? new();
        _recents = state.Recents ?? new();
        Theme = state.Theme;
    }

    public IReadOnlyList<string> Favorites => _favorites;

    public IReadOnlyList<string> Recents => _recents;

    public string? Theme { get; set; }

    public bool IsFavorite(string id) => _favorites.Contains(id);

    /// <returns>Whether the item is a favorite after the toggle.</returns>
    public bool ToggleFavorite(string id)
    {
        if (_favorites.Remove(id))
        {
            return false;
        }

        _favorites.Add(id);
        return true;
    }

    public void Touch(string id)
    {
        _recents.Remove(id);
        _recents.Insert(0, id);
        if (_recents.Count > MaxRecents)
        {
            _recents.RemoveRange(MaxRecents, _recents.Count - MaxRecents);
        }
    }

    public string ToJson() => JsonSerializer.Serialize(new State(_favorites, _recents, Theme), LibraryJsonContext.Default.State);

    public static UserLibrary FromJson(string json)
    {
        try
        {
            return JsonSerializer.Deserialize(json, LibraryJsonContext.Default.State) is { } state
                ? new UserLibrary(state)
                : new UserLibrary();
        }
        catch (JsonException)
        {
            return new UserLibrary();
        }
    }

    internal sealed record State(List<string>? Favorites, List<string>? Recents, string? Theme);
}

[JsonSerializable(typeof(UserLibrary.State))]
internal sealed partial class LibraryJsonContext : JsonSerializerContext;
