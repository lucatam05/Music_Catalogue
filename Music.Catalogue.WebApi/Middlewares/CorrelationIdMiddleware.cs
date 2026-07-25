using Serilog.Context;

namespace MusicCatalogue.Middlewares;

/// <summary>
/// Legge l'header di correlazione dalla richiesta in ingresso (o ne genera uno nuovo se assente),
/// lo rende disponibile ai log del servizio tramite Serilog LogContext e lo riflette nella response.
/// </summary>
public class CorrelationIdMiddleware(RequestDelegate next)
{
    public const string HeaderName = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        string correlationId = context.Request.Headers.TryGetValue(HeaderName, out var existing) &&
                                !string.IsNullOrWhiteSpace(existing)
            ? existing.ToString()
            : Guid.NewGuid().ToString();

        context.Items[HeaderName] = correlationId;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        /*PushProperty returns an object that implements IDisposable.
         When the using block ends, Dispose() is called automatically.*/
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}
