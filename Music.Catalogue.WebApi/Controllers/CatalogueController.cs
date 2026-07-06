using Microsoft.AspNetCore.Mvc;
using Music.Catalogue.Business.Abstractions;
using Music.Catalogue.Shared.Exceptions;

namespace MusicCatalogue.Controllers;


[ApiController]
[Route("[controller]/[action]")]
public class CatalogueController(IBusiness business) : ControllerBase
{
    [HttpGet(Name = "GetCanzoniPerNome")]
    public async Task<ActionResult> GetCanzoniPerNomeAsync(string titolo,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var list = await business.GetCanzonePerNomeAsync(titolo, cancellationToken);

            return Ok(list);
        }
        catch (ModelNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (SpotifyException ex)
        {
            return StatusCode(503, ex.Message);
        }
    }
    
    [HttpGet(Name = "GetCanzoniPerArtista")]
    public async Task<ActionResult> GetCanzoniPerArtista(string artista,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var list = await business.GetCanzoniPerArtistaAsync(artista, cancellationToken);

            return Ok(list);
        }
        catch (ModelNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (SpotifyException ex)
        {
            return StatusCode(503, ex.Message);
        }
    }
    
    [HttpGet(Name = "GetCanzoniPerAlbum")]
    public async Task<ActionResult> GetCanzoniPerAlbum(string album,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var list = await business.GetCanzoniPerAlbumAsync(album, cancellationToken);

            return Ok(list);
        }
        catch (ModelNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (SpotifyException ex)
        {
            return StatusCode(503, ex.Message);
        }
    }
    [HttpGet(Name = "GetCanzonePerID")]
    public async Task<ActionResult> GetCanzonePerIdAsync(string id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canzone = await business.GetCanzonePerIdAsync(id, cancellationToken);
            return Ok(canzone);
        }
        catch (ModelNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (SpotifyException ex)
        {
            return StatusCode(503, ex.Message);
        }
    }
}