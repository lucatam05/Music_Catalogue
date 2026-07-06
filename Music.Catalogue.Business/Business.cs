using Music.Catalogue.Business.Abstractions;
using Music.Catalogue.Spotify.Abstractions;
using Music.Catalogue.Repository.Abstractions;
using Music.Catalogue.Shared;
using Music.Catalogue.Shared.Exceptions;

namespace Music.Catalogue.Business;

public class Business(IRepository repository, ISpotifyClient spotifyClient) : IBusiness
{
    private async Task InsertCanzoneAsync(string id, string titolo, string artista, string album, DateTime data,
        int durata, CancellationToken cancellationToken = default)
    {
        await repository.InsertCanzoneAsync(id, titolo, artista, album, data, durata, cancellationToken);
    }

    public async Task<SongDTO> GetCanzonePerIdAsync(string id, CancellationToken cancellationToken)
    {
        var cache = await repository.GetCanzonePerIdAsync(id, cancellationToken);
        if (cache is null)
        {
            SongDTO? canzone = await spotifyClient.SearchCanzoniByIdSpotifyAsync(id, cancellationToken);
            if (canzone is null)
                throw new SpotifyException("Canzone non trovata");
            await InsertCanzoneAsync(canzone.SpotifyId, canzone.Titolo, canzone.Artista, canzone.Album,
                canzone.DataUscita, canzone.Durata, cancellationToken);
            return canzone;
        }

        return new SongDTO
        {
            SpotifyId = cache.SpotifyId,
            Titolo = cache.Titolo,
            Artista = cache.Artista,
            Album = cache.Album,
            DataUscita = cache.DataUscita,
            Durata = cache.Durata
        };
    }
    
    public async Task<List<SongDTO>?> GetCanzonePerNomeAsync(string titolo, CancellationToken cancellationToken)
    {
        return await spotifyClient.SearchCanzoniAsync(titolo, cancellationToken);
    }

    public async Task<List<SongDTO>?> GetCanzoniPerArtistaAsync(string artista, CancellationToken cancellationToken)
    {
        return await spotifyClient.SearchCanzoniPerArtistaAsync(artista, cancellationToken);
    }

    public async Task<List<SongDTO>?> GetCanzoniPerAlbumAsync(string album, CancellationToken cancellationToken)
    {
        return await spotifyClient.SearchCanzoniPerAlbumAsync(album, cancellationToken);
    }
}