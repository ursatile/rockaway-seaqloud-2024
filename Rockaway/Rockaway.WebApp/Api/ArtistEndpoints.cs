using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Rockaway.WebApp.Data;
using Rockaway.WebApp.Data.Entities;
namespace Rockaway.WebApp.Api;

public static class ArtistEndpoints {
	public static void MapArtistEndpoints(this IEndpointRouteBuilder routes) {
		var group = routes.MapGroup("/api/Artist").WithTags(nameof(Artist));

		group.MapGet("/", async (RockawayDbContext db) => {
			return await db.Artists.ToListAsync();
		})
		.WithName("GetAllArtists")
		.WithOpenApi();

		const int COUNT = 4;
		group.MapGet("/{id}/shows", async Task<Results<Ok<object>, NotFound>> (Guid id, int index, RockawayDbContext db) => {
			var artist = await db.Artists
				.Include(a => a.HeadlineShows)
				.ThenInclude(show => show.Venue)
				.AsNoTracking().FirstOrDefaultAsync(model => model.Id == id);
			if (artist == default) return TypedResults.NotFound();
			var result = new {
				next = new {
					href = $"/api/artist/shows?index={index + COUNT}"
				},
				count = artist.HeadlineShows.Count,
				items = artist.HeadlineShows.Skip(index).Take(COUNT).Select(show => new {
					date = show.Date.ToString(),
					artist = new {
						name = artist.Name,
						href = $"/api/artists/{artist.Id}"
					},
					venue = show.Venue.Name,
					venueAddress = show.Venue.Address
				})
			};
			return TypedResults.Ok((object) result);
		})
			.WithName("GetShowsByArtistId")
			.WithOpenApi();

		group.MapGet("/{id}", async Task<Results<Ok<Artist>, NotFound>> (Guid id, RockawayDbContext db) => {
			return await db.Artists.AsNoTracking()
					.FirstOrDefaultAsync(model => model.Id == id)
				is Artist model
					? TypedResults.Ok(model)
					: TypedResults.NotFound();
		})
			.WithName("GetArtistById")
			.WithOpenApi();

		group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (Guid id, Artist artist, RockawayDbContext db) => {
			var affected = await db.Artists
				.Where(model => model.Id == id)
				.ExecuteUpdateAsync(setters => setters
					.SetProperty(m => m.Id, artist.Id)
					.SetProperty(m => m.Name, artist.Name)
					.SetProperty(m => m.Description, artist.Description)
					.SetProperty(m => m.Slug, artist.Slug)
					);
			return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
		})
				.WithName("UpdateArtist")
				.WithOpenApi();

		group.MapPost("/", async (Artist artist, RockawayDbContext db) => {
			db.Artists.Add(artist);
			await db.SaveChangesAsync();
			return TypedResults.Created($"/api/Artist/{artist.Id}", artist);
		})
		.WithName("CreateArtist")
		.WithOpenApi();

		group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid id, RockawayDbContext db) => {
			var affected = await db.Artists
				.Where(model => model.Id == id)
				.ExecuteDeleteAsync();
			return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
		})
		.WithName("DeleteArtist")
		.WithOpenApi();
	}
}
