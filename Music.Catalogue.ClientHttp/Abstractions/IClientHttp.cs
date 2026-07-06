using Music.Catalogue.Shared;

namespace Music.Catalogue.ClientHttp.Abstractions;

public interface IClientHttp
{
    Task<SongDTO?> SearchCanzoniByIdSpotify(string id, CancellationToken cancellationToken);
    Task<List<SongDTO>?> SearchCanzoniPerArtistaAsync(string titolo, CancellationToken cancellationToken = default);
}