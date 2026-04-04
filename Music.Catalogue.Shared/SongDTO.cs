namespace Music.Catalogue.Shared;

public class SongDTO
{
    public required string SpotifyId { get; set; }
    public required string Titolo { get; set; }
    public required string Artista { get; set; }
    public required string Album { get; set; }
    public DateTime DataUscita { get; set; }
    public int Durata { get; set; }
}