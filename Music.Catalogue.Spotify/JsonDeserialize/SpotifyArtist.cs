using System.Text.Json.Serialization;

namespace Music.Catalogue.Spotify.JsonDeserialize;

public class SpotifyArtist
{
    [JsonPropertyName("name")]
    public required string name { get; set; }
}