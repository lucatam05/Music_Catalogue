using System.Text.Json.Serialization;

namespace Music.Catalogue.ClientHttp.JsonDeserialize;

public class SpotifySong
{
    [JsonPropertyName("id")]
    public required string Spotify_id { get; set; }
    
    [JsonPropertyName("name")]
    public required string Titolo { get; set; }
    
    [JsonPropertyName("artists")]
    public required List<SpotifyArtist> Artisti { get; set; }
    
    [JsonPropertyName("album")]
    public required SpotifyAlbum Album { get; set; }
    
    [JsonPropertyName("duration_ms")]
    public int Durata { get; set; }
}