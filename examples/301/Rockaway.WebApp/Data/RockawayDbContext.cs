using Microsoft.EntityFrameworkCore;
using Rockaway.WebApp.Data.Entities;
using Rockaway.WebApp.Data.Sample;

namespace Rockaway.WebApp.Data;

// We must declare a constructor that takes a DbContextOptions<RockawayDbContext>
// if we want to use Asp.NET to configure our database connection and provider.
public class RockawayDbContext(DbContextOptions<RockawayDbContext> options) : DbContext(options) {

	public DbSet<Artist> Artists { get; set; } = default!;

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);
		modelBuilder.Entity<Artist>().HasData(SampleData.Artists.AllArtists);
	}
}