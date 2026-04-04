using Music.Catalogue.Business.Abstractions;
using Music.Catalogue.ClientHttp.Abstractions;
using Music.Catalogue.Repository.Abstractions;
using Music.Catalogue.Shared;
using Music.Catalogue.Shared.Exceptions;

namespace Music.Catalogue.Business;

public class Business(IRepository repository, IClientHttp clientHttp) : IBusiness
{
    private async Task InsertCanzoneAsync(string id, string titolo, string artista, string album, DateTime data,
        int durata, CancellationToken cancellationToken = default)
    {
        await repository.InsertCanzoneAsync(id, titolo, artista, album, data, durata, cancellationToken);
    }
    
    public async Task<SongDTO?> GetCanzonePerIDAsync(string id, CancellationToken cancellationToken = default)
    {
        Repository.Model.Songs? canzone = await repository.GetCanzonePerIDAsync(id, cancellationToken);

        if (canzone is null)
        {
            SongDTO? song = await clientHttp.SearchCanzoniByIDSpotify(id, cancellationToken);
            if (song is null)
                throw new ModelNotFoundException("Canzone non trovata!");
            await InsertCanzoneAsync(song.SpotifyId, song.Titolo, song.Artista, song.Album, song.DataUscita,
                song.Durata, cancellationToken);
            return song;
        }

        return new SongDTO
        {
            SpotifyId = canzone.SpotifyId,
            Titolo = canzone.Titolo,
            Artista = canzone.Artista,
            Album = canzone.Album,
            Durata = canzone.Durata,
            DataUscita = canzone.DataUscita
        };
    }
    
    public async Task<List<SongDTO>?> GetCanzonePerNomeAsync(string titolo, CancellationToken cancellationToken)
    {
        List<SongDTO>? canzoni = await clientHttp.SearchCanzoniAsync(titolo, cancellationToken);
        if (canzoni is null)
            throw new ModelNotFoundException("Canzone non trovata");

        return canzoni;
    }

    public async Task<List<SongDTO>?> GetCanzoniPerArtistaAsync(string artista,
        CancellationToken cancellationToken = default)
    {
        List<SongDTO>? canzoni = await clientHttp.SearchCanzoniPerArtistaAsync(artista, cancellationToken);
        if (canzoni is null)
            throw new ModelNotFoundException("Artista non trovato");

        return canzoni;
    }

    public async Task<List<SongDTO>?> GetCanzoniPerAlbumAsync(string album,
        CancellationToken cancellationToken = default)
    {
        List<SongDTO>? canzoni = await clientHttp.SearchCanzoniPerAlbumAsync(album, cancellationToken);
        if (canzoni is null)
            throw new ModelNotFoundException("Album non trovato");

        return canzoni;
    }
}