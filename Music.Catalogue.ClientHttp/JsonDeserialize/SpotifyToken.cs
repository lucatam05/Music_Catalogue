using System.Text.Json.Serialization;

namespace Music.Catalogue.ClientHttp.JsonDeserialize;

public class SpotifyToken
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }
    
    [JsonPropertyName("token_type")]
    public required string TokenType { get; set; }
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}