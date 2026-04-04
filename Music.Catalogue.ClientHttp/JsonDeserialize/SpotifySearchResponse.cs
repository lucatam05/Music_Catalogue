using System.Text.Json.Serialization;

namespace Music.Catalogue.ClientHttp.JsonDeserialize;

public class SpotifySearchResponse
{
    [JsonPropertyName("tracks")]
    public required SpotifyTracks Tracks { get; set; }
}