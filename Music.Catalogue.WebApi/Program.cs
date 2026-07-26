using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Music.Catalogue.Business;
using Music.Catalogue.Business.Abstractions;
using Music.Catalogue.ClientHttp;
using Music.Catalogue.ClientHttp.Abstractions;
using Music.Catalogue.Repository;
using Music.Catalogue.Repository.Abstractions;
using MusicCatalogue;
using MusicCatalogue.HealthChecks;
using MusicCatalogue.Middlewares;
using Serilog;

// Bootstrap logger: attivo prima ancora che il builder venga costruito,
// così cattura anche eventuali errori durante la configurazione dei servizi.
Log.Logger = new LoggerConfiguration()
    .Enrich.WithProperty("ServiceName", "CatalogueService")
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Avvio di CatalogueService...");

    var builder = WebApplication.CreateBuilder(args);

    // Sostituisce il logger di default con Serilog, leggendo la configurazione
    // dalla sezione "Serilog" di appsettings.json
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddDbContext<CatalogueDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddScoped<IBusiness, Business>();
    builder.Services.AddScoped<IRepository, Repository>();
    builder.Services.AddSpotifyHttpClients();
    builder.Services.AddHttpClient<IClientHttp, ClientHttp>();

    builder.Services.AddHealthChecks()
        .AddDbContextCheck<CatalogueDbContext>("database")
        .AddCheck<SpotifyHealthCheck>("spotify");

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<CatalogueDbContext>();
        db.Database.Migrate();
    }

    // Deve precedere UseSerilogRequestLogging per far sì che anche la riga di log
    // riassuntiva della richiesta sia arricchita con il CorrelationId
    app.UseMiddleware<CorrelationIdMiddleware>();

    // Logga automaticamente ogni richiesta HTTP (metodo, path, status code, durata)
    app.UseSerilogRequestLogging();

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseAuthorization();

    app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriter.WriteAsync
        })
        .AllowAnonymous();

    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "CatalogueService terminato in modo inatteso durante l'avvio");
}
finally
{
    Log.CloseAndFlush();
}
