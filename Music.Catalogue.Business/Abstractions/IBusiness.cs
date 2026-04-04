using Music.Catalogue.Shared;

namespace Music.Catalogue.Business.Abstractions;

public interface IBusiness
{
    
    Task<List<SongDTO>?> GetCanzoniPerArtistaAsync(string artista, CancellationToken cancellationToken = default);
    
    Task<List<SongDTO>?> GetCanzoniPerAlbumAsync(string album, CancellationToken cancellationToken = default);

    Task<List<SongDTO>?> GetCanzonePerNomeAsync(string titolo, CancellationToken cancellationToken = default);
}