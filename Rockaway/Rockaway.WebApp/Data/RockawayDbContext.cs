using Microsoft.EntityFrameworkCore;
using Rockaway.WebApp.Data.Entities;
using Rockaway.WebApp.Data.Sample;

namespace Rockaway.WebApp.Data;

public class RockawayDbContext : DbContext {
	public RockawayDbContext(DbContextOptions<RockawayDbContext> options) : base(options) {

	}

	public DbSet<Artist> Artists { get; set; } = default!;

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);
		modelBuilder.Entity<Artist>().HasData(SampleData.Artists.AllArtists);
	}
}
