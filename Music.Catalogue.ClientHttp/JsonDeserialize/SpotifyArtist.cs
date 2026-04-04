using System.Text.Json.Serialization;

namespace Music.Catalogue.ClientHttp.JsonDeserialize;

public class SpotifyArtist
{
    [JsonPropertyName("name")]
    public required string name { get; set; }
}