using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Music.Catalogue.Spotify.Abstractions;
using Music.Catalogue.Spotify.JsonDeserialize;
using Music.Catalogue.Shared;
using Music.Catalogue.Shared.Exceptions;

namespace Music.Catalogue.Spotify;

public class SpotifyClient(HttpClient httpClient, IConfiguration configuration) : ISpotifyClient
{
    private async Task<string?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        var clientId = configuration["Spotify:ClientId"];
        var clientSecret = configuration["Spotify:ClientSecret"];
    
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
    
        var request = new HttpRequestMessage(HttpMethod.Post, configuration["Spotify:TokenUrl"]);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        request.Content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });
    
        var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode) 
            return null;
    
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var tokenResponse = JsonSerializer.Deserialize<SpotifyToken>(json);
        return tokenResponse?.AccessToken;
    }

    private async Task<List<SongDTO>?> SearchAsync(string query, CancellationToken cancellationToken)
    {
        string? token = await GetTokenAsync(cancellationToken);
        if (token is null) 
            throw new SpotifyException("La richiesta verso Spotify non è andata a buon fine");

        var url = $"{configuration["Spotify:SearchUrl"]}?q={query}&type=track&limit=10";
    
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    
        var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new SpotifyException("La richiesta verso Spotify non è andata a buon fine");

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var spotifyResponse = JsonSerializer.Deserialize<SpotifySearchResponse>(json);
        
        if (spotifyResponse is null)
            throw new SpotifyException("La deserializzazione del JSON non è andata a buon fine");

        return spotifyResponse.Tracks.Items.Select(s =>
        {
            DateTime data;
            DateTime.TryParse(s.Album.DataUscita, out data);

            return new SongDTO()
            {
                SpotifyId = s.Spotify_id,
                Titolo = s.Titolo,
                Artista = s.Artisti.First().name,
                Album = s.Album.Nome,
                DataUscita = data,
                Durata = s.Durata / 1000
            };
        }).ToList();
    }

    public async Task<List<SongDTO>?> SearchCanzoniAsync(string titolo, CancellationToken cancellationToken = default)
    {
        return await SearchAsync($"track:{titolo}", cancellationToken);
    }
    
    public async Task<List<SongDTO>?> SearchCanzoniPerArtistaAsync(string artista, CancellationToken cancellationToken)
    { 
        // Usa il metodo SearchAsync interno, applicando il filtro sul tag "artist" di Spotify
        return await SearchAsync($"artist:{artista}", cancellationToken);
    }
    
    public async Task<List<SongDTO>?> SearchCanzoniPerAlbumAsync(string album, CancellationToken cancellationToken = default)
    {
        return await SearchAsync($"album:{album}", cancellationToken);
    }

    public async Task<SongDTO?> SearchCanzoniByIdSpotifyAsync(string id, CancellationToken cancellationToken)
    {
        string? token = await GetTokenAsync(cancellationToken);
        if (token is null) 
            throw new SpotifyException("La richiesta verso Spotify non è andata a buon fine");

        var url = $"{configuration["Spotify:TrackUrl"]}/{id}";    
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    
        var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode) 
            throw new SpotifyException("La richiesta verso Spotify non è andata a buon fine");

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var spotifyResponse = JsonSerializer.Deserialize<SpotifySong>(json);

        if (spotifyResponse is null)
            throw new SpotifyException("La deserializzazione del JSON non è andata a buon fine");

        DateTime data;
        DateTime.TryParse(spotifyResponse.Album.DataUscita, out data);

        return new SongDTO()
        {
            SpotifyId = spotifyResponse.Spotify_id,
            Titolo = spotifyResponse.Titolo,
            Artista = spotifyResponse.Artisti.First().name,
            Album = spotifyResponse.Album.Nome,
            DataUscita = data,
            Durata = spotifyResponse.Durata / 1000
        };
    }

    
}