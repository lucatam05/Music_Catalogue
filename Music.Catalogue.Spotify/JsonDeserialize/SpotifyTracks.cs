using System.Text.Json.Serialization;

namespace Music.Catalogue.Spotify.JsonDeserialize;

public class SpotifyTracks
{
    [JsonPropertyName("items")]
    public required List<SpotifySong> Items { get; set; }
}