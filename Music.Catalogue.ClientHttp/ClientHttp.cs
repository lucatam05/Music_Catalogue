using System.Net.Http.Json;
using Music.Catalogue.ClientHttp.Abstractions;
using Music.Catalogue.Shared;

namespace Music.Catalogue.ClientHttp;

public class ClientHttp(HttpClient httpClient) : IClientHttp
{
    public async Task<SongDTO?> SearchCanzoniByIdSpotify(string id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"/Catalogue/GetCanzonePerID?id={id}", cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;
    
        return await response.Content.ReadFromJsonAsync<SongDTO>(cancellationToken: cancellationToken);
    }
    
    public async Task<List<SongDTO>?> SearchCanzoniPerArtistaAsync(string artista, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"/Catalogue/GetCanzoniPerNome?artista={artista}", cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;
    
        return await response.Content.ReadFromJsonAsync<List<SongDTO>>(cancellationToken: cancellationToken);
    }
}