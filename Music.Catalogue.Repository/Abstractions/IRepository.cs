using Music.Catalogue.Repository.Model;

namespace Music.Catalogue.Repository.Abstractions;

public interface IRepository
{
    Task<Songs?> GetCanzonePerIdAsync(string id, CancellationToken cancellationToken = default);

    Task InsertCanzoneAsync(string id, string titolo, string artista, string album, DateTime data, int durata, CancellationToken cancellationToken = default);

    Task<List<Songs>?> GetCanzonePerNomeAsync(string titolo, CancellationToken cancellationToken = default);
    Task<List<Songs>?> GetCanzonePerArtistaAsync(string artista, CancellationToken cancellationToken = default);
    Task<List<Songs>?> GetCanzonePerAlbumAsync(string album, CancellationToken cancellationToken = default);

}