using Music.Catalogue.Repository.Abstractions;
using Music.Catalogue.Repository.Model;
using Microsoft.EntityFrameworkCore;

namespace Music.Catalogue.Repository;

public class Repository(CatalogueDbContext catalogueDbContext) : IRepository
{
    public async Task<Songs?> GetCanzonePerIDAsync(string id, CancellationToken cancellationToken = default)
    {
        return await catalogueDbContext.SongsEnumerable
            .FirstOrDefaultAsync(s => s.SpotifyId == id, cancellationToken);
    }

    public async Task InsertCanzoneAsync(string id, string titolo, string artista, string album, DateTime data,
        int durata, CancellationToken cancellationToken = default)
    {
        Songs canzone = new Songs
        {
            SpotifyId = id,
            Titolo = titolo,
            Artista = artista,
            Album = album,
            DataUscita = data,
            Durata = durata
        };
        
        catalogueDbContext.Add(canzone);
        await catalogueDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Songs>?> GetCanzonePerNomeAsync(string titolo, CancellationToken cancellationToken = default)
    {
        return await catalogueDbContext.SongsEnumerable
            .Where(s => s.Titolo.ToLower().Contains(titolo.ToLower()))
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<Songs>?> GetCanzonePerArtistaAsync(string artista, CancellationToken cancellationToken = default)
    {
        return await catalogueDbContext.SongsEnumerable
            .Where(s => s.Titolo.ToLower().Contains(artista.ToLower()))
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<Songs>?> GetCanzonePerAlbumAsync(string album, CancellationToken cancellationToken = default)
    {
        return await catalogueDbContext.SongsEnumerable
            .Where(s => s.Titolo.ToLower().Contains(album.ToLower()))
            .ToListAsync(cancellationToken);
    }
}