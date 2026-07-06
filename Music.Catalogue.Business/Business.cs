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
    
    public async Task<List<SongDTO>?> GetCanzonePerNomeAsync(string titolo, CancellationToken cancellationToken)
    {
        var canzoniDb = await repository.GetCanzonePerNomeAsync(titolo, cancellationToken);
        if (canzoniDb != null && canzoniDb.Any())
            return canzoniDb.Select(c => new SongDTO { 
                SpotifyId = c.SpotifyId,
                Titolo = c.Titolo,
                Artista = c.Artista,
                Album = c.Album,
                DataUscita = c.DataUscita,
                Durata = c.Durata}).ToList();
        
        List<SongDTO>? canzoni = await clientHttp.SearchCanzoniAsync(titolo, cancellationToken);
        if (canzoni is null)
            throw new ModelNotFoundException("Canzone non trovata");
        
        foreach (var canzone in canzoni)
        {
            var esistente = await repository.GetCanzonePerIDAsync(canzone.SpotifyId, cancellationToken);
            if (esistente is null)
                await InsertCanzoneAsync(canzone.SpotifyId,
                    canzone.Titolo,
                    canzone.Artista,
                    canzone.Album,
                    canzone.DataUscita,
                    canzone.Durata,
                    cancellationToken);
        }

        return canzoni;
    }

    public async Task<List<SongDTO>?> GetCanzoniPerArtistaAsync(string artista,
        CancellationToken cancellationToken = default)
    {
        var canzoniDb = await repository.GetCanzonePerArtistaAsync(artista, cancellationToken);
        if (canzoniDb != null && canzoniDb.Any())
            return canzoniDb.Select(c => new SongDTO { 
                SpotifyId = c.SpotifyId,
                Titolo = c.Titolo,
                Artista = c.Artista,
                Album = c.Album,
                DataUscita = c.DataUscita,
                Durata = c.Durata}).ToList();
        
        List<SongDTO>? canzoni = await clientHttp.SearchCanzoniPerArtistaAsync(artista, cancellationToken);
        if (canzoni is null)
            throw new ModelNotFoundException("Artista non trovato");
        
        foreach (var canzone in canzoni)
        {
            var esistente = await repository.GetCanzonePerIDAsync(canzone.SpotifyId, cancellationToken);
            if (esistente is null)
                await InsertCanzoneAsync(canzone.SpotifyId,
                    canzone.Titolo,
                    canzone.Artista,
                    canzone.Album,
                    canzone.DataUscita,
                    canzone.Durata,
                    cancellationToken);
        }

        return canzoni;
    }

    public async Task<List<SongDTO>?> GetCanzoniPerAlbumAsync(string album,
        CancellationToken cancellationToken = default)
    {
        var canzoniDb = await repository.GetCanzonePerAlbumAsync(album, cancellationToken);
        if (canzoniDb != null && canzoniDb.Any())
            return canzoniDb.Select(c => new SongDTO { 
                SpotifyId = c.SpotifyId,
                Titolo = c.Titolo,
                Artista = c.Artista,
                Album = c.Album,
                DataUscita = c.DataUscita,
                Durata = c.Durata}).ToList();
        
        List<SongDTO>? canzoni = await clientHttp.SearchCanzoniPerAlbumAsync(album, cancellationToken);
        if (canzoni is null)
            throw new ModelNotFoundException("Album non trovato");
        
        foreach (var canzone in canzoni)
        {
            var esistente = await repository.GetCanzonePerIDAsync(canzone.SpotifyId, cancellationToken);
            if (esistente is null)
                await InsertCanzoneAsync(canzone.SpotifyId,
                    canzone.Titolo,
                    canzone.Artista,
                    canzone.Album,
                    canzone.DataUscita,
                    canzone.Durata,
                    cancellationToken);
        }

        return canzoni;
    }
}