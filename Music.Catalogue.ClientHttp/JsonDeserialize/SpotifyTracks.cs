using System.Text.Json.Serialization;

namespace Music.Catalogue.ClientHttp.JsonDeserialize;

public class SpotifyTracks
{
    [JsonPropertyName("items")]
    public required List<SpotifySong> Items { get; set; }
}