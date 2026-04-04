using System.Text.Json.Serialization;

namespace Music.Catalogue.ClientHttp.JsonDeserialize;

public class SpotifyAlbum
{
    [JsonPropertyName("name")]
    public required string Nome { get; set; }
    
    [JsonPropertyName("release_date")]
    public required string DataUscita { get; set; }
}