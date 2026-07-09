using Music.Catalogue.Business;
using Music.Catalogue.Business.Abstractions;
using Microsoft.EntityFrameworkCore;
using Music.Catalogue.ClientHttp;
using Music.Catalogue.ClientHttp.Abstractions;
using Music.Catalogue.Repository;
using Music.Catalogue.Repository.Abstractions;
using Music.Catalogue.Spotify;
using Music.Catalogue.Spotify.Abstractions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CatalogueDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IBusiness, Business>();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddHttpClient<ISpotifyClient, SpotifyClient>();
builder.Services.AddHttpClient<IClientHttp, ClientHttp>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<CatalogueDbContext>();
db.Database.Migrate();

app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();