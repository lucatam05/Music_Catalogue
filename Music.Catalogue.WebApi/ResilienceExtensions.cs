using Microsoft.Extensions.Http.Resilience;
using Polly;
using Music.Catalogue.Spotify;
using Music.Catalogue.Spotify.Abstractions;

namespace MusicCatalogue;

public static class SpotifyResilienceExtensions
{
    public static IServiceCollection AddSpotifyHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient("SpotifyApi")
            .AddStandardResilienceHandler(ConfigureResilience);

        services.AddSingleton<ISpotifyTokenProvider, SpotifyTokenProvider>();

        services.AddHttpClient<ISpotifyClient, SpotifyClient>()
            .AddStandardResilienceHandler(ConfigureResilience);

        return services;
    }

    private static void ConfigureResilience(HttpStandardResilienceOptions options)
    {
        options.Retry.MaxRetryAttempts = 3;
        options.Retry.BackoffType = DelayBackoffType.Exponential;
        options.Retry.UseJitter = true;

        options.CircuitBreaker.FailureRatio = 0.5;
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
        options.CircuitBreaker.MinimumThroughput = 4;
        options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15);

        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(5);
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(20);
    }
}