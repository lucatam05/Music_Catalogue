using System.Text.Json.Serialization;

namespace Music.Catalogue.Spotify.JsonDeserialize;

public class SpotifySearchResponse
{
    [JsonPropertyName("tracks")]
    public required SpotifyTracks Tracks { get; set; }
}