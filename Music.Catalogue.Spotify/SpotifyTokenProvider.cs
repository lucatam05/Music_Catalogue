using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Music.Catalogue.Spotify.Abstractions;
using Music.Catalogue.Spotify.JsonDeserialize;

namespace Music.Catalogue.Spotify;

public class SpotifyTokenProvider(IHttpClientFactory httpClientFactory, IConfiguration configuration) : ISpotifyTokenProvider
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private string? _cachedToken;
    private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;
    private static readonly TimeSpan SafetyMargin = TimeSpan.FromSeconds(60);
    
    public async Task<string?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        if (IsTokenValid())
            return _cachedToken;

        await _lock.WaitAsync(cancellationToken);
        try
        {
            // double-check: un'altra richiesta potrebbe aver già rinfrescato
            // il token mentre questa era in attesa del semaforo
            if (IsTokenValid())
                return _cachedToken;

            var httpClient = httpClientFactory.CreateClient("SpotifyApi");

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
            {
                _cachedToken = null;
                _expiresAt = DateTimeOffset.MinValue;
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenResponse = JsonSerializer.Deserialize<SpotifyToken>(json);

            _cachedToken = tokenResponse?.AccessToken;
            _expiresAt = DateTimeOffset.UtcNow.AddSeconds(tokenResponse?.ExpiresIn ?? 0);

            return _cachedToken;
        }
        finally
        {
            _lock.Release();
        }
    }

    private bool IsTokenValid() =>
        _cachedToken is not null && DateTimeOffset.UtcNow < _expiresAt - SafetyMargin;
}