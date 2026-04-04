using Music.Catalogue.Repository.Model;

namespace Music.Catalogue.Repository.Abstractions;

public interface IRepository
{
    Task<Songs?> GetCanzonePerIDAsync(string id, CancellationToken cancellationToken = default);

    Task InsertCanzoneAsync(string id, string titolo, string artista, string album, DateTime data, int durata, CancellationToken cancellationToken = default);

    Task<List<Songs>?> GetCanzonePerNomeAsync(string titolo, CancellationToken cancellationToken = default);
}