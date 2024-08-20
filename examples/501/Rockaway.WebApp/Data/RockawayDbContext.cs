using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Rockaway.WebApp.Data.Entities;
using Rockaway.WebApp.Data.Sample;

namespace Rockaway.WebApp.Data;

// We must declare a constructor that takes a DbContextOptions<RockawayDbContext>
// if we want to use ASP.NET to configure our database connection and provider.
public class RockawayDbContext(DbContextOptions<RockawayDbContext> options)
	: IdentityDbContext<IdentityUser>(options) {

	public DbSet<Artist> Artists { get; set; } = default!;
	public DbSet<Venue> Venues { get; set; } = default!;

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);
		// Override EF Core's default table naming (which pluralizes entity names)
		// and use the same names as the C# classes instead
		var rockawayEntityNamespace = typeof(Artist).Namespace;
		var rockawayEntities = modelBuilder.Model.GetEntityTypes().Where(e => e.ClrType.Namespace == rockawayEntityNamespace);
		foreach (var entity in rockawayEntities) entity.SetTableName(entity.DisplayName());

		modelBuilder.Entity<Artist>(entity => {
			entity.HasIndex(artist => artist.Slug).IsUnique();
		});

		modelBuilder.Entity<Venue>(entity => {
			entity.HasIndex(venue => venue.Slug).IsUnique();
		});

		modelBuilder.Entity<Artist>()
			.HasData(SampleData.Artists.AllArtists);
		modelBuilder.Entity<Venue>()
			.HasData(SampleData.Venues.AllVenues);

		modelBuilder.Entity<IdentityUser>()
			.HasData(SampleData.Users.Admin);

	}
}