using Microsoft.EntityFrameworkCore;
using Music.Catalogue.Repository.Model;

namespace Music.Catalogue.Repository;

public class CatalogueDbContext(DbContextOptions<CatalogueDbContext> dbContextOptions) : DbContext(dbContextOptions)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Songs>().HasKey(s => s.SpotifyId);
        modelBuilder.Entity<Songs>()
            .Property(u => u.DataUscita)
            .HasConversion(
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        modelBuilder.Entity<Songs>().ToTable("Songs");
    }
    
    public DbSet<Songs> SongsEnumerable { get; set; }
}