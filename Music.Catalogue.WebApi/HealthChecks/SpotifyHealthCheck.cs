using Microsoft.Extensions.Diagnostics.HealthChecks;
using Music.Catalogue.Spotify.Abstractions;

namespace MusicCatalogue.HealthChecks;

/// <summary>
/// Verifica che sia possibile ottenere un token OAuth valido per Spotify.
/// Il provider mette in cache il token: in condizioni normali questo check NON genera
/// una nuova chiamata HTTP verso Spotify ad ogni /health, solo quando il token è scaduto.
/// </summary>
public class SpotifyHealthCheck(ISpotifyTokenProvider tokenProvider) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await tokenProvider.GetTokenAsync(cancellationToken);

            return !string.IsNullOrWhiteSpace(token)
                ? HealthCheckResult.Healthy("Token Spotify valido")
                : HealthCheckResult.Unhealthy("Impossibile ottenere un token Spotify valido");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Errore durante la verifica del token Spotify", ex);
        }
    }
}
