using Music.Catalogue.Shared;
namespace Music.Catalogue.ClientHttp.Abstractions;

public interface IClientHttp
{
    Task<List<SongDTO>?> SearchCanzoniAsync(string titolo, CancellationToken cancellationToken = default);

    Task<List<SongDTO>?> SearchCanzoniPerArtistaAsync(string artista, CancellationToken cancellationToken = default);
    
    Task<List<SongDTO>?> SearchCanzoniPerAlbumAsync(string album, CancellationToken cancellationToken = default);

    Task<SongDTO?> SearchCanzoniByIDSpotify(string id, CancellationToken cancellationToken = default);
}