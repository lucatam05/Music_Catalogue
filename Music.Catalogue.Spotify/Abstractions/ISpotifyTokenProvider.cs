namespace Music.Catalogue.Spotify.Abstractions;

public interface ISpotifyTokenProvider
{
    Task<string?> GetTokenAsync(CancellationToken cancellationToken = default);
}