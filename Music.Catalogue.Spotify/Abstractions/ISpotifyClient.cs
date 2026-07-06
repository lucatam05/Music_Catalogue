using Music.Catalogue.Shared;
namespace Music.Catalogue.Spotify.Abstractions;

public interface ISpotifyClient
{
    Task<List<SongDTO>?> SearchCanzoniAsync(string titolo, CancellationToken cancellationToken = default);

    Task<List<SongDTO>?> SearchCanzoniPerArtistaAsync(string artista, CancellationToken cancellationToken = default);
    
    Task<List<SongDTO>?> SearchCanzoniPerAlbumAsync(string album, CancellationToken cancellationToken = default);

    Task<SongDTO?> SearchCanzoniByIdSpotifyAsync(string id, CancellationToken cancellationToken = default);
}